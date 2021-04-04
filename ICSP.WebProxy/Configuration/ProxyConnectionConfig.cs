using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using ICSP.Core;
using ICSP.Core.Client;

using Newtonsoft.Json;

namespace ICSP.WebProxy.Configuration
{
  public class ProxyConnectionConfig
  {
    public ProxyConnectionConfig()
    {
    }

    [JsonIgnore]
    public ProxyConfig Parent { get; set; }

    public int ID { get; set; }

    [Display(Name = "Localhost")]
    [Required]
    public string LocalHost { get; set; } = "http://*";

    [Display(Name = "Remotehost")]
    [Required]
    public string RemoteHost { get; set; }

    [Display(Name = "Remoteport")]
    [Required]
    public ushort RemotePort { get; set; } = ICSPClient.DefaultPort;

    [Display(Name = "UserName")]
    [Required]
    public string UserName { get; set; } = ICSPManager.DefaultUsername;

    [Display(Name = "Password")]
    public string Password { get; set; } = ICSPManager.DefaultPassword;

    [Display(Name = "Password")]
    [JsonIgnore]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string NewPassword { get; set; }

    [Display(Name = "Devices")]
    public List<ushort> Devices { get; set; } = new List<ushort>();

    [Display(Name = "Devices")]
    [Required]
    [JsonIgnore]
    public string DeviceList
    {
      get => string.Join(", ", Devices);
      set
      {
        try
        {
          if(string.IsNullOrWhiteSpace(value))
            Devices = new List<ushort>();
          else
            Devices = value.Split(new char[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(ushort.Parse).ToList();
        }
        catch(Exception) { }
      }
    }

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