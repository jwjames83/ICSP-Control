using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ICSP.Core.Client;

namespace ICSP.WebProxy.Configuration
{
  public class ProxyConnectionViewModel
  {
    public ProxyConnectionViewModel()
    {
    }

    [JsonIgnore]
    public ProxyConfig Parent { get; set; }

    public int ID { get; set; }

    [Display(Name = "Localhost")]
    public string LocalHost { get; set; }

    [Display(Name = "Remotehost")]
    public string RemoteHost { get; set; }

    [Display(Name = "Remoteport")]
    public ushort RemotePort { get; set; } = ICSPClient.DefaultPort;

    [Display(Name = "Devices")]
    public List<ushort> Devices { get; set; } = new List<ushort>();

    [Display(Name = "Devices")]
    public string DeviceList { get => string.Join(", ", Devices); }

    [Display(Name = "Base Directory")]
    public string BaseDirectory { get; set; }

    [Display(Name = "Request Path")]
    public string RequestPath { get; set; }

    [Display(Name = "Enabled")]
    public bool Enabled { get; set; } = true;

    [Display(Name = "Converter")]
    public string Converter { get; set; }

    public Dictionary<string, ProxyDeviceConfig> DeviceConfig { get; set; } = new Dictionary<string, ProxyDeviceConfig>();
  }
}