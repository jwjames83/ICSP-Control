using System;

using Microsoft.VisualBasic.CompilerServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.WebProxy.Json
{
  public class WebControlJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(string) || objectType.IsPrimitive || objectType.IsEnum;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      switch(value)
      {
        case string s:
        {
          // Except color palettes
          if(writer.Path.StartsWith("palettes"))
          {
            writer.WriteValue(value);
          }
          else
          {
            if(string.IsNullOrEmpty(s))
              writer.WriteNull();
            else
              writer.WriteValue(value);
          }

          break;
        }
        case bool v:
        {
          writer.WriteValue(v ? "1" : "0");
          break;
        }
        case Enum v:
        {
          writer.WriteValue(((int)value).ToString().ToLower());
          break;
        }
        default:
        {
          // In JSON format, numbers and booleans do not have quotes around them, while strings do (see JSON.org).
          writer.WriteValue(value.ToString().ToLower());

          // writer.WriteValue(value);

          break;
        }
      }
    }

    public override bool CanRead
    {
      get { return false; }
    }
  }
}