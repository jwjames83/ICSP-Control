using Newtonsoft.Json;

using TP.Design.Model;

namespace ICSP.WebProxy.WebControl
{
  public class WebControlFont
  {
    /*
    "57": {
      "file": "xiva symbol.ttf",
      "fileSize": "25060",
      "name": "XiVA Symbol",
      "subfamilyName": "Regular",
      "fullName": "XiVA Symbol",
      "size": "67",
      "usageCount": "2"
    }
    */

    [JsonIgnore]
    public int Number { get; set; }

    [JsonProperty("file", Order = 1)]
    public string File { get; set; }

    [JsonProperty("fileSize", Order = 2, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int FileSize { get; set; }

    [JsonIgnore]
    public int FaceIndex { get; set; }

    [JsonProperty("name", Order = 4)]
    public string Name { get; set; }

    [JsonProperty("subfamilyName", Order = 5)]
    public string SubFamilyName { get; set; }

    [JsonProperty("fullName", Order = 6)]
    public string FullName { get; set; }

    [JsonProperty("size", Order = 7)]
    public int Size { get; set; }

    [JsonProperty("usageCount", Order = 8)]
    public int UsageCount { get; set; }

    public static implicit operator WebControlFont(Font font)
    {
      return new WebControlFont()
      {
        Number = font.Number,
        File = font.File,
        FileSize = font.FileSize,
        FaceIndex = font.FaceIndex,
        Name = font.Name,
        SubFamilyName = font.SubFamilyName,
        FullName = font.FullName,
        Size = font.Size,
        UsageCount = font.UsageCount,
      };
    }

    public static implicit operator Font(WebControlFont font)
    {
      return new Font()
      {
        Number = font.Number,
        File = font.File,
        FileSize = font.FileSize,
        FaceIndex = font.FaceIndex,
        Name = font.Name,
        SubFamilyName = font.SubFamilyName,
        FullName = font.FullName,
        Size = font.Size,
        UsageCount = font.UsageCount,
      };
    }
  }
}