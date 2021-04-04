using Newtonsoft.Json;

namespace TP.Design.Model.ProjectProperties
{
  public class SupportFileList
  {
    /*
    G4:
    <supportFileList>
      <mapFile>map.xma</mapFile>
      <colorFile>pal_001.xma</colorFile>
      <fontFile>fnt.xma</fontFile>
      <themeFile></themeFile>
      <iconFile>icon.xma</iconFile>
      <externalButtonFile></externalButtonFile>
    </supportFileList>
    
    G5:
    <supportFileList>
      <mapFile>map.xma</mapFile>
      <colorFile>pal_001.xma</colorFile>
      <fontFile>fonts.xma</fontFile>
      <themeFile></themeFile>
      <externalButtonFile></externalButtonFile>
      <appFile>G5Apps.xma</appFile>
      <logFile></logFile>
    </supportFileList>
    */

    [JsonProperty("mapFile", Order = 1)]
    public string MapFile { get; set; }

    [JsonProperty("colorFile", Order = 2)]
    public string ColorFile { get; set; }

    [JsonProperty("fontFile", Order = 3)]
    public string FontFile { get; set; }

    [JsonProperty("themeFile", Order = 4)]
    public string ThemeFile { get; set; }

    [JsonProperty("iconFile", Order = 5)]
    public string IconFile { get; set; }

    [JsonProperty("externalButtonFile", Order = 6)]
    public string ExternalButtonFile { get; set; }

    [JsonProperty("appFile", Order = 7)]
    public string AppFile { get; set; }

    [JsonProperty("logFile", Order = 8)]
    public string LogFile { get; set; }
  }
}