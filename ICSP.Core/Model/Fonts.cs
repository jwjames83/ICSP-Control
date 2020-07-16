using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.Core.Model
{
  public class Fonts : List<Font>
  {
    [JsonExtensionData]
    private readonly IDictionary<string, JToken> mAdditionalData;

    public Fonts()
    {
      mAdditionalData = new Dictionary<string, JToken>();

      AllFonts = new List<Font>();

      /*
      Cannot deserialize the current JSON object (e.g. {"name":"value"}) into type 'ICSP.Core.Model.Fonts' because the type requires a JSON array (e.g. [1,2,3]) to deserialize correctly.
      To fix this error either change the JSON to a JSON array (e.g. [1,2,3]) or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer,
      not a collection type like an array or List<T>) that can be deserialized from a JSON object.
      JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object.Path 'fontList', line 2, position 13.
      */
    }

    [JsonIgnore]
    public List<Font> AllFonts { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      /*
      {
      "fontList": {
        "font": [
          {
            "file": "amx_icons.ttf",
            "fullName": "AMX Icon",
            "usageCount": "496"
          },
      */

      try
      {
        if(mAdditionalData.TryGetValue("fontList", out var fontListToken))
        {
          if(fontListToken["font"] != null)
          {
            foreach(var font in fontListToken["font"])
            {
              var lNumber = int.Parse(font["number"]?.ToString() ?? "0");

              var lFont = font?.ToObject<Font>();

              AllFonts?.Add(lFont);
            }
          }

          if(mAdditionalData.TryGetValue("fontList", out var subPageSetsToken))
          {
            if(subPageSetsToken["fontList"] != null)
              AllFonts.AddRange(subPageSetsToken["fontList"].ToObject<List<Font>>());
          }
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }

  public class Font
  {
    /*
    fnt.xma:
    ---------------------------------------------
    <font number="32">
      <file>arlrdbd.ttf</file>
      <fileSize>45260</fileSize>
      <faceIndex>0</faceIndex>
      <name>Arial Rounded MT Bold</name>
      <subfamilyName>Regular</subfamilyName>
      <fullName>Arial Rounded MT Bold</fullName>
      <size>10</size>
      <usageCount>8</usageCount>
    </font>

    fonts.xma:
    ---------------------------------------------
    <font>
      <file>amx_icons.ttf</file>
      <fullName>AMX Icon</fullName>
      <usageCount>496</usageCount>
    </font>
    */

    [JsonProperty("number", Order = 1)]
    public int Number { get; set; }

    [JsonProperty("file", Order = 2)]
    public string File { get; set; }

    [JsonProperty("fileSize", Order = 3, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int FileSize { get; set; }

    [JsonProperty("faceIndex", Order = 4)]
    public int FaceIndex { get; set; }

    [JsonProperty("name", Order = 5)]
    public string Name { get; set; }

    [JsonProperty("subfamilyName", Order = 6)]
    public string SubFamilyName { get; set; }

    [JsonProperty("fullName", Order = 7)]
    public string FullName { get; set; }

    [JsonProperty("size", Order = 8)]
    public int Size { get; set; }

    [JsonProperty("usageCount", Order = 9)]
    public int UsageCount { get; set; }
  }
}