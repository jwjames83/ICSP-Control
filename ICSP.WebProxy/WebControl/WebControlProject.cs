using System.Collections.Generic;

using Newtonsoft.Json;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlProject
  {
    public WebControlProject()
    {
      Settings = new WebControlSettings();

      Pages = new Dictionary<string, WebControlPage>();

      SubPages = new Dictionary<string, WebControlSubPage>();

      Palettes = new Dictionary<string, Dictionary<int, WebControlPaletteDataItem>>();

      Fonts = new Dictionary<int, WebControlFont>();

      Chameleons = new Dictionary<string, string>();
    }

    [JsonProperty("settings", Order = 1)]
    public WebControlSettings Settings { get; set; }

    [JsonProperty("pages", Order = 2)]
    public Dictionary<string, WebControlPage> Pages { get; set; }

    [JsonProperty("subpages", Order = 3)]
    public Dictionary<string, WebControlSubPage> SubPages { get; set; }

    [JsonProperty("palettes", Order = 4)]
    public Dictionary<string, Dictionary<int, WebControlPaletteDataItem>> Palettes { get; set; }

    [JsonProperty("fonts", Order = 5)]
    public Dictionary<int, WebControlFont> Fonts { get; set; }

    [JsonProperty("chameleons", Order = 6)]
    public Dictionary<string, string> Chameleons { get; set; }
  }
}