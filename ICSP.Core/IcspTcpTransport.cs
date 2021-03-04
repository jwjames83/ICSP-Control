using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSP.Core
{
  public class IcspTcpTransport : IcspTransportBase
  {
    private Socket m_socket = null;

    private InputStream m_in = null;

    private OutputStream m_out = null;

    private ServerSocket m_acceptSocket = null;

    private byte[] m_txBuffer = null;

    private int m_currentTimeout = -1;

    private byte[] m_rxBuffer = null;

    private int m_offset;

    private int m_length;

    public IcspTcpTransport(Logger paramLogger, MxProperty paramMxProperty) :
        base(paramLogger, paramMxProperty)
    {
      this.m_txBuffer = new byte[2087];
      this.m_rxBuffer = new byte[4174];
      this.m_offset = 0;
      this.m_length = 0;
      String str = this.m_props.getProperty("icsp.transport.mode", "auto");
      if(str.equalsIgnoreCase("auto"))
      {
        this.m_connectMode = 1;
      }
      else if(str.equalsIgnoreCase("url"))
      {
        this.m_connectMode = 2;
      }
      else if(str.equalsIgnoreCase("listen"))
      {
        this.m_connectMode = 3;
      }
      else
      {
        this.m_connectMode = 0;
      }
      if(this.m_masterUrl.length() == 0 && this.m_connectMode == 2)
        this.m_connectMode = 0;
    }

    public bool connect()
    {
      if(this.m_connectMode == 1)
        return autoConnect();
      if(this.m_connectMode == 2)
        return urlConnect();
      if(this.m_connectMode == 3)
        return passiveConnect();
      if(this.m_log.getLogLevel() >= 3)
        this.m_log.info("IcspTcpTransport::Connect() invalid mode " + this.m_connectMode);
      return false;
    }

    private bool autoConnect()
    {
      DatagramPacket datagramPacket;
      DatagramSocket datagramSocket = null;

      bool bool = false;
      if(this.m_log.getLogLevel() >= 2)
        this.m_log.warn("IcspTcpTransport.autoConnect()");
      try
      {
        datagramSocket = new DatagramSocket(1319);
        datagramSocket.setSoTimeout(1000);
        datagramPacket = new DatagramPacket(new byte[2000], 2000);
      }
      catch(SocketException socketException)
      {
        socketException.printStackTrace();
        if(datagramSocket != null)
          datagramSocket.close();
        return bool;
      }
      this.m_connectStatus = 1;
      fireConnectEvent();
      if(this.m_log.getLogLevel() >= 3)
        this.m_log.info("IcspTcpTransport: starting autoDetect");
      while(this.m_connectMode == 1 && this.m_connectStatus != 0 && datagramSocket != null)
      {
        bool bool1 = false;
        try
        {
          datagramSocket.receive(datagramPacket);
          if(datagramPacket.getLength() > 0)
          {
            byte[] arrayOfByte = datagramPacket.getData();
            if(arrayOfByte[0] == 2)
            {
              int i = (arrayOfByte[1] & 0xFF) << 8 | arrayOfByte[2] & 0xFF;
              byte b = 0;
              int j;
              for(j = 0; j < i + 3; j++)
                b = (byte)(b + arrayOfByte[j]);
              if(b == arrayOfByte[i + 3])
              {
                j = (arrayOfByte[11] & 0xFF) << 8 | arrayOfByte[12] & 0xFF;
                int k = (arrayOfByte[13] & 0xFF) << 8 | arrayOfByte[14] & 0xFF;
                int m = (arrayOfByte[20] & 0xFF) << 8 | arrayOfByte[21] & 0xFF;
                if(m == 1282 && (this.m_system == 0 || this.m_system == j) && k == 0)
                {
                  if(this.m_log.getLogLevel() >= 3)
                    this.m_log.info("autoDetect found blink");
                  String str = datagramPacket.getAddress().toString().substring(1);
                  setMasterURL(str);
                  bool = urlConnect();
                }
              }
            }
          }
        }
        catch(SocketTimeoutException socketTimeoutException)
        {

        }
        catch(IOException iOException)
        {
          iOException.printStackTrace();
          bool1 = true;
        }
        catch(Exception exception)
        {
          exception.printStackTrace();
          bool1 = true;
        }
        if(bool1)
        {
          datagramSocket.close();
          datagramSocket = null;
          try
          {
            datagramSocket = new DatagramSocket(1319);
            datagramSocket.setSoTimeout(1000);
          }
          catch(SocketException socketException)
          {
            socketException.printStackTrace();
            if(datagramSocket != null)
            {
              datagramSocket.close();
              datagramSocket = null;
            }
          }
        }
      }
      if(datagramSocket != null)
        datagramSocket.close();
      return bool;
    }

    private bool urlConnect()
    {
      if(this.m_log.getLogLevel() >= 3)
        this.m_log.info("IcspTcpTransport::urlConnect(" + this.m_masterUrl + ")");
      this.m_connectStatus = 4;
      fireConnectEvent();
      if(this.m_socket != null)
      {
        if(this.m_log.getLogLevel() >= 2)
          this.m_log.warn("IcspTcpTransport: connect call with open socket");
        try
        {
          this.m_socket.close();
        }
        catch(IOException iOException) { }
        this.m_socket = null;
      }
      try
      {
        this.m_socket = new Socket(this.m_masterUrl, this.m_masterPort);
        if(this.m_socket.isConnected())
        {
          this.m_socket.setSoLinger(false, 0);
          this.m_socket.setTcpNoDelay(true);
          if(this.m_log.getLogLevel() >= 3)
            this.m_log.info("Master accepted connection.");
          this.m_in = this.m_socket.getInputStream();
          this.m_out = this.m_socket.getOutputStream();
          this.m_connectStatus = 0;
          fireConnectEvent();
          return true;
        }
      }
      catch(UnknownHostException unknownHostException)
      {
        unknownHostException.printStackTrace();
      }
      catch(IOException iOException) { }
      if(this.m_log.getLogLevel() >= 3)
        this.m_log.info("IcspTcpTransport.urlConnect: connect failed closing socket");
      try
      {
        if(this.m_socket != null)
          this.m_socket.close();
      }
      catch(IOException iOException)
      {
        this.m_log.error("Unexpected I/O exception in socket.close()");
      }
      this.m_socket = null;
      this.m_connectMode = 2;
      this.m_connectStatus = 3;
      if(this.m_log.getLogLevel() >= 3)
        this.m_log.info("IcspTcpTransport.urlConnect: firing disconnected event");
      fireConnectEvent();
      return false;
    }

    private bool passiveConnect()
    {
      if(this.m_log.getLogLevel() >= 3)
        this.m_log.info("IcspTcpTransport.passiveConnect()");
      if(this.m_socket != null)
      {
        if(this.m_log.getLogLevel() >= 2)
          this.m_log.warn("IcspTcpTransport: connect call with open socket");
        try
        {
          this.m_socket.close();
        }
        catch(IOException iOException) { }
        this.m_socket = null;
      }
      try
      {
        this.m_connectStatus = 2;
        fireConnectEvent();
        this.m_acceptSocket = new ServerSocket();
        this.m_acceptSocket.setReuseAddress(true);
        this.m_acceptSocket.bind(new InetSocketAddress(1319));
        this.m_socket = this.m_acceptSocket.accept();
        this.m_acceptSocket.close();
        this.m_acceptSocket = null;
        this.m_socket.setSoLinger(false, 0);
        this.m_socket.setTcpNoDelay(true);
        if(this.m_log.getLogLevel() >= 3)
          this.m_log.info("Master has connected.");
        this.m_in = this.m_socket.getInputStream();
        this.m_out = this.m_socket.getOutputStream();
        this.m_connectStatus = 0;
        fireConnectEvent();
        return true;
      }
      catch(IOException iOException)
      {
        iOException.printStackTrace();
        if(this.m_socket != null)
        {
          try
          {
            this.m_socket.close();
          }
          catch(IOException iOException1) { }
          this.m_socket = null;
        }
        if(this.m_acceptSocket != null)
        {
          try
          {
            this.m_acceptSocket.close();
          }
          catch(IOException iOException1) { }
          this.m_acceptSocket = null;
        }
        return false;
      }
    }

    public void deviceChanged() { }

    public bool disconnect()
    {
      if(this.m_log.getLogLevel() >= 4)
        this.m_log.debug("IcspTcpTransport.disconnect()");
      if(this.m_socket != null)
      {
        this.m_log.info("IcspTcpTransport.diconnect: closing socket");
        try
        {
          this.m_socket.close();
        }
        catch(Exception exception)
        {
          this.m_log.error("IcspTcpTransport.disconnect close failed - " + exception.getMessage());
        }
        this.m_socket = null;
      }
      if(this.m_connectStatus != 3)
      {
        this.m_connectStatus = 3;
        fireConnectEvent();
      }
      return true;
    }

    public int receive(byte[] paramArrayOfbyte, int paramInt1, int paramInt2)
    {
      int i = 0;
      int j = 0;
      byte b = 0;
      byte b1 = 0;
      if(paramArrayOfbyte == null)
      {
        this.m_log.error("IcspTcpTransport.receive - received NULL buffer");
        return -1;
      }
      if(this.m_connectStatus != 0 || this.m_socket == null)
      {
        this.m_log.error("IcspTcpTransport.receive - not yet connected");
        return -1;
      }
      if(paramInt2 != this.m_currentTimeout)
        try
        {
          this.m_socket.setSoTimeout(paramInt2);
          this.m_currentTimeout = paramInt2;
        }
        catch(SocketException socketException)
        {
          this.m_log.error("IcspTcpTransport.receive: failed to set socket timeout");
        }
      while(true)
      {
        if(this.m_offset >= this.m_length)
        {
          this.m_offset = 0;
          try
          {
            this.m_length = this.m_in.read(this.m_rxBuffer);
          }
          catch(InterruptedIOException interruptedIOException)
          {
            this.m_length = 0;
            return 0;
          }
          catch(IOException iOException)
          {
            this.m_length = 0;
            return -2;
          }
        }
        if(this.m_length < 0)
        {
          this.m_length = 0;
          return -2;
        }
        while(this.m_offset < this.m_length)
        {
          byte b2;
          int k;
          int m;
          switch(b1)
          {
            case false:
              i = 0;
              b = this.m_rxBuffer[this.m_offset++];
              if(b == 2 || b == 4)
                b1 = 1;
              continue;
            case true:
              b2 = this.m_rxBuffer[this.m_offset++];
              j = b2 << 8;
              b1 = 2;
              continue;
            case true:
              b2 = this.m_rxBuffer[this.m_offset++];
              j |= b2 & 0xFF;
              j++;
              b1 = 3;
              if(j > 2048)
              {
                this.m_log.error("IcspTcpTransport.receive: msgLen = " + j + ">" + ');
  
                b1 = 0;
                this.m_offset = 0;
                this.m_length = 0;
                return -2;
              }
              continue;
            case true:
              if(j >= paramInt1)
              {
                if(this.m_log.getLogLevel() >= 3)
                  this.m_log.info("-1 <= IcspTcpTransport.receive()");
                this.m_log.error("getMessage: buffer too small, len=" + paramInt1);
                b1 = 0;
                this.m_offset = 0;
                this.m_length = 0;
                return -2;
              }
              k = this.m_length - this.m_offset;
              m = j - i;
              if(k >= m)
              {
                System.arraycopy(this.m_rxBuffer, this.m_offset, paramArrayOfbyte, i, m);
                this.m_offset += m;
                b1 = 0;
                int n = processRX(b, j - 1, paramArrayOfbyte);
                if(n > 0)
                  return n;
                continue;
              }
              System.arraycopy(this.m_rxBuffer, this.m_offset, paramArrayOfbyte, i, k);
              i += k;
              this.m_offset += k;
              continue;
          }
          this.m_log.error("IcspTcpTransport.receive bad state");
          b1 = 0;
        }
      }
    }

    private int processRX(int paramInt1, int paramInt2, byte[] paramArrayOfbyte)
    {
      int i;
      int j;
      int k;
      int m;
      int n;
      int i1;
      int i2;

      bool flag;
      IcspConnectionNode icspConnectionNode = null;
      if(paramArrayOfbyte == null)
      {
        this.m_log.error("IcspTcpTransport::processRX - bad buffer/packet");
        return 0;
      }
      if(paramInt1 != 2 && paramInt1 != 4)
      {
        this.m_log.error("IcspTcpTransport::processRX Protocol = " + paramInt1);
        return 0;
      }
      byte b1 = (byte)paramInt1;
      b1 = (byte)(b1 + (paramInt2 >> 8));
      b1 = (byte)(b1 + (paramInt2 & 0xFF));
      byte b;
      for(b = 0; b < paramInt2; b++)
        b1 = (byte)(b1 + paramArrayOfbyte[b]);
      if(b1 != paramArrayOfbyte[paramInt2])
      {
        this.m_log.error("IcspTcpTransport::processRX checksum error packet");
        return 0;
      }
      DataInputStream dataInputStream = new DataInputStream(new ByteArrayInputStream(paramArrayOfbyte, 0, paramInt2));
      if(paramInt1 == 4)
        try
        {
          byte[] arrayOfByte;
          dataInputStream.readUnsignedShort();
          dataInputStream.readUnsignedShort();
          int i3 = dataInputStream.readUnsignedByte();
          if(i3 == 2)
          {
            i = dataInputStream.readUnsignedByte();
            k = paramInt2 - 6 - i;
            byte[] arrayOfByte1 = new byte[4];
            dataInputStream.read(arrayOfByte1);
            n = dataInputStream.readUnsignedShort();
            i1 = dataInputStream.readUnsignedShort();
            icspConnectionNode = getConnectionNode(i1);
            if(icspConnectionNode != null)
            {
              Crypto crypto = icspConnectionNode.getCrypto();
              if(crypto != null && crypto instanceof ARC4) {
                ARC4 aRC4 = (ARC4)crypto;
                arrayOfByte = new byte[k];
                dataInputStream.read(arrayOfByte);
                aRC4.crypt(arrayOfByte, 0, k, arrayOfByte1);
              } else
              {
                this.m_log.error("IcspTcpTransport.processRX: Received encrypted packet but encryption not initialized for device");
                return 0;
              }
            }
            else
            {
              this.m_log.error("IcspTcpTransport.processRX: Received encrypted packet for unknown device");
              return 0;
            }
          }
          else
          {
            this.m_log.error("IcspTcpTransport.processRX: received packet with invalid encryption type - " + i3);
            return 0;
          }
          dataInputStream = new DataInputStream(new ByteArrayInputStream(arrayOfByte));
          paramInt1 = dataInputStream.readUnsignedByte();
          if(paramInt1 != 2)
          {
            this.m_log.error("IcspTcpTransport.processRX: bad protocol in decrypted data = " + paramInt1);
            return 0;
          }
          paramInt2 = dataInputStream.readUnsignedShort();
          b1 = (byte)paramInt1;
          b1 = (byte)(b1 + (paramInt2 >> 8));
          b1 = (byte)(b1 + (paramInt2 & 0xFF));
          for(b = 0; b < paramInt2; b++)
            b1 = (byte)(b1 + arrayOfByte[b + 3]);
          if(b1 != arrayOfByte[paramInt2 + 3])
          {
            this.m_log.error("IcspTcpTransport.processRX: decrypted checksum error packet, caluclated=" + b1 + " data=" + arrayOfByte[paramInt2 + 3]);
            return 0;
          }
          System.arraycopy(arrayOfByte, 3, paramArrayOfbyte, 0, paramInt2);
        }
        catch(IOException iOException)
        {
          this.m_log.error("IcspTcpTransport.processRX: parse error in encrypted data");
          return 0;
        }
      try
      {
        int i5;
        int i3 = dataInputStream.readUnsignedShort();
        i = dataInputStream.readUnsignedShort();
        j = dataInputStream.readUnsignedShort();
        k = dataInputStream.readUnsignedShort();
        m = dataInputStream.readUnsignedShort();
        n = dataInputStream.readUnsignedShort();
        i1 = dataInputStream.readUnsignedShort();
        dataInputStream.readUnsignedByte();
        i2 = dataInputStream.readUnsignedShort();
        int i4 = dataInputStream.readUnsignedShort();
        if(icspConnectionNode == null)
          icspConnectionNode = getConnectionNode(j);
        flag = ((i3 & 0x10) != 0) ? true : false;
        switch(i4)
        {
          case 1793:
            if(icspConnectionNode == null)
            {
              this.m_log.error("IcspTcpTransport.processRX: Authentication Challenge for unknown device -" + j);
              paramInt2 = 0;
              break;
            }
            if(this.m_md != null)
            {
              this.m_md.reset();
              byte[] arrayOfByte1 = new byte[4];
              arrayOfByte1[0] = dataInputStream.readByte();
              arrayOfByte1[1] = dataInputStream.readByte();
              arrayOfByte1[2] = dataInputStream.readByte();
              arrayOfByte1[3] = dataInputStream.readByte();
              icspConnectionNode.setAuthenticationChallenge(arrayOfByte1);
              this.m_md.update(arrayOfByte1);
              this.m_md.update(icspConnectionNode.getAuthenticationUsername().getBytes());
              this.m_md.update(icspConnectionNode.getAuthenticationPassword().getBytes());
              byte[] arrayOfByte2 = this.m_md.digest();
              sendAuthenticationResponse(m, n, i1, i, j, k, 2, arrayOfByte2);
            }
            else
            {
              this.m_log.error("IcspTcpTransport.processRX: Authentication Challenge received but not MD5 available.");
            }
            paramInt2 = 0;
            break;
          case 1795:
            if(icspConnectionNode == null)
            {
              this.m_log.error("IcspTcpTransport.processRX: Authentication result for unknown device -" + j);
              paramInt2 = 0;
              break;
            }
            i5 = dataInputStream.readUnsignedShort();
            if((i5 & 0x8000) != 0)
            {
              icspConnectionNode.setAuthenticationState(4);
            }
            else if((i5 & 0x1) != 0)
            {
              icspConnectionNode.setAuthenticationState(1);
              if((i5 & 0x6) != 0)
              {
                icspConnectionNode.setAuthenticationState(2);
                icspConnectionNode.setEncryptionMode(i5 & 0x6);
                switch(i5 & 0x6)
                {
                  case 4:
                    if(this.m_md != null)
                    {
                      this.m_md.reset();
                      this.m_md.update(icspConnectionNode.getAuthenticationChallenge());
                      try
                      {
                        byte[] arrayOfByte1 = Base64.decode(icspConnectionNode.getAuthenticationUsername());
                        byte[] arrayOfByte2 = Base64.decode(icspConnectionNode.getAuthenticationPassword());
                        this.m_md.update(arrayOfByte1);
                        this.m_md.update(arrayOfByte2);
                      }
                      catch(IOException iOException)
                      {
                        this.m_log.error("IcspTcpTransport.processRX: failed to build encryption key!");
                      }
                      byte[] arrayOfByte = this.m_md.digest();
                      ARC4 aRC4 = new ARC4(arrayOfByte, 16);
                      icspConnectionNode.setCrypto(aRC4);
                    }
                    break;
                }
              }
              icspConnectionNode.handleTransportEvent(2, icspConnectionNode.getAuthenticationState());
            }
            paramInt2 = 0;
            break;
        }
      }
      catch(IOException iOException)
      {
        this.m_log.error("IcspTcpTransport.processRX: parse error");
        return 0;
      }
      if(flag)
        sendAcknowledge(m, n, i1, i, j, k, i2);
      return paramInt2;
    }

    public synchronized bool send(byte[] paramArrayOfbyte, int paramInt)
    {
      if(paramArrayOfbyte == null)
      {
        this.m_log.error("IcspTcpTransport.send(): msg is null");
        return false;
      }
      if(paramInt > 2048)
      {
        this.m_log.error("IcspTcpTransport.send(): packet too large - " + paramInt + " bytes.");
        return false;
      }
      int i = 17;
      int j = i;
      this.m_txBuffer[j++] = 2;
      this.m_txBuffer[j++] = (byte)(paramInt >> 8 & 0xFF);
      this.m_txBuffer[j++] = (byte)(paramInt & 0xFF);
      System.arraycopy(paramArrayOfbyte, 0, this.m_txBuffer, j, paramInt);
      j += paramInt;
      int k = getTimeoutFlags();
      this.m_txBuffer[i + 3] = (byte)(this.m_txBuffer[i + 3] & 0xFF | k >> 8);
      this.m_txBuffer[i + 4] = (byte)(this.m_txBuffer[i + 4] & 0xFF | k & 0xFF);
      this.m_txBuffer[i + 17] = 15;
      byte b = 0;
      int m;
      for(m = i; m < i + paramInt + 3; m++)
        b = (byte)(b + this.m_txBuffer[m]);
      this.m_txBuffer[i + paramInt + 3] = b;
      m = paramInt + 4;
      int n = ((paramArrayOfbyte[10] & 0xFF) << 8) + (paramArrayOfbyte[11] & 0xFF);
      IcspConnectionNode icspConnectionNode = getConnectionNode(n);
      if(icspConnectionNode == null)
      {
        this.m_log.error("IcspTcpTransport.send: message send from unknown device - " + n);
        return false;
      }
      if(icspConnectionNode.getEncryptionMode() == 4)
      {
        Crypto crypto = icspConnectionNode.getCrypto();
        if(crypto != null && crypto instanceof ARC4) {
          ARC4 aRC4 = (ARC4)crypto;
          int i1 = (this.m_rand.nextInt() << 16) + this.m_rand.nextInt();
          byte[] arrayOfByte = new byte[4];
          arrayOfByte[0] = (byte)(i1 >> 24 & 0xFF);
          arrayOfByte[1] = (byte)(i1 >> 16 & 0xFF);
          arrayOfByte[2] = (byte)(i1 >> 8 & 0xFF);
          arrayOfByte[3] = (byte)(i1 & 0xFF);
          aRC4.crypt(this.m_txBuffer, i, m, arrayOfByte);
          i = j = 0;
          this.m_txBuffer[j++] = 4;
          this.m_txBuffer[j++] = (byte)(m + 18 - 4 >> 8 & 0xFF);
          this.m_txBuffer[j++] = (byte)(m + 18 - 4 & 0xFF);
          this.m_txBuffer[j++] = paramArrayOfbyte[8];
          this.m_txBuffer[j++] = paramArrayOfbyte[9];
          this.m_txBuffer[j++] = paramArrayOfbyte[10];
          this.m_txBuffer[j++] = paramArrayOfbyte[11];
          this.m_txBuffer[j++] = 2;
          this.m_txBuffer[j++] = 8;
          byte b1;
          for(b1 = 0; b1 < 4; b1++)
            this.m_txBuffer[j++] = arrayOfByte[b1];
          this.m_txBuffer[j++] = paramArrayOfbyte[2];
          this.m_txBuffer[j++] = paramArrayOfbyte[3];
          this.m_txBuffer[j++] = paramArrayOfbyte[4];
          this.m_txBuffer[j++] = paramArrayOfbyte[5];
          b = 0;
          for(b1 = 0; b1 < m + 18 - 1; b1++)
            b = (byte)(b + this.m_txBuffer[b1]);
          this.m_txBuffer[m + 18 - 1] = b;
          m += 18;
        } else
        {
          this.m_log.error("IcspTcpTransport.send: encryption algorithm not initialized for device " + n);
        }
      }
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
          this.m_log.error("IcspTcpTransport: Socket write exception");
        }
      }
      else
      {
        this.m_log.error("IcspTcpTransport: Bad m_socket. msg not sent");
      }
      return false;
    }

    public void setConnectionMode(int paramInt)
    {
      String str = "";
      if(paramInt != 3 && this.m_acceptSocket != null)
      {
        try
        {
          this.m_acceptSocket.close();
        }
        catch(IOException iOException) { }
        this.m_acceptSocket = null;
      }
      if(paramInt == 2)
      {
        str = "url";
      }
      else if(paramInt == 3)
      {
        str = "listen";
      }
      else if(paramInt == 1)
      {
        str = "auto";
      }
      else if(paramInt == 0)
      {
        str = "none";
      }
      else
      {
        if(this.m_log.getLogLevel() >= 2)
          this.m_log.warn("Unknown connection mode = " + paramInt);
        return;
      }
      this.m_connectMode = paramInt;
      this.m_props.setProperty("icsp.transport.mode", str);
      if(!this.m_props.save())
        this.m_log.error("IcspTcpTransport: unable to save mode information");
    }

    private int getTimeoutFlags()
    {
      return 512;
    }
  }