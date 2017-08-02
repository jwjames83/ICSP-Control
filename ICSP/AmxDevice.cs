namespace ICSP
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
    
    public ushort Device { get; }

    public ushort Port { get; }

    public ushort System { get; }

    public byte[] GetBytesSDP()
    {
      byte[] bytes = new byte[6];

      bytes[0] = (byte)(System >> 8);
      bytes[1] = (byte)System;

      bytes[2] = (byte)(Device >> 8);
      bytes[3] = (byte)Device;

      bytes[4] = (byte)(Port >> 8);
      bytes[5] = (byte)Port;

      return bytes;
    }

    public byte[] GetBytesDPS()
    {
      byte[] bytes = new byte[6];

      bytes[0] = (byte)(Device >> 8);
      bytes[1] = (byte)Device;

      bytes[2] = (byte)(Port >> 8);
      bytes[3] = (byte)Port;

      bytes[4] = (byte)(System >> 8);
      bytes[5] = (byte)System;

      return bytes;
    }

    public override string ToString()
    {
      return string.Format("{0:00000}:{1:000}:{2:000}", Device, Port, System);
    }
  }
}
