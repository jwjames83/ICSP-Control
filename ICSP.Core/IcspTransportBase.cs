using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSP.Core
{
  public abstract class IcspTransportBase : IcspTransport
  {
    public const int ICSP_AUTHENTICATED = 1;
    public const int ICSP_ENCRYPTION_ALGORITHM_NONE = 0;
    public const int ICSP_ENCRYPTION_ALGORITHM_EARC4 = 4;
    public const int ICSP_ENCRYPTION_ALGORITHM_FUTURE1 = 6;
    public const int ICSP_AUTHENTICATED_NOT_ALLOWED = 32768;
    public const int RECEIVE_TIMEOUT = 0;
    public const int RECEIVE_NOOP = -1;
    public const int RECEIVE_FAILURE = -2;
    public const int ENCRYPTION_ALGORITHM_MASK = 6;
    public const int ICSP_PACKET_ENCRYPTION_EARC4 = 2;
    public const int AUTHENTICATION_EVENT = 2;
    public const int NO_TRANSPORT = 0;
    public const int ICSNET_TRANSPORT = 1;
    public const int TCPIP_TRANSPORT = 2;
    public const int USB_TRANSPORT = 3;
    public const int UDPIP_TRANSPORT = 4;
    public const int CONNECTED = 0;
    public const int LISTENING_AUTO = 1;
    public const int LISTENING_PASSIVE = 2;
    public const int DISCONNECTED = 3;
    public const int CONNECTING = 4;
    public const int TERMINATED = 5;
    public const int NO_CONNECT = 0;
    public const int AUTO_CONNECT = 1;
    public const int URL_CONNECT = 2;
    public const int PASSIVE_CONNECT = 3;
    public const int NDP_CONNECT = 4;
    public const int NormalState = 0;
    public const int AuthenticatedState = 1;
    public const int AuthEncrypted = 2;
    public const int InvalidUser = 3;
    public const int AccessNotAllowed = 4;
    public const int NDPStateNone = -1;
    public const int NDPStateSearching = 0;
    public const int NDPStateOrphan = 1;
    public const int NDPStateLost = 2;
    public const int NDPStateFound = 3;
    public const int NDP_ReqFlagSearching = 1;
    public const int NDP_ReqFlagOrphan = 2;
    public const int NDP_ReqFlagLost = 4;
    public const int NDP_ReqFlagFound = 8;
    public const int NDP_ReqFlagMaster = 16;
    public const int NDP_ReqFlagDevValid = 256;
    public const int NDP_ReqFlagSysValid = 512;
    public const int NDP_ANNOUNCE_IDLE = 0;
    public const int NDP_ANNOUNCE_SEND_FIRST_ANNOUNCE = 1;
    public const int NDP_ANNOUNCE_REPEAT_ANNOUNCE_5S = 2;
    public const int NDP_ANNOUNCE_REPEAT_ANNOUNCE_1M = 3;
    public const int NDP_ANNOUNCE_REPEAT_ANNOUNCE_10M = 4;

    protected Logger m_log;

    protected Vector m_listeners;

    protected MxProperty m_props;

    protected String m_masterUrl;

    protected int m_masterPort;

    protected int m_system;

    protected IcspAddress m_icspAddress = new IcspAddress();

    protected int m_connectStatus;

    protected int m_connectMode;

    protected MessageDigest m_md;

    protected Random m_rand;

    protected bool m_terminated;

    public IcspTransportBase(Logger paramLogger, MxProperty paramMxProperty)
    {
      this.m_icspAddress.type = 2;
      this.m_icspAddress.data = new byte[4];
      try
      {
        InetAddress inetAddress = InetAddress.getLocalHost();
        System.arraycopy(inetAddress.getAddress(), 0, this.m_icspAddress.data, 0, 4);
      }
      catch(UnknownHostException unknownHostException)
      {
        this.m_icspAddress.data[0] = 0;
        this.m_icspAddress.data[1] = 0;
        this.m_icspAddress.data[2] = 0;
        this.m_icspAddress.data[3] = 0;
      }
      this.m_log = paramLogger;
      this.m_listeners = new Vector();
      if(paramMxProperty != null)
      {
        this.m_props = paramMxProperty;
      }
      else
      {
        this.m_props = new MxProperty();
      }
      this.m_masterUrl = "";
      this.m_masterPort = 1319;
      this.m_system = 0;
      this.m_connectStatus = 3;
      this.m_connectMode = 0;
      try
      {
        this.m_md = MessageDigest.getInstance("MD5");
      }
      catch(NoSuchAlgorithmException noSuchAlgorithmException)
      {
        this.m_log.error("IcspTransportBase.constructor: MD5 algorithm unavailable");
        this.m_md = null;
      }
      this.m_rand = new Random();
      this.m_masterUrl = this.m_props.getProperty("icsp.transport.url", "");
      try
      {
        this.m_system = Integer.parseInt(this.m_props.getProperty("icsp.transport.system", "0"));
      }
      catch(NumberFormatException numberFormatException)
      {
        this.m_system = 0;
      }
      try
      {
        this.m_masterPort = Integer.parseInt(this.m_props.getProperty("icsp.transport.port", "1319"));
      }
      catch(NumberFormatException numberFormatException)
      {
        this.m_masterPort = 1319;
      }
    }

    public abstract bool connect();

    public abstract void deviceChanged();

    public abstract bool disconnect();

    public abstract int receive(byte[] paramArrayOfbyte, int paramInt1, int paramInt2);

    public abstract bool send(byte[] paramArrayOfbyte, int paramInt);

    public abstract void setConnectionMode(int paramInt);

    public bool checkForReinit()
    {
      char c;
      String str = this.m_props.getProperty("icsp.transport.mode", "auto");
      if((str.equalsIgnoreCase("auto") && this.m_connectMode != 1) || (str.equalsIgnoreCase("url") && this.m_connectMode != 2) || (str.equalsIgnoreCase("listen") && this.m_connectMode != 3))
        return true;
      try
      {
        c = Integer.parseInt(this.m_props.getProperty("icsp.transport.port", "1319"));
      }
      catch(NumberFormatException numberFormatException)
      {
        c = 'ԧ';
      }
      if(c != this.m_masterPort)
        return true;
      if(this.m_connectMode == 2)
      {
        String str1 = this.m_props.getProperty("icsp.transport.url", "");
        if(!str1.equals(this.m_masterUrl))
          return true;
      }
      if(this.m_connectMode == 1)
      {
        bool bool;
        try
        {
          bool = Integer.parseInt(this.m_props.getProperty("icsp.transport.system", "0"));
        }
        catch(NumberFormatException numberFormatException)
        {
          bool = false;
        }
        if(bool && this.m_system != bool)
          return true;
      }
      return false;
    }

    public void terminate()
    {
      disconnect();
      this.m_connectStatus = 5;
      fireConnectEvent();
      this.m_listeners.clear();
      this.m_log = null;
      this.m_props = null;
      this.m_md = null;
      this.m_rand = null;
    }

    public void addListener(IcspTransportListener paramIcspTransportListener)
    {
      if(this.m_connectStatus == 5)
        return;
      if(this.m_log.getLogLevel() >= 4)
        this.m_log.debug("IcspTransportBase.AddListener()");
      if(!this.m_listeners.contains(paramIcspTransportListener))
        this.m_listeners.add(paramIcspTransportListener);
    }

    public int getConnectStatus()
    {
      return this.m_connectStatus;
    }

    public int getConnectionMode()
    {
      return this.m_connectMode;
    }

    public IcspAddress getHardwareAddress()
    {
      return this.m_icspAddress;
    }

    public int getMasterSystem()
    {
      return this.m_system;
    }

    public String getMasterURL()
    {
      return this.m_masterUrl;
    }

    public void removeListener(IcspTransportListener paramIcspTransportListener)
    {
      if(this.m_connectStatus == 5)
        return;
      if(this.m_log.getLogLevel() >= 4)
        this.m_log.debug("IcspTransportBase.removeListener()");
      this.m_listeners.remove(paramIcspTransportListener);
    }

    public void setMasterSystem(int paramInt)
    {
      if(this.m_connectStatus == 5)
        return;
      this.m_system = paramInt;
      this.m_props.setProperty("icsp.transport.system", Integer.toString(paramInt));
      if(!this.m_props.save())
        this.m_log.error("IcspTransportBase: unable to save master system number");
    }

    public void setMasterURL(String paramString)
    {
      if(this.m_connectStatus == 5)
        return;
      if(paramString != null)
      {
        this.m_masterUrl = paramString;
        this.m_props.setProperty("icsp.transport.url", this.m_masterUrl);
        if(!this.m_props.save())
          this.m_log.error("IcspTransportBase: unable to save master URL");
      }
    }

    protected void sendAuthenticationResponse(int paramInt1, int paramInt2, int paramInt3, int paramInt4, int paramInt5, int paramInt6, int paramInt7, byte[] paramArrayOfbyte)
    {
      byte[] arrayOfByte = new byte[37];
      arrayOfByte[0] = 0;
      arrayOfByte[1] = 0;
      arrayOfByte[2] = (byte)(paramInt1 >> 8 & 0xFF);
      arrayOfByte[3] = (byte)(paramInt1 & 0xFF);
      arrayOfByte[4] = (byte)(paramInt2 >> 8 & 0xFF);
      arrayOfByte[5] = (byte)(paramInt2 & 0xFF);
      arrayOfByte[6] = (byte)(paramInt3 >> 8 & 0xFF);
      arrayOfByte[7] = (byte)(paramInt3 & 0xFF);
      arrayOfByte[8] = (byte)(paramInt4 >> 8 & 0xFF);
      arrayOfByte[9] = (byte)(paramInt4 & 0xFF);
      arrayOfByte[10] = (byte)(paramInt5 >> 8 & 0xFF);
      arrayOfByte[11] = (byte)(paramInt5 & 0xFF);
      arrayOfByte[12] = (byte)(paramInt6 >> 8 & 0xFF);
      arrayOfByte[13] = (byte)(paramInt6 & 0xFF);
      arrayOfByte[14] = 16;
      arrayOfByte[15] = 15;
      arrayOfByte[16] = -1;
      arrayOfByte[17] = 7;
      arrayOfByte[18] = 2;
      arrayOfByte[19] = (byte)(paramInt7 >> 8 & 0xFF);
      arrayOfByte[20] = (byte)(paramInt7 & 0xFF);
      for(byte b = 0; b < 16; b++)
        arrayOfByte[b + 21] = paramArrayOfbyte[b];
      send(arrayOfByte, 37);
    }

    protected void sendAcknowledge(int paramInt1, int paramInt2, int paramInt3, int paramInt4, int paramInt5, int paramInt6, int paramInt7)
    {
      byte[] arrayOfByte = new byte[19];
      byte b = 8;
      arrayOfByte[0] = (byte)(b >> 8 & 0xFF);
      arrayOfByte[1] = (byte)(b & 0xFF);
      arrayOfByte[2] = (byte)(paramInt1 >> 8 & 0xFF);
      arrayOfByte[3] = (byte)(paramInt1 & 0xFF);
      arrayOfByte[4] = (byte)(paramInt2 >> 8 & 0xFF);
      arrayOfByte[5] = (byte)(paramInt2 & 0xFF);
      arrayOfByte[6] = (byte)(paramInt3 >> 8 & 0xFF);
      arrayOfByte[7] = (byte)(paramInt3 & 0xFF);
      arrayOfByte[8] = (byte)(paramInt4 >> 8 & 0xFF);
      arrayOfByte[9] = (byte)(paramInt4 & 0xFF);
      arrayOfByte[10] = (byte)(paramInt5 >> 8 & 0xFF);
      arrayOfByte[11] = (byte)(paramInt5 & 0xFF);
      arrayOfByte[12] = (byte)(paramInt6 >> 8 & 0xFF);
      arrayOfByte[13] = (byte)(paramInt6 & 0xFF);
      arrayOfByte[14] = 16;
      arrayOfByte[15] = (byte)(paramInt7 >> 8 & 0xFF);
      arrayOfByte[16] = (byte)(paramInt7 & 0xFF);
      arrayOfByte[17] = 0;
      arrayOfByte[18] = 1;
      send(arrayOfByte, 19);
    }

    protected void fireConnectEvent()
    {
      if(this.m_log.getLogLevel() >= 4)
        this.m_log.debug("IcspTransportBase.fireConnectEvent() with status " + this.m_connectStatus);
      int i = this.m_listeners.size();
      for(byte b = 0; b < i; b++)
      {
        IcspTransportListener icspTransportListener = this.m_listeners.get(b);
        icspTransportListener.icspTransportEvent(1, this.m_connectStatus);
      }
    }

    protected IcspConnectionNode getConnectionNode(int paramInt)
    {
      IcspConnectionNode icspConnectionNode = null;
      int i = this.m_listeners.size();
      for(byte b = 0; b < i && icspConnectionNode == null; b++)
      {
        IcspTransportListener icspTransportListener = this.m_listeners.get(b);
        icspConnectionNode = icspTransportListener.getConnectionNode(paramInt);
      }
      return icspConnectionNode;
    }
  }
