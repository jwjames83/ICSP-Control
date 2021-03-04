using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSP.Core
{
  public interface IcspTransport
  {
    bool checkForReinit();

    void terminate();

    bool send(byte[] paramArrayOfbyte, int paramInt);

    int receive(byte[] paramArrayOfbyte, int paramInt1, int paramInt2);

    bool connect();

    bool disconnect();

    IcspAddress getHardwareAddress();

    int getConnectStatus();

    void setConnectionMode(int paramInt);

    int getConnectionMode();

    void addListener(IcspTransportListener paramIcspTransportListener);

    void removeListener(IcspTransportListener paramIcspTransportListener);

    String getMasterURL();

    void setMasterURL(String paramString);

    int getMasterSystem();

    void setMasterSystem(int paramInt);

    void deviceChanged();
  }

  public interface IcspTransportListener
  {
    public static int CONNECT_EVENT = 1;

    void icspTransportEvent(int paramInt1, int paramInt2);

    Vector getNdpReportingData();

    IcspConnectionNode getConnectionNode(int paramInt);
  }

  public class IcspAddress
  {
    public static int NoAddress = 0;

    public static int NeuronID = 1;

    public static int IPAddress = 2;

    public static int Axlink = 3;

    public static int MacAddress = 4;

    public static int IPv4Port = 5;

    public static int IPv4PortMacAddress = 6;

    public static int NeuronSubNode_ICSP = 16;

    public static int NeuronSubNode_PL = 17;

    public static int IPSocketAddress = 18;

    public static int RS232 = 19;

    public int type;

    public byte[] data;

    public static int getTypeLen(int paramInt)
    {
      switch(paramInt)
      {
        default:
          return 0;
        case 1:
        case 4:
        case 5:
          return 6;
        case 2:
          return 4;
        case 6:
          return 12;
        case 16:
        case 17:
          return 2;
        case 18:
          break;
      }
      return 8;
    }

    public byte[] getIPv4()
    {
      byte[] arrayOfByte;
      switch(this.type)
      {
        case 2:
        case 5:
        case 6:
          arrayOfByte = new byte[4];
          System.arraycopy(this.data, 0, arrayOfByte, 0, 4);
          return arrayOfByte;
        case 18:
          arrayOfByte = new byte[4];
          System.arraycopy(this.data, 4, arrayOfByte, 0, 4);
          return arrayOfByte;
      }
      return null;
    }

    public int getPort()
    {
      switch(this.type)
      {
        case 5:
        case 6:
          return (this.data[4] & 0xFF) << 8 | this.data[5] & 0xFF;
      }
      return 1319;
    }

    public byte[] getMacAddress()
    {
      if(this.type == 6)
      {
        byte[] arrayOfByte = new byte[6];
        System.arraycopy(this.data, 6, arrayOfByte, 0, 6);
        return arrayOfByte;
      }
      if(this.type == 4)
      {
        byte[] arrayOfByte = new byte[6];
        System.arraycopy(this.data, 0, arrayOfByte, 0, 6);
        return arrayOfByte;
      }
      return null;
    }

    public boolean setIpv4(byte[] paramArrayOfbyte)
    {
      switch(this.type)
      {
        case 2:
        case 5:
        case 6:
          if(this.data == null)
            this.data = new byte[getTypeLen(this.type)];
          System.arraycopy(paramArrayOfbyte, 0, this.data, 0, 4);
          return true;
        case 18:
          if(this.data == null)
            this.data = new byte[getTypeLen(this.type)];
          System.arraycopy(paramArrayOfbyte, 0, this.data, 4, 4);
          return true;
      }
      return false;
    }

    public boolean setPort(int paramInt)
    {
      switch(this.type)
      {
        case 5:
        case 6:
          if(this.data == null)
            this.data = new byte[getTypeLen(this.type)];
          this.data[4] = (byte)(paramInt >> 8 & 0xFF);
          this.data[5] = (byte)(paramInt & 0xFF);
          return true;
      }
      return false;
    }

    public boolean setMacAddress(byte[] paramArrayOfbyte)
    {
      if(this.type == 6)
      {
        if(this.data == null)
          this.data = new byte[getTypeLen(this.type)];
        System.arraycopy(paramArrayOfbyte, 0, this.data, 6, 6);
        return true;
      }
      if(this.type == 4)
      {
        if(this.data == null)
          this.data = new byte[getTypeLen(this.type)];
        System.arraycopy(paramArrayOfbyte, 0, this.data, 0, 6);
        return true;
      }
      return false;
    }
  }
}