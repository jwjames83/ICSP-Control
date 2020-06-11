using System;

namespace ICSP.Core
{
  public struct PanelType
  {
    public ushort DeviceId { get; set; }

    public string DeviceType { get; set; }

    public override bool Equals(Object obj)
    {
      return obj is PanelType && this == (PanelType)obj;
    }

    public override int GetHashCode()
    {
      return DeviceId.GetHashCode() ^ DeviceType.GetHashCode();
    }

    public static bool operator ==(PanelType device1, PanelType device2)
    {
      return device1.DeviceId == device2.DeviceId && device1.DeviceType == device2.DeviceType;
    }

    public static bool operator !=(PanelType device1, PanelType device2)
    {
      return !(device1 == device2);
    }
  }
}
