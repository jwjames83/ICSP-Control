using System;

namespace ICSP.Core
{
  public struct PanelType
  {
    /*
    G4-Panels (PPF.xml):
    -----------------------------------------------------------------------------
    <type>MVP-5150</type>
    <description>MVP-5150/5100: 5.2 inch color wireless Touch Panel</description>
    <deviceID>333</deviceID>
    -----------------------------------------------------------------------------
    
    G5-Panels (PPF.xml):
    -----------------------------------------------------------------------------
    <type>XF-700T</type>
    <product>MXT-701</product>
    <description>Modero X 7" tabletop touch panel</description>
    <deviceID>415</deviceID>
    -----------------------------------------------------------------------------
    */

    public PanelType(PanelGeneration generation, ushort deviceId, string type, string product, string description)
    {
      Generation = generation;
      DeviceId = deviceId;
      Type = type;
      Product = product;
      Description = description;
    }

    public PanelGeneration Generation { get; set; }

    public string Type { get; set; }

    public string Product { get; set; }

    public string Description { get; set; }

    public ushort DeviceId { get; set; }

    public override bool Equals(Object obj)
    {
      return obj is PanelType && this == (PanelType)obj;
    }

    public override int GetHashCode()
    {
      return DeviceId.GetHashCode() ^ Type.GetHashCode();
    }

    public static bool operator ==(PanelType device1, PanelType device2)
    {
      return device1.DeviceId == device2.DeviceId && device1.Type == device2.Type;
    }

    public static bool operator !=(PanelType device1, PanelType device2)
    {
      return !(device1 == device2);
    }
  }
}
