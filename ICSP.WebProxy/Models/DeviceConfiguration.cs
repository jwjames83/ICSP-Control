using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ICSP.WebProxy.Models
{
  public class DeviceConfiguration
  {
    [Required]
    [Display(Name = "Panel Type", Prompt = "Panel Type")]
    public string PanelType { get; set; }

    [Required]
    [Range(1, 100)]
    // [Range(1, 100, ErrorMessage = "Wert zwischen 1 und 100 liegen")]
    [Display(Name = "Portcount", Prompt = "Portcount")]
    public ushort PortCount { get; set; } = 1;

    [Display(Name = "Devicename", Prompt = "Devicename")]
    public string DeviceName { get; set; }
  }
}