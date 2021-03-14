using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Cryptography;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;
using ICSP.Core.Manager.ConnectionManager;

using Serilog.Events;

namespace ICSP.Core.Client
{
  public class ICSPClient : IDisposable
  {
    public const int DefaultPort = 1319;

    // Authentication
    public int ICSP_Authenticated           /**/ = 0x0001;
    public int ICSP_AuthenticatedNotAllowed /**/ = 0x8000;

    // Encryption
    public int ICSP_EncryptionAlgorithmNone    /**/ = 0;
    public int ICSP_EncryptionAlgorithmRC4     /**/ = 4;
    public int ICSP_EncryptionAlgorithmFuture1 /**/ = 6;
    public int ICSP_EncryptionAlgorithmMask    /**/ = 6;

    public int ICSP_PacketEncryptionRC4 = 2;

    private int mConnectionTimeout = 1;

    public event EventHandler<ClientOnlineOfflineEventArgs> ClientOnlineStatusChanged;

    public event EventHandler<ICSPMsgDataEventArgs> DataReceived;

    private NetworkStream mStream;

    private TcpClient mSocket;

    private bool mHasShutdown;

    private int mTaskConnectAsyncRunning = 0;

    private bool mIsDisposed;

    public ICSPClient()
    {
      // Credentials = new NetworkCredential(ICSPManager.DefaultUsername, ICSPManager.DefaultPassword);
    }

    public static MsgFactory Factory { get; } = new MsgFactory();

    /// <summary>
    /// Gets the time to wait while trying to establish a connection
    /// before terminating the attempt and generating an error.
    /// </summary>
    /// <remarks>
    /// You can set the amount of time a connection waits to time out by 
    /// using the ConnectTimeout or Connection Timeout keywords in the connection string.
    /// A value of 0 indicates no limit, and should be avoided in a ConnectionString 
    /// because an attempt to connect waits indefinitely.
    /// </remarks>
    public int ConnectionTimeout
    {
      get
      {
        return mConnectionTimeout;
      }
      set
      {
        if(value < 0)
          throw new ArgumentOutOfRangeException(nameof(ConnectionTimeout));

        mConnectionTimeout = value;
      }
    }

    public bool Connected
    {
      get { return mSocket?.Connected ?? false; }
    }

    public RC4 CryptoProvider { get; set; }

    public EncryptionMode EncryptionMode { get; set; }

    public IPEndPoint RemoteEndPoint { get; private set; }

    public IPEndPoint LocalEndPoint { get; private set; }

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if(!mIsDisposed)
      {
        if(disposing)
        {
          // Verwalteten Zustand (verwaltete Objekte) entsorgen
          if(mSocket != null)
          {
            mSocket.Close();
            mSocket = null;
          }
        }

        mIsDisposed = true;
      }
    }

    public async Task ConnectAsync(string host, int port)
    {
      // Ensure that method would be called only once
      if(Interlocked.Exchange(ref mTaskConnectAsyncRunning, 1) != 0)
        return;

      try
      {
        if(Connected)
        {
          Logger.LogInfo("Host={0:l}, Port={1} => Client already connected", host, port);
          return;
        }

        mHasShutdown = false;

        // Disposing all Clients that's pending or connected
        mSocket?.Close();

        // Establish the remote endpoint for the socket
        var lIpAddress = Dns.GetHostAddresses(host)[0];

        Logger.LogInfo("Host={0:l}, Port={1}", lIpAddress, port);

        mSocket = new TcpClient(AddressFamily.InterNetwork)
        {
          ReceiveBufferSize = 8192,
          SendBufferSize = 8192,
        };

        try
        {
          var lTask = mSocket.ConnectAsync(lIpAddress, port);

          if(mConnectionTimeout > 0)
          {
            var lSuccess = lTask.Wait(TimeSpan.FromSeconds(mConnectionTimeout));

            if(!lSuccess)
            {
              mSocket.Close();

              // WSAETIMEDOUT: 10060(0x274C)
              // A connection attempt failed because the connected party did not properly respond after a period of time, 
              // or established connection failed because connected host has failed to respond
              // throw new SocketException(0x0000274c); // Timeout

              var lMsg = string.Format(
                "Failed to connect to the specified master controller. Your current connection configuration is: {0}:{1}", lIpAddress, port);

              throw new ApplicationException(lMsg); // Timeout
            }
          }
          else
          {
            await lTask;
          }
        }
        catch(ApplicationException)
        {
          throw;
        }
        catch(Exception ex)
        {
          Logger.LogError(ex.Message);

          throw;
        }

        if(mSocket.Connected)
        {
          RemoteEndPoint = ((IPEndPoint)mSocket.Client.RemoteEndPoint);
          LocalEndPoint = ((IPEndPoint)mSocket.Client.LocalEndPoint);

          Logger.LogInfo("Client connected: RemoteEndPoint={0:l}:{1}, LocalEndPoint={2:l}:{3}", RemoteEndPoint.Address, RemoteEndPoint.Port, LocalEndPoint.Address, LocalEndPoint.Port);

          mStream = new NetworkStream(mSocket.Client, true);

          var mCts = new CancellationTokenSource();

          ReadAsync(mCts.Token);

          ClientOnlineStatusChanged?.Invoke(this, new ClientOnlineOfflineEventArgs(0, true, RemoteEndPoint?.ToString()));
        }
        else
        {
          Logger.LogError("Client connect failed: {0:l}:{1}", lIpAddress, port);
        }
      }
      finally
      {
        Interlocked.Exchange(ref mTaskConnectAsyncRunning, 0);
      }
    }

    public void Disconnect()
    {
      if(mSocket != null)
      {
        try
        {
          Logger.LogInfo("Socket -> Shutdown(SocketShutdown.Both)");

          // Shutdown generate a IOException in ReadAsync
          mHasShutdown = true;

          mSocket?.Client?.Shutdown(SocketShutdown.Both);
        }
        catch(Exception ex)
        {
          Logger.LogError(ex);
        }
      }
    }

    public async Task SendAsync(byte[] bytes)
    {
      try
      {
        if(mSocket != null)
        {
          Logger.LogVerbose("{0} Bytes", bytes.Length);

          await mStream?.WriteAsync(bytes, 0, bytes.Length);
        }
      }
      catch(Exception ex)
      {
        // Der Datenstrom wird gerade durch einen früheren Vorgang im Datenstrom verwendet.
        Logger.LogError("Error: {0}", ex.Message);
      }
    }

    public async Task SendAsync(ICSPMsg request)
    {
      if(request == null)
        throw new ArgumentNullException(nameof(request));

      try
      {
        if(mSocket != null)
        {
          Logger.LogVerbose(false, "ICSPClient.Send[1]: MessageId=0x{0:X4}, Source={1:l}, Dest={2:l}, Type={3:l}", request.ID, request.Source, request.Dest, request.GetType().Name);
          Logger.LogVerbose(false, "ICSPClient.Send[2]: Data={0:l}", BitConverter.ToString(request.RawData).Replace("-", " "));

          if((EncryptionMode & EncryptionMode.RC4_Send) > 0)
          {
            var lMsg = new ICSPEncryptedMsg(CryptoProvider, request, 4);

            Logger.LogVerbose(false, "ICSPClient.Send[3]: Data={0:l}", BitConverter.ToString(lMsg.RawData).Replace("-", " "));

            await mStream?.WriteAsync(lMsg.RawData, 0, lMsg.RawData.Length);
          }
          else
          {
            await mStream?.WriteAsync(request.RawData, 0, request.RawData.Length);
          }
        }
      }
      catch(Exception ex)
      {
        // Der Datenstrom wird gerade durch einen früheren Vorgang im Datenstrom verwendet.
        Logger.LogError("Error: {0}", ex.Message);
      }
    }

    private async void ReadAsync(CancellationToken cancellationToken)
    {
      try
      {
        var lBytes = new List<byte>();

        while(true)
        {
          var lBuffer = new byte[mSocket?.ReceiveBufferSize ?? 0];

          var lCount = await mStream.ReadAsync(lBuffer, 0, lBuffer.Length, cancellationToken);

          if(lCount == 0)
          {
            OnClientDisconnected();
            return;
          }

          Logger.LogVerbose("Data {0} bytes", lCount);

          lBytes.AddRange(lBuffer.Range(0, lCount));

          // Incoming packages are not always complete
          // Preprocess Packet
          while(lBytes.Count >= ICSPMsg.PacketLengthMin)
          {
            if(Logger.LogLevel <= LogEventLevel.Verbose)
              Logger.LogVerbose("Data 0x: {0:l}", BitConverter.ToString(lBytes.ToArray()).Replace("-", " "));

            var lProtocol = lBytes[0];

            if(lProtocol != ICSPMsg.ProtocolValue && lProtocol != ICSPEncryptedMsg.ProtocolValue)
            {
              Logger.LogError($"Unknonw protocol: Value=0x{lProtocol:X2}");

              lBytes.Clear();

              break;
            }

            // +4 => Protocol (1), Length (2), Checksum (1)
            var lPacketSize = ((lBytes[1] << 8) | lBytes[2]) + 4;

            if(lBytes.Count >= lPacketSize)
            {
              var lPacketBytes = lBytes.GetRange(0, lPacketSize).ToArray();

              lBytes.RemoveRange(0, lPacketSize);

              switch(lProtocol)
              {
                // ================================
                // Default data
                // ================================
                case ICSPMsg.ProtocolValue:
                {
                  var lMsg = Factory.FromData(lPacketBytes);

                  OnDataReceived(new ICSPMsgDataEventArgs(lMsg));

                  break;
                }
                // ================================
                // Encrypted data
                // ================================
                case ICSPEncryptedMsg.ProtocolValue:
                {
                  if(CryptoProvider == null)
                  {
                    Logger.LogError("Received encrypted packet but property CryptoProvider not initialized.");
                    break;
                  }

                  var lEncryptedMsg = new ICSPEncryptedMsg(CryptoProvider, lPacketBytes);

                  if(Logger.LogLevel <= LogEventLevel.Verbose)
                    lEncryptedMsg.WriteLogVerbose();

                  lPacketBytes = lEncryptedMsg.GetDecryptedData();

                  var lMsg = Factory.FromData(lPacketBytes);

                  OnDataReceived(new ICSPMsgDataEventArgs(lMsg));

                  break;
                }
              }
            }
            else
            {
              Logger.LogVerbose("Buffer: {0} bytes, needed {1} bytes", lCount, lPacketSize);
              break;
            }
          }
        }
      }
      catch(IOException ex)
      {
        // Shutdown generate a IOException in ReadAsync
        if(ex.InnerException is SocketException socketEx)
        {
          if(socketEx.SocketErrorCode == SocketError.ConnectionReset || socketEx.SocketErrorCode == SocketError.OperationAborted)
          {
            if(!mHasShutdown)
              Logger.LogError(socketEx.Message);
          }
          else
            Logger.LogError(socketEx.Message);
        }
        else
        {
          if(!mHasShutdown)
            Logger.LogError(ex.Message);
        }

        OnClientDisconnected();
      }
      catch(Exception ex)
      {
        Logger.LogError(ex);
      }

      mHasShutdown = false;
    }

    private void OnClientDisconnected()
    {
      Logger.LogInfo("Client Disconnected: {0:l}", RemoteEndPoint);
      
      EncryptionMode = EncryptionMode.None;

      ClientOnlineStatusChanged?.Invoke(this, new ClientOnlineOfflineEventArgs(0, false, RemoteEndPoint?.ToString()));

      mSocket = null;
    }

    private void OnDataReceived(ICSPMsgDataEventArgs e)
    {
      try
      {
        DataReceived?.Invoke(this, e);
      }
      catch(Exception ex)
      {
        Logger.LogError(ex);
      }
    }
  }
}