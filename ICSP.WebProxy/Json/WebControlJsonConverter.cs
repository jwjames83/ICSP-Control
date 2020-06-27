using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.WebProxy.Json
{
  public class WebControlJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(string);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      JToken lToken = JToken.FromObject(value);
      
      var lPAth = writer.Path;

      if(lToken.Type == JTokenType.String)
      {
        // Except color palettes
        if(writer.Path.StartsWith("palettes"))
        {
          lToken.WriteTo(writer);
        }
        else
        {
          if(string.IsNullOrEmpty(lToken.ToString()))
            writer.WriteNull();
          else
            lToken.WriteTo(writer);
        }
      }
      else
      {
        // Normal serialization
        lToken.WriteTo(writer);
      }
    }

    public override bool CanRead
    {
      get { return false; }
    }
  }
}
