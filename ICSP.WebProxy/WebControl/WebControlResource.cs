
using ICSP.Core.Json;
using ICSP.Core.Model.ProjectProperties;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlResource
  {
    [JsonIgnore]
    public string Type { get; set; }

    [JsonIgnore]
    public string Name { get; set; }

    [JsonProperty("protocol", Order = 3)]
    public ProtocolType Protocol { get; set; }

    [JsonProperty("user", Order = 4, NullValueHandling = NullValueHandling.Ignore)]
    public string User { get; set; }

    [JsonIgnore]
    public bool PasswordEncrypted { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    [JsonProperty("host", Order = 6, NullValueHandling = NullValueHandling.Ignore)]
    public string Host { get; set; }

    [JsonProperty("path", Order = 7, NullValueHandling = NullValueHandling.Ignore)]
    public string Path { get; set; }

    [JsonProperty("file", Order = 8, NullValueHandling = NullValueHandling.Ignore)]
    public string File { get; set; }

    [JsonProperty("refresh", Order = 9, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int RefreshRate { get; set; }

    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("preserve", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool RefreshOnlyAtPanelStartup { get; set; }

    [JsonIgnore]
    public FormatType Format { get; set; } // xport-s, csv-headers, csv

    [JsonConverter(typeof(BoolConverter))]
    [JsonProperty("forceReload", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool ForceReload { get; set; }

    public static implicit operator WebControlResource(Resource font)
    {
      return new WebControlResource()
      {
        Type = font.Type,
        Name = font.Name,
        Protocol = font.Protocol,
        User = font.User,
        PasswordEncrypted = font.PasswordEncrypted,
        Password = font.Password,
        Host = font.Host,
        Path = font.Path,
        File = font.File,
        RefreshRate = font.RefreshRate,
        RefreshOnlyAtPanelStartup = font.RefreshOnlyAtPanelStartup,
        Format = font.Format,
        ForceReload = font.ForceReload,
      };
    }

    public static implicit operator Resource(WebControlResource font)
    {
      return new Resource()
      {
        Type = font.Type,
        Name = font.Name,
        Protocol = font.Protocol,
        User = font.User,
        PasswordEncrypted = font.PasswordEncrypted,
        Password = font.Password,
        Host = font.Host,
        Path = font.Path,
        File = font.File,
        RefreshRate = font.RefreshRate,
        RefreshOnlyAtPanelStartup = font.RefreshOnlyAtPanelStartup,
        Format = font.Format,
        ForceReload = font.ForceReload,
      };
    }
  }
}
