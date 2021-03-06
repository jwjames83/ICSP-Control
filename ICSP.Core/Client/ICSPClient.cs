using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Constants;
using ICSP.Core.Cryptography;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;
using ICSP.Core.Manager.DeviceManager;

using Serilog.Events;

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

              ProcessData(lBytes[0], lDataSize, lBytes.GetRange(3, lSize - 3).ToArray());

              var lMsg = Factory.FromData(lBytes.GetRange(0, lSize).ToArray());

              lBytes.RemoveRange(0, lSize);

              switch(lMsg)
              {
                case ICSPMsg m: OnDataReceived(new ICSPMsgDataEventArgs(m)); break;
                case ICSPEncryptedMsg m:
                {
                  // TODO ...

                  Logger.LogVerbose("{0} Bytes", m.RawData.Length);
                  Logger.LogVerbose("Data 0x: {0:l}", BitConverter.ToString(m.RawData).Replace("-", " "));

                  if(Logger.LogLevel <= LogEventLevel.Verbose)
                    m.WriteLogVerbose();

                  break;
                }
              }

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

    // In work ...
    private IcspConnection mConnection = new IcspConnection();

    private int ProcessData(int protocol, int size, byte[] buffer)
    {
      int desSystem;
      int desDevice;
      int desPort;

      int srcSystem;
      int srcDevice;
      int srcPort;

      int id;

      bool nIsNewbee;

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

      var lStream = new DataInputStream(new MemoryStream(buffer, 0, size));

      if(protocol == 4)
      {
        try
        {
          byte[] encryptedData;

          lStream.ReadUShort();
          lStream.ReadUShort();

          var lEncryptionType = lStream.ReadByte();

          var cStr = BitConverter.ToString(buffer).Replace("-", " ");

          Console.WriteLine(cStr);

          // ---------------------------------------------------------------------------------------------
          // P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data | CS
          // ---------------------------------------------------------------------------------------------

          // ---------------------------------------------------------------------------------------------
          //  srcSys | srcPort | ET | Ln | Salt        | desSys  | desDev | Encrypted Data | CS
          // ---------------------------------------------------------------------------------------------
          // 00 01   | 00 00   | 02 | 08 | 5B CF 08 88 | 00 01   | 7D 01  | 04 D3 D5 45 E0 D2 64 97 72 9C 3C A3 82 22 E5 49 C9 20 67 5A E7 6E 29 9F B5 96 61 E2 74 48 F3 | D6

          if(lEncryptionType == 2) // RC4
          {
            var len = lStream.ReadByte();

            var lDataSize = size - 6 - len;

            var salt = new byte[4];

            lStream.Read(salt);

            srcDevice = lStream.ReadUShort();
            desDevice = lStream.ReadUShort();

            if(mConnection != null)
            {
              if(mConnection.CryptoProvider is RC4 cryptoProvider)
              {
                encryptedData = new byte[lDataSize];

                lStream.Read(encryptedData);

                // ==============================================================================================================================

                var lTestChallenge = new byte[] { 0x51, 0xa4, 0x0e, 0x5f, };

                using var lHashAlgorithm = HashAlgorithm.Create("MD5");

                if(lHashAlgorithm == null)
                  throw new Exception("ICSP: Failed to build encryption key!");

                var lHash = lHashAlgorithm.ComputeHash(
                  lTestChallenge
                  .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes("administrator"))))
                  .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes("password")))).ToArray());

                var lCryptoProvider = RC4.Create(lHash);

                // ==============================================================================================================================
                // TEST-STUFF ...
                // ==============================================================================================================================

                // ---------------------------------------------------------------------------------------------
                //            SrcS SrcDev | ET | Ln | Salt        | dSys desDEv | srcPort | Encrypted Data | CS
                // ---------------------------------------------------------------------------------------------
                // 04 00 21 | 00 01 00 00 | 01 | 04 | 52 2a 17 f5 | ea b5 b4 2d 46 cb 40 2b 83 82 7a 98 07 e3 2c 8e 85 b2 e1 db 19 40 cd | 83

                var lTestSalt = new byte[] { 0x52, 0x2a, 0x17, 0xf5, };

                var lEncryptedTestData = new byte[] { 0xea, 0xb5, 0xb4, 0x2d, 0x46, 0xcb, 0x40, 0x2b, 0x83, 0x82, 0x7a, 0x98, 0x07, 0xe3, 0x2c, 0x8e, 0x85, 0xb2, 0xe1, 0xdb, 0x19, 0x40, 0xcd, };

                cStr = BitConverter.ToString(lEncryptedTestData).Replace("-", " ");

                Console.WriteLine(cStr);

                lCryptoProvider.TransformBlock(lEncryptedTestData, 0, 23, lTestSalt);

                lCryptoProvider.TransformBlockDefault(lEncryptedTestData, lTestSalt);

                // EA B5 B4 2D 46 CB 40 2B 83 82 7A 98 07 E3 2C 8E 85 B2 E1 DB 19 40 CD
                // ED 23 6D 08 33 A0 0A E5 53 18 CE FF 82 B5 5F 7C E0 9F 0C D5 CB B2 F4

                cStr = BitConverter.ToString(lEncryptedTestData).Replace("-", " ");

                Console.WriteLine(cStr);

                // ==============================================================================================================================
                // TEST-STUFF ...
                // ==============================================================================================================================

                cStr = BitConverter.ToString(encryptedData).Replace("-", " ");

                Console.WriteLine(cStr);

                // E6 3C 9A 0B 29 6B 1E 6F 93 37 C6 48 EB 39 03 FC 91 5B 18 4F 98 1C 11 50 55 6C 80 8D 40 EC 5B

                cryptoProvider.TransformBlock(encryptedData, 0, lDataSize, salt);

                cStr = BitConverter.ToString(encryptedData).Replace("-", " ");

                Console.WriteLine(cStr);

                // C5 28 54 A0 D4 0C 95 30 18 C5 94 D2 A0 BA F8 9C 7E EC 2E 66 E6 AC 95 29 3F EF BF CF 58 C5 30
              }
              else
              {
                Console.WriteLine("IcspTcpTransport.processRX: Received encrypted packet but encryption not initialized for device");
                return 0;
              }
            }
            else
            {
              return 0;
            }
          }
          else
          {
            Console.WriteLine("IcspTcpTransport.processRX: received packet with invalid encryption type - " + lEncryptionType);
            return 0;
          }

          lStream = new DataInputStream(new MemoryStream(encryptedData));

          protocol = lStream.ReadByte();

          if(protocol != 2)
          {
            Console.WriteLine("IcspTcpTransport.processRX: bad protocol in decrypted data = " + protocol);
            return 0;
          }

          size = lStream.ReadUShort();

          checksum = (byte)protocol;
          checksum = (byte)(checksum + (size >> 8));
          checksum = (byte)(checksum + (size & 0xFF));

          for(index = 0; index < size; index++)
            checksum = (byte)(checksum + encryptedData[index + 3]);

          if(checksum != encryptedData[size + 3])
          {
            Console.WriteLine("IcspTcpTransport.processRX: decrypted checksum error packet, caluclated=" + checksum + " data=" + encryptedData[size + 3]);
            return 0;
          }

          Array.Copy(encryptedData, 3, buffer, 0, size);
        }
        catch(IOException ex)
        {
          Console.WriteLine("IcspTcpTransport.processRX: parse error in encrypted data", ex);

          return 0;
        }
      }

      try
      {
        var flags = (ICSPMsgFlag)lStream.ReadUShort();

        desSystem = lStream.ReadUShort();
        desDevice = lStream.ReadUShort();
        desPort = lStream.ReadUShort();

        srcSystem = lStream.ReadUShort();
        srcDevice = lStream.ReadUShort();
        srcPort = lStream.ReadUShort();

        lStream.ReadByte(); // Hop

        id = lStream.ReadUShort();

        // DynamicDeviceAddressResponse = 0x0503;
        int command = lStream.ReadUShort();

        nIsNewbee = ((flags & ICSPMsgFlag.Newbee) != 0) ? true : false;

        switch(command)
        {
          case ConnectionManagerCmd.ChallengeRequestMD5: // MD5-Challenge request
          {
            var challengeData = new byte[4];

            challengeData[0] = lStream.ReadByte();
            challengeData[1] = lStream.ReadByte();
            challengeData[2] = lStream.ReadByte();
            challengeData[3] = lStream.ReadByte();

            mConnection.AuthenticationChallenge = challengeData;

            break;
          }
          case 0x0731: // NX-1200 -> Authentication Challenge request for SHA256?
          {
            return 0;
          }
          case ConnectionManagerCmd.ChallengeAckMD5:
          {
            // MD5 Second part

            var cStr = BitConverter.ToString(buffer).Replace("-", " ");

            //          | ---------------------------------------------------------------------------------------------
            //          | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
            //          | ---------------------------------------------------------------------------------------------
            //          | 02 08 | 00 01 7D 01 00 01 | 00 01 00 00 00 01 | 0F | 00 03 | 07 03 | 00 00 00 00 | C1

            // Fail:    | 02 08 | 00 01 7d 01 00 00 | 00 01 00 00 00 00 | 0f | 00 03 | 07 03 | 00 00 | 00 00 | bf
            // OK:      | 02 08 | 00 01 7d 01 00 00 | 00 01 00 00 00 00 | 0f | 00 09 | 07 03 | 00 03 | ff ff | c6

            // send ... Protokoll 4!
            // 04 00 69 | 00 01 7d 01 01 04 00 29 48 23 1a b1 3c 4a a3 3f d7 cf 0a 5e 64 0a 73 a7 73 2b b0 b7 87 32 a2 95 24 16 7d c9 d4 62 c9 ee dc 4f 90 07 06 dd dc 6d 35 61 d0 b6 23 2c f5 d3 75 d7 00 cb 7b a1 12 04 69 92 22 5f ba 58 c4 14 a3 14 2d 90 a0 f4 c6 2d 24 eb fc b7 ec ab 7f 5c 8f e5 db 35 1b 28 0a da a6 41 cf cd a4 94 43 c7 b1 d0

            if(mConnection == null)
            {
              Console.WriteLine("IcspTcpTransport.processRX: Authentication result for unknown device -" + desDevice);
              size = 0;
              break;
            }

            var lState = (AuthenticationState)lStream.ReadUShort();

            /*
            AuthenticatedBlinkOff = 3;
            AuthenticatedBlinkOn  = 4;  
            EncryptedBlinkOff     = 5;  
            EncryptedBlinkOn      = 6;  
            AuthenticationFailed  = 7;  
            AccessNotAllowed      = 8;
            */

            if((lState & (AuthenticationState)0x8000) != 0)
            {
              // 1000 0000 0000 0000
              mConnection.AuthenticationState = 4;
            }
            else if((lState & AuthenticationState.Authenticated) != 0)
            {
              // Success
              // 0000 0000 0000 0001
              mConnection.AuthenticationState = 1;

              // 0000 0000 0000 0110 (6)
              if((lState & AuthenticationState.DoEncrypt | AuthenticationState.EncryptionModeRC4) != 0)
              {
                mConnection.AuthenticationState = 2;

                mConnection.EncryptionMode = (int)(lState & AuthenticationState.DoEncrypt | AuthenticationState.EncryptionModeRC4);

                switch((int)(lState & AuthenticationState.DoEncrypt | AuthenticationState.EncryptionModeRC4))
                {
                  case 4:
                  {
                    using var lHashAlgorithm = HashAlgorithm.Create("MD5");

                    if(lHashAlgorithm == null)
                      throw new Exception("ICSP: Failed to build encryption key!");

                    var lHash = lHashAlgorithm.ComputeHash(
                      mConnection.AuthenticationChallenge
                      .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes("administrator"))))
                      .Concat(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes("password")))).ToArray());

                    mConnection.CryptoProvider = RC4.Create(lHash);

                    break;
                  }
                }
              }

              mConnection.HandleTransportEvent(2, mConnection.AuthenticationState);
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
        SendAcknowledge(srcSystem, srcDevice, srcPort, desSystem, desDevice, desPort, id);

      return size;
    }

    private byte[] mSendBuffer = new byte[100];

    private int GetTimeoutFlags()
    {
      return 512;
    }

    protected void SendAuthenticationResponse(int desSystem, int desDevice, int desPort, int srcSystem, int srcDevice, int srcPort, int paramInt7, byte[] hash)
    {
      byte[] arrayOfByte = new byte[37];

      arrayOfByte[00] = 0;
      arrayOfByte[01] = 0;
      arrayOfByte[02] = (byte)(desSystem >> 8 & 0xFF);
      arrayOfByte[03] = (byte)(desSystem & 0xFF);
      arrayOfByte[04] = (byte)(desDevice >> 8 & 0xFF);
      arrayOfByte[05] = (byte)(desDevice & 0xFF);
      arrayOfByte[06] = (byte)(desPort >> 8 & 0xFF);
      arrayOfByte[07] = (byte)(desPort & 0xFF);
      arrayOfByte[08] = (byte)(srcSystem >> 8 & 0xFF);
      arrayOfByte[09] = (byte)(srcSystem & 0xFF);
      arrayOfByte[10] = (byte)(srcDevice >> 8 & 0xFF);
      arrayOfByte[11] = (byte)(srcDevice & 0xFF);
      arrayOfByte[12] = (byte)(srcPort >> 8 & 0xFF);
      arrayOfByte[13] = (byte)(srcPort & 0xFF);
      arrayOfByte[14] = 16;
      arrayOfByte[15] = 15;
      arrayOfByte[16] = 0; // -1
      arrayOfByte[17] = 7;
      arrayOfByte[18] = 2;
      arrayOfByte[19] = (byte)(paramInt7 >> 8 & 0xFF);
      arrayOfByte[20] = (byte)(paramInt7 & 0xFF);

      for(byte b = 0; b < 16; b++)
        arrayOfByte[b + 21] = hash[b];

      var cStr = BitConverter.ToString(arrayOfByte).Replace("-", " ");
      Console.WriteLine(cStr);

      // 00 00 00 01 00 00 00 01 00 01 7D 01 00 01 10 0F 00 | 07 02 | 00 02 | C8 FF 27 66 71 D2 AB 8F DC 97 86 28 21 51 48 CD

      SendInternal(arrayOfByte, 37);
    }

    private bool SendInternal(byte[] data, int size)
    {
      if(data == null)
      {
        Console.WriteLine("IcspTcpTransport.send(): msg is null");
        return false;
      }

      if(size > 2048)
      {
        Console.WriteLine("IcspTcpTransport.send(): packet too large - " + size + " bytes.");
        return false;
      }

      mSendBuffer = new byte[100];

      int i = 17;
      int j = i;

      this.mSendBuffer[j++] = 2;
      this.mSendBuffer[j++] = (byte)(size >> 8 & 0xFF);
      this.mSendBuffer[j++] = (byte)(size & 0xFF);

      Array.Copy(data, 0, this.mSendBuffer, j, size);

      j += size;

      int k = GetTimeoutFlags();

      this.mSendBuffer[i + 3] = (byte)(this.mSendBuffer[i + 3] & 0xFF | k >> 8);
      this.mSendBuffer[i + 4] = (byte)(this.mSendBuffer[i + 4] & 0xFF | k & 0xFF);
      this.mSendBuffer[i + 17] = 15;

      byte b = 0;
      int m;

      for(m = i; m < i + size + 3; m++)
        b = (byte)(b + this.mSendBuffer[m]);

      this.mSendBuffer[i + size + 3] = b;

      m = size + 4;

      int n = ((data[10] & 0xFF) << 8) + (data[11] & 0xFF);

      if(mConnection == null)
      {
        Console.WriteLine("IcspTcpTransport.send: message send from unknown device - " + n);
        return false;
      }

      if(mConnection.EncryptionMode == 4)
      {
        if(mConnection.CryptoProvider is not null and RC4)
        {
          var m_rand = new Random();

          int i1 = (m_rand.Next() << 16) + m_rand.Next();

          byte[] arrayOfByte = new byte[4];

          arrayOfByte[0] = (byte)(i1 >> 24 & 0xFF);
          arrayOfByte[1] = (byte)(i1 >> 16 & 0xFF);
          arrayOfByte[2] = (byte)(i1 >> 8 & 0xFF);
          arrayOfByte[3] = (byte)(i1 & 0xFF);

          mConnection.CryptoProvider.TransformBlock(this.mSendBuffer, i, m, arrayOfByte);

          i = j = 0;

          this.mSendBuffer[j++] = 4;
          this.mSendBuffer[j++] = (byte)(m + 18 - 4 >> 8 & 0xFF);
          this.mSendBuffer[j++] = (byte)(m + 18 - 4 & 0xFF);
          this.mSendBuffer[j++] = data[8];
          this.mSendBuffer[j++] = data[9];
          this.mSendBuffer[j++] = data[10];
          this.mSendBuffer[j++] = data[11];
          this.mSendBuffer[j++] = 2;
          this.mSendBuffer[j++] = 8;

          byte b1;

          for(b1 = 0; b1 < 4; b1++)
            this.mSendBuffer[j++] = arrayOfByte[b1];

          this.mSendBuffer[j++] = data[2];
          this.mSendBuffer[j++] = data[3];
          this.mSendBuffer[j++] = data[4];
          this.mSendBuffer[j++] = data[5];

          b = 0;

          for(b1 = 0; b1 < m + 18 - 1; b1++)
            b = (byte)(b + this.mSendBuffer[b1]);

          this.mSendBuffer[m + 18 - 1] = b;

          m += 18;
        }
        else
        {
          Console.WriteLine("IcspTcpTransport.send: encryption algorithm not initialized for device " + n);
        }
      }

      /*
      if(this.m_out != null)
      {
        try
        {
          this.m_out.write(this.m_txBuffer, i, m);
          this.m_out.flush();
          return true;
        }
        catch(IOException iOException)
        {
          Console.WriteLine("IcspTcpTransport: Socket write exception");
        }
      }
      else
      {
        Console.WriteLine("IcspTcpTransport: Bad m_socket. msg not sent");
      }
      */

      var cStr = BitConverter.ToString(this.mSendBuffer).Replace("-", " ");

      Console.WriteLine(cStr);

      // ---------------------------------------------------------------------------------------------------------------------------------
      // Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
      // ---------------------------------------------------------------------------------------------------------------------------------

      // 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
      // 02 00 25 
      // 02 00 | 00 01 00 00 00 01 | 00 01 7D 01 00 01 | 0F | 0F 00 | 07 02 | 00 02 | C8 FF 27 66 71 D2 AB 8F DC 97 86 28 21 51 48 CD | 4D

      return false;
    }

    private void SendAcknowledge(int desSystem, int desDevice, int desPort, int srcSystem, int srcDevice, int srcPort, int id)
    {
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

  internal class IcspConnection
  {
    private int mSystem;
    private int mDevice;
    
    private List<DeviceInfoData> mDeviceInfoList = new List<DeviceInfoData>();

    public IcspConnection()
    {
      mDeviceInfoList.Add(new DeviceInfoData(10000, null));
    }

    internal ICrypto CryptoProvider { get; set; }

    internal int ConnectionState { get; set; }

    internal int AuthenticationState { get; set; }

    internal int EncryptionMode { get; set; }

    internal byte[] AuthenticationChallenge { get; set; }

    internal void HandleTransportEvent(int connectionState, int authenticationState)
    {
      if(connectionState == 1)
      {
        switch(authenticationState)
        {
          case 0:
          {
            if(mDevice == 0)
            {
              ConnectionState = 2;

              // Generate dynamic device address request ...
              // generateRequestDynamicDeviceAddress(this, 38, m_stack.nextMessageId(), m_system, 0, 1, m_system, m_device, 1, m_device, m_address);
            }
            else
            {
              ConnectionState = 3;
              SendDeviceInfoMessages(0);
            }

            return;
          }
          case 3:
          case 5:
          {
            ConnectionState = 1;
            AuthenticationChallenge = null;
            AuthenticationState = 0;
            EncryptionMode = 0;

            if(mDevice >= 32001)
            {
              ConnectionState = 2;
              mDevice = 0;
            }

            return;
          }
          case 1:
          case 2:
          case 4:
          {
            mSystem = 0;
            return;
          }
        }

        Console.WriteLine("IcspDevice.handleTransportEvent: invalid connect event type = " + authenticationState);
      }
      else if(connectionState == 2)
      {
        if(authenticationState == 1 || authenticationState == 2)
          SendDeviceInfoMessages(-1);
      }
      else
      {
        // m_log.error("IcspDevice.processTransportEvent: invalid event type = " + connectionState);
      }
    }

    protected void SendDeviceInfoMessages(int deviceIndex)
    {
      int deviceListCount = mDeviceInfoList.Count;

      if(deviceIndex == 0)
        deviceListCount = 1;

      if(deviceIndex == -1)
        deviceIndex = 0;

      //if(m_log.getLogLevel() >= 4)
      //  m_log.debug("sendDeviceInfoMessages(" + deviceIndex + "," + deviceListCount + ") for device " + m_device);

      for(int index = deviceIndex; index < deviceListCount; index++)
      {
        DeviceInfoData icspDeviceInfo = mDeviceInfoList[index];

        if(icspDeviceInfo != null)
        {
          Console.WriteLine(icspDeviceInfo);

          // var req = MsgCmdDeviceInfo.CreateRequest(new AmxDevice(0,1,0), new AmxDevice(0, 1, 0), icspDeviceInfo);

          // m_stack.generateDeviceInfo((j == 0) ? 18 : 0, m_stack.nextMessageId(), m_system, 0, 1, m_system, m_device, 1, 
          // m_system, m_device, icspDeviceInfo.flag, icspDeviceInfo.objectId, icspDeviceInfo.parentId, icspDeviceInfo.mfgId, icspDeviceInfo.deviceId,
          // icspDeviceInfo.serialNumber, icspDeviceInfo.firmwareId, icspDeviceInfo.version, icspDeviceInfo.deviceName, icspDeviceInfo.mfgName, m_address);
        }
      }
    }
  }

  internal class DataInputStream
  {
    private MemoryStream mMemoryStream;

    public DataInputStream(MemoryStream memoryStream)
    {
      mMemoryStream = memoryStream;
    }

    internal void Read(byte[] buffer)
    {
      mMemoryStream.Read(buffer, 0, buffer.Length);
    }

    internal byte ReadByte()
    {
      return (byte)mMemoryStream.ReadByte();
    }

    internal int ReadUShort()
    {
      var buffer = new byte[2];

      mMemoryStream.Read(buffer, 0, 2);

      return ((buffer[0] << 8) | buffer[1]);
    }
  }
}