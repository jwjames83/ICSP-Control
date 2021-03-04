using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Client
{
  public class ICSPClient : IDisposable
  {
    public const int DefaultPort = 1319;

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

          await mStream?.WriteAsync(request.RawData, 0, request.RawData.Length);
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

          Logger.LogVerbose("Data: {0} Bytes", lCount);

          lBytes.AddRange(lBuffer.Range(0, lCount));

          // Incoming packages are not always complete
          // Preprocess Packet
          while(lBytes.Count >= ICSPMsg.PacketLengthMin)
          {
            // +4 => Protocol (1), Length (2), Checksum (1)
            var lSize = ((lBytes[1] << 8) | lBytes[2]) + 4;

            if(lBytes.Count >= lSize)
            {
              var lDataSize = ((lBytes[1] << 8) | lBytes[2]);

              ProcessRx(lBytes[0], lDataSize, lBytes.GetRange(3, lSize - 3).ToArray());

              var lMsg = Factory.FromData(lBytes.GetRange(0, lSize).ToArray());

              lBytes.RemoveRange(0, lSize);

              OnDataReceived(new ICSPMsgDataEventArgs(lMsg));

              // mManager.OnDataReceived(this, lMsg);
            }
            else
            {
              Logger.LogVerbose("Buffer: {0} Bytes, Needed {1} Bytes", lCount, lSize);
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

    protected MessageDigest m_md = new MessageDigest();

    private int ProcessRx(int protocol, int size, byte[] buffer)
    {
      int desSystem;
      int desDevice;
      int desPort;
      int srcSystem;
      int srcDevice;
      int srcPort;
      int id;

      bool nIsNewbee;

      IcspConnectionNode icspConnectionNode = null;

      if(buffer == null)
      {
        Console.WriteLine("IcspTcpTransport::processRX - bad buffer/packet");
        return 0;
      }

      if(protocol != 2 && protocol != 4)
      {
        Console.WriteLine("IcspTcpTransport::processRX Protocol = " + protocol);
        return 0;
      }

      var lDataSize = size - 1; // - Checksum

      byte checksum = (byte)protocol;

      checksum = (byte)(checksum + (size >> 8));
      checksum = (byte)(checksum + (size & 0xFF));

      int index;

      for(index = 0; index < size; index++)
        checksum = (byte)(checksum + buffer[index]);

      if(checksum != buffer[size])
      {
        Console.WriteLine("IcspTcpTransport::processRX checksum error packet");
        return 0;
      }

      DataInputStream dataInputStream = new DataInputStream(new MemoryStream(buffer, 0, size));

      if(protocol == 4)
      {
        try
        {
          byte[] arrayOfByte;

          dataInputStream.readUnsignedShort();
          dataInputStream.readUnsignedShort();

          int i3 = dataInputStream.readUnsignedByte();

          if(i3 == 2)
          {
            desSystem = dataInputStream.readUnsignedByte();
            desPort = size - 6 - desSystem;
            byte[] arrayOfByte1 = new byte[4];

            dataInputStream.read(arrayOfByte1);

            srcDevice = dataInputStream.readUnsignedShort();
            srcPort = dataInputStream.readUnsignedShort();

            icspConnectionNode = getConnectionNode(srcPort);

            if(icspConnectionNode != null)
            {
              Crypto crypto = icspConnectionNode.getCrypto();

              if(crypto != null && crypto is ARC4)
              {
                ARC4 aRC4 = (ARC4)crypto;

                arrayOfByte = new byte[desPort];

                dataInputStream.read(arrayOfByte);

                aRC4.crypt(arrayOfByte, 0, desPort, arrayOfByte1);
              }
              else
              {
                Console.WriteLine("IcspTcpTransport.processRX: Received encrypted packet but encryption not initialized for device");
                return 0;
              }
            }
            else
            {
              Console.WriteLine("IcspTcpTransport.processRX: Received encrypted packet for unknown device");
              return 0;
            }
          }
          else
          {
            Console.WriteLine("IcspTcpTransport.processRX: received packet with invalid encryption type - " + i3);
            return 0;
          }

          dataInputStream = new DataInputStream(new MemoryStream(arrayOfByte));

          protocol = dataInputStream.readUnsignedByte();

          if(protocol != 2)
          {
            Console.WriteLine("IcspTcpTransport.processRX: bad protocol in decrypted data = " + protocol);
            return 0;
          }

          size = dataInputStream.readUnsignedShort();

          checksum = (byte)protocol;
          checksum = (byte)(checksum + (size >> 8));
          checksum = (byte)(checksum + (size & 0xFF));

          for(index = 0; index < size; index++)
            checksum = (byte)(checksum + arrayOfByte[index + 3]);

          if(checksum != arrayOfByte[size + 3])
          {
            Console.WriteLine("IcspTcpTransport.processRX: decrypted checksum error packet, caluclated=" + checksum + " data=" + arrayOfByte[size + 3]);
            return 0;
          }

          Array.Copy(arrayOfByte, 3, buffer, 0, size);
        }
        catch(IOException ex)
        {
          Console.WriteLine("IcspTcpTransport.processRX: parse error in encrypted data", ex);

          return 0;
        }
      }

      try
      {
        ICSPMsgFlag flags = (ICSPMsgFlag)dataInputStream.readUnsignedShort();

        desSystem = dataInputStream.readUnsignedShort();
        desDevice = dataInputStream.readUnsignedShort();
        desPort = dataInputStream.readUnsignedShort();

        srcSystem = dataInputStream.readUnsignedShort();
        srcDevice = dataInputStream.readUnsignedShort();
        srcPort = dataInputStream.readUnsignedShort();

        dataInputStream.readUnsignedByte(); // Hop

        id = dataInputStream.readUnsignedShort();

        // DynamicDeviceAddressResponse = 0x0503;
        int command = dataInputStream.readUnsignedShort();

        if(icspConnectionNode == null)
          icspConnectionNode = getConnectionNode(desDevice);

        nIsNewbee = ((flags & ICSPMsgFlag.Newbee) != 0) ? true : false;

        switch(command)
        {
          case 0x0731: // NX-1200 -> Authentication Challenge request for SHA256?
          {
            return 0;
          }
          case 0x0703: // MD5 Second part ... (Protocol switch ...)
          {
            var str = BitConverter.ToString(buffer).Replace("-", " ");

            //          | ---------------------------------------------------------------------------------------------
            //          | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
            //          | ---------------------------------------------------------------------------------------------
            //          | 02 08 | 00 01 7D 01 00 01 | 00 01 00 00 00 01 | 0F | 00 03 | 07 03 | 00 00 00 00 | C1

            // send ... Protokoll 4!
            // 04 00 69 | 00 01 7d 01 01 04 00 29 48 23 1a b1 3c 4a a3 3f d7 cf 0a 5e 64 0a 73 a7 73 2b b0 b7 87 32 a2 95 24 16 7d c9 d4 62 c9 ee dc 4f 90 07 06 dd dc 6d 35 61 d0 b6 23 2c f5 d3 75 d7 00 cb 7b a1 12 04 69 92 22 5f ba 58 c4 14 a3 14 2d 90 a0 f4 c6 2d 24 eb fc b7 ec ab 7f 5c 8f e5 db 35 1b 28 0a da a6 41 cf cd a4 94 43 c7 b1 d0
            
            if(icspConnectionNode == null)
            {
              Console.WriteLine("IcspTcpTransport.processRX: Authentication result for unknown device -" + desDevice);

              size = 0;

              break;
            }

            var lState = dataInputStream.readUnsignedShort();

            /*
            public static final int AuthenticatedBlinkOff = 3;  
            public static final int AuthenticatedBlinkOn  = 4;  
            public static final int EncryptedBlinkOff     = 5;  
            public static final int EncryptedBlinkOn      = 6;  
            public static final int AuthenticationFailed  = 7;  
            public static final int AccessNotAllowed      = 8;
            */

            if((lState & 0x8000) != 0)
            {
              icspConnectionNode.setAuthenticationState(4);
            }
            else if((lState & 0x1) != 0)
            {
              icspConnectionNode.setAuthenticationState(1);

              if((lState & 0x6) != 0)
              {
                icspConnectionNode.setAuthenticationState(2);
                icspConnectionNode.setEncryptionMode(lState & 0x6);

                switch(lState & 0x6)
                {
                  case 4:
                  {
                    if(this.m_md != null)
                    {
                      this.m_md.reset();
                      this.m_md.update(icspConnectionNode.getAuthenticationChallenge());

                      try
                      {
                        var lUsername = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("administrator"));
                        var lPassword = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("password"));

                        byte[] arrayOfByte1 = System.Text.Encoding.UTF8.GetBytes(lUsername);
                        byte[] arrayOfByte2 = System.Text.Encoding.UTF8.GetBytes(lPassword);

                        this.m_md.update(arrayOfByte1);
                        this.m_md.update(arrayOfByte2);
                      }
                      catch(IOException ex)
                      {
                        Console.WriteLine("IcspTcpTransport.processRX: failed to build encryption key!", ex);
                      }

                      byte[] lHash = this.m_md.digest();

                      ARC4 aRC4 = new ARC4(lHash, 16);

                      icspConnectionNode.setCrypto(aRC4);
                    }

                    break;
                  }
                }
              }

              icspConnectionNode.handleTransportEvent(2, icspConnectionNode.getAuthenticationState());
            }

            size = 0;

            break;
          }
        }
      }
      catch(IOException ex)
      {
        Console.WriteLine("IcspTcpTransport.processRX: parse error", ex);

        return 0;
      }

      if(nIsNewbee)
        sendAcknowledge(srcSystem, srcDevice, srcPort, desSystem, desDevice, desPort, id);

      return size;
    }

    private void sendAuthenticationResponse(int m, int n, int i1, int i, int j, int k, int v, byte[] arrayOfByte2)
    {
    }

    private void sendAcknowledge(int m, int n, int i1, int i, int j, int k, int i2)
    {
    }

    private IcspConnectionNode getConnectionNode(int i1)
    {
      return new IcspConnectionNode();
    }

    private void OnClientDisconnected()
    {
      Logger.LogInfo("Client Disconnected: {0:l}", RemoteEndPoint);

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

  public class MessageDigest
  {
    HashAlgorithm mHashAlgorithm;

    byte[] mBuffer = new byte[0];

    public MessageDigest()
    {
      mHashAlgorithm = MD5.Create("MD5");
    }

    internal byte[] digest()
    {
      return mHashAlgorithm.ComputeHash(mBuffer);
    }

    internal void reset()
    {
      mBuffer = new byte[0];

      mHashAlgorithm = HashAlgorithm.Create("MD5");
    }

    internal void update(byte[] buffer)
    {
      mBuffer = mBuffer.Concat(buffer).ToArray();
    }
  }

  internal class ARC4
  {
    private byte[] mHash;
    private int mV;

    public ARC4(byte[] hash, int v)
    {
      mHash = hash;
      mV = v;
    }

    internal void crypt(byte[] arrayOfByte, int v, int k, byte[] arrayOfByte1)
    {
    }
  }

  internal class Crypto : ARC4
  {
    public Crypto(byte[] arrayOfByte, int v) : base(arrayOfByte, v)
    {
    }
  }

  internal class IcspConnectionNode
  {
    internal byte[] getAuthenticationChallenge()
    {
      return new byte[] { };
    }

    internal string getAuthenticationPassword()
    {
      return "";
    }

    internal int getAuthenticationState()
    {
      return 0;
    }

    internal string getAuthenticationUsername()
    {
      return "";
    }

    internal Crypto getCrypto()
    {
      return null;
    }

    internal void handleTransportEvent(int v, object p)
    {
    }

    internal void setAuthenticationChallenge(byte[] arrayOfByte1)
    {
    }

    internal void setAuthenticationState(int v)
    {
    }

    internal void setCrypto(ARC4 aRC4)
    {
    }

    internal void setEncryptionMode(int v)
    {
    }
  }

  internal class DataInputStream
  {
    private MemoryStream mMemoryStream;

    public DataInputStream(MemoryStream memoryStream)
    {
      mMemoryStream = memoryStream;
    }

    internal void read(byte[] arrayOfByte1)
    {
    }

    internal byte readByte()
    {
      return (byte)mMemoryStream.ReadByte();
    }

    internal int readUnsignedByte()
    {
      return mMemoryStream.ReadByte();
    }

    internal int readUnsignedShort()
    {
      var buffer = new byte[2];

      mMemoryStream.Read(buffer, 0, 2);

      return ((buffer[0] << 8) | buffer[1]);
    }
  }
}