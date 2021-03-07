using System;

namespace ICSP.Core
{
  public struct AmxDevice
  {
    public static readonly AmxDevice Empty = new AmxDevice();

    public AmxDevice(ushort dev, ushort port, ushort system)
    {
      Device = dev;
      Port = port;
      System = system;
    }

    public static AmxDevice FromSDP(byte[] bytes)
    {
      return new AmxDevice(
        (ushort)(bytes[2] * 256 + bytes[3]),
        (ushort)(bytes[4] * 256 + bytes[5]),
        (ushort)(bytes[0] * 256 + bytes[1]));
    }

    public static AmxDevice FromDPS(byte[] bytes)
    {
      return new AmxDevice(
        (ushort)(bytes[0] * 256 + bytes[1]),
        (ushort)(bytes[2] * 256 + bytes[3]),
        (ushort)(bytes[4] * 256 + bytes[5]));
    }

    public static AmxDevice FromSD(byte[] bytes)
    {
      return new AmxDevice(
        (ushort)(bytes[2] * 256 + bytes[3]),
        1, // Default Port
        (ushort)(bytes[0] * 256 + bytes[1]));
    }

    public ushort Device { get; }

    public ushort Port { get; }

    public ushort System { get; }

    public byte[] GetBytesSDP()
    {
      return new byte[6] {
        (byte)(System >> 8), (byte)System,
        (byte)(Device >> 8), (byte)Device,
        (byte)(Port >> 8),   (byte)Port, };
    }

    public byte[] GetBytesDPS()
    {
      return new byte[6] {
        (byte)(Device >> 8), (byte)Device,
        (byte)(Port >> 8),   (byte)Port,
        (byte)(System >> 8), (byte)System, };
    }

    public override string ToString()
    {
      return string.Format("{0:00000}:{1:000}:{2:000}", Device, Port, System);
    }

    public override bool Equals(Object obj)
    {
      return obj is AmxDevice && this == (AmxDevice)obj;
    }

    public override int GetHashCode()
    {
      return Device.GetHashCode() ^ Port.GetHashCode() ^ System.GetHashCode();
    }

    public static bool operator ==(AmxDevice device1, AmxDevice device2)
    {
      return device1.Device == device2.Device && device1.Port == device2.Port && device1.System == device2.System;
    }

    public static bool operator !=(AmxDevice device1, AmxDevice device2)
    {
      return !(device1 == device2);
    }
  }
}
