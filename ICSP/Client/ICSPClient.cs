using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using ICSP.Logging;

namespace ICSP.Client
{
  public class ICSPClient : IDisposable
  {
    public const int DefaultPort = 1319;

    private int mConnectionTimeout = 1;

    public event EventHandler<ClientConnectedEventArgs> ClientConnected;
    public event EventHandler<ClientConnectedEventArgs> ClientDisconnected;

    public event EventHandler<DataReceivedEventArgs> DataReceived;

    private SynchronizationContext mSyncContext;

    private NetworkStream mStream;

    private Socket mSocket;

    private bool mHasShutdown;

    private bool mIsDisposed;

    public ICSPClient()
    {
      mSyncContext = new SynchronizationContext();
    }

    ~ICSPClient()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
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

    public void Connect(string host, int port)
    {
      if(Connected)
      {
        Logger.LogInfo("StartClient: Host={0}, Port={1} => Client already connected", host, port);
        return;
      }

      mHasShutdown = false;

      // Disposing all Clients that's pending or connected
      mSocket?.Close();

      // Establish the remote endpoint for the socket
      var lIpAddress = Dns.GetHostAddresses(host)[0];

      Logger.LogInfo("StartClient: Host={0}, Port={1}", lIpAddress, port);

      mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

      try
      {
        var result = mSocket.BeginConnect(lIpAddress, port, null, null);

        if(mConnectionTimeout > 0)
        {
          var lSuccess = result.AsyncWaitHandle.WaitOne(mConnectionTimeout * 1000, true);

          if(!lSuccess)
          {
            mSocket.Close();

            // WSAETIMEDOUT: 10060(0x274C)
            // A connection attempt failed because the connected party did not properly respond after a period of time, 
            // or established connection failed because connected host has failed to respond
            // throw new SocketException(0x0000274c); // Timeout

            var lMsg = string.Format(
              "Failed to connect to the specified master controller.\r\n" +
              "Your current connection configuration is: {0}:{1}", lIpAddress, port);

            throw new ApplicationException(lMsg); // Timeout
          }
        }
        else
          result.AsyncWaitHandle.WaitOne();
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);

        throw;
      }

      if(mSocket.Connected)
      {
        Logger.LogInfo("Client connected: {0}:{1}", lIpAddress, port);

        RemoteIpAddress = ((IPEndPoint)mSocket.RemoteEndPoint).Address;
        LocalIpAddress = ((IPEndPoint)mSocket.LocalEndPoint).Address;

        mStream = new NetworkStream(mSocket);

        ReadAsync();

        if(ClientConnected != null)
          mSyncContext?.Send(x => ClientConnected(this, new ClientConnectedEventArgs(RemoteIpAddress)), null);
      }
      else
      {
        Logger.LogError("Client connect failed: {0}:{1}", lIpAddress, port);
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
          
          mSocket.Shutdown(SocketShutdown.Both);
        }
        catch(Exception ex)
        {
          Logger.LogError(ex);
        }
      }
    }

    public void Send(byte[] bytes)
    {
      try
      {
        if(mSocket != null)
        {
          Logger.LogDebug("{0} Bytes", bytes.Length);

          mStream.WriteAsync(bytes, 0, bytes.Length);
        }
      }
      catch(Exception ex)
      {
        // Der Datenstrom wird gerade durch einen früheren Vorgang im Datenstrom verwendet.
        Logger.LogError("Error: {0}", ex.Message);
      }
    }

    public void Send(ICSPMsg request)
    {
      if(request == null)
        throw new ArgumentNullException(nameof(request));

      try
      {
        if(mSocket != null)
        {
          Logger.LogDebug(false, "ICSPClient.Send[1]: MessageId=0x{0:X4}, Type={1}", request.ID, request.GetType().Name);
          Logger.LogDebug(false, "ICSPClient.Send[2]: Source={0}, Dest={1}", request.Source, request.Dest);

          mStream.WriteAsync(request.RawData, 0, request.RawData.Length);
        }
      }
      catch(Exception ex)
      {
        // Der Datenstrom wird gerade durch einen früheren Vorgang im Datenstrom verwendet.
        Logger.LogError("Error: {0}", ex.Message);
      }
    }

    public void SetSynchronizationContext(SynchronizationContext syncContext)
    {
      if(syncContext == null)
        throw new ArgumentNullException(nameof(syncContext));

      mSyncContext = syncContext;
    }

    private async void ReadAsync()
    {
      try
      {
        while(true)
        {
          var lBytes = new byte[mSocket.ReceiveBufferSize];

          var lResult = await mStream.ReadAsync(lBytes, 0, lBytes.Length);

          if(lResult == 0)
          {
            OnClientDisconnected();

            return;
          }

          var lMessage = Encoding.Default.GetString(lBytes, 0, lResult);

          Logger.LogVerbose("AsyncClient Data: {0} Bytes", lMessage.Length);

          Array.Resize(ref lBytes, lResult);

          OnDataReceived(lBytes);
        }
      }
      catch(IOException ex)
      {
        var lException = ex.InnerException as SocketException;

        if(lException?.SocketErrorCode == SocketError.ConnectionReset)
        {
          if(!mHasShutdown)
            Logger.LogError(lException.Message);
        }
        else
          Logger.LogError(ex.Message);

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
      Logger.LogInfo("Client Disconnected: {0}", RemoteIpAddress);
      
      if(ClientDisconnected != null)
        mSyncContext?.Send(x => ClientDisconnected(this, new ClientConnectedEventArgs(RemoteIpAddress)), null);

      mSocket = null;
    }

    private void OnDataReceived(byte[] bytes)
    {
      try
      {
        DataReceived?.Invoke(this, new DataReceivedEventArgs(bytes));
      }
      catch(Exception ex)
      {
        Logger.LogError(ex);
      }
    }

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

    public IPAddress RemoteIpAddress { get; private set; }

    public IPAddress LocalIpAddress { get; private set; }

    /*
    public static IPAddress GetLocalIPAddress()
    {
      IPEndPoint lEndPoint = null; // mSocket.LocalEndPoint as IPEndPoint;

      if(lEndPoint != null)
        return lEndPoint.Address;

      var lHost = Dns.GetHostEntry(Dns.GetHostName());

      foreach(var lIPAddress in lHost.AddressList)
      {
        if(lIPAddress.AddressFamily == AddressFamily.InterNetwork)
          return lIPAddress;
      }

      throw new Exception("Local IP Address Not Found!");
    }
    */
  }
}