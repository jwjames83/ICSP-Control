using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ICSP.Core.Json
{
  public static partial class JsonExtensions
  {
    public static JToken DefaultFromObject(this JsonSerializer serializer, object value)
    {
      if(value == null)
        return JValue.CreateNull();

      var lDto = Activator.CreateInstance(typeof(DefaultSerializationDTO<>).MakeGenericType(value.GetType()), value);
      var lRoot = JObject.FromObject(lDto, serializer);

      return lRoot["Value"].RemoveFromLowestPossibleParent() ?? JValue.CreateNull();
    }

    public static object DefaultToObject(this JToken token, Type type, JsonSerializer serializer = null)
    {
      if(token == null)
        throw new ArgumentNullException(nameof(token));

      JContainer oldParent = token.Parent;

      var lDtoToken = new JObject(new JProperty("Value", token));
      Type lDtoType = typeof(DefaultSerializationDTO<>).MakeGenericType(type);
      var lDto = (IHasValue)(serializer ?? JsonSerializer.CreateDefault()).Deserialize(lDtoToken.CreateReader(), lDtoType);

      if(oldParent == null)
        token.RemoveFromLowestPossibleParent();

      return lDto == null ? null : lDto.GetValue();
    }

    public static JToken RemoveFromLowestPossibleParent(this JToken node)
    {
      if(node == null)
        return null;

      // If the parent is a JProperty, remove that instead of the token itself.
      JToken lToken = node.Parent is JProperty ? node.Parent : node;

      lToken.Remove();

      // Also detach the node from its immediate containing property -- Remove() does not do this even though it seems like it should
      if(lToken is JProperty)
        ((JProperty)node.Parent).Value = null;

      return node;
    }

    private interface IHasValue
    {
      object GetValue();
    }

    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy), IsReference = false)]
    private class DefaultSerializationDTO<T> : IHasValue
    {
      public DefaultSerializationDTO(T value) { Value = value; }

      public DefaultSerializationDTO() { }

      [JsonConverter(typeof(NoConverter)), JsonProperty(ReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
      public T Value { get; set; }

      object IHasValue.GetValue() { return Value; }
    }
  }

  public class NoConverter : JsonConverter
  {
    // NoConverter taken from this answer https://stackoverflow.com/a/39739105/3744182
    // To https://stackoverflow.com/questions/39738714/selectively-use-default-json-converter
    // By https://stackoverflow.com/users/3744182/dbc
    public override bool CanConvert(Type objectType) { throw new NotImplementedException(); /* This converter should only be applied via attributes */ }

    public override bool CanRead { get { return false; } }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) { throw new NotImplementedException(); }

    public override bool CanWrite { get { return false; } }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }
  }
}
