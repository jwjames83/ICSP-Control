using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.WebProxy.Json
{
  public class NullStringConverter : JsonConverter
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

      if(lToken.Type == JTokenType.String)
      {
        if(string.IsNullOrEmpty(lToken.ToString()))
          writer.WriteNull();
        else
          lToken.WriteTo(writer);
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
