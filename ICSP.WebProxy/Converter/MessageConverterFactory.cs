using System;
using System.Collections.Generic;
using System.Linq;

using ICSP.Core.Reflection;

namespace ICSP.WebProxy.Converter
{
  public static class MessageConverterFactory
  {
    private static readonly Dictionary<Type, IMessageConverter> mConverters;

    static MessageConverterFactory()
    {
      mConverters = new Dictionary<Type, IMessageConverter>();

      var lTypes = TypeHelper.GetImplementedClassesForInterface(typeof(IMessageConverter));

      foreach(var type in lTypes)
      {
        if(type.IsAssignableFrom(typeof(IMessageConverter)))
          throw new ArgumentException("Converter is not assignable from IMessageConverter", nameof(type));

        var lType = (IMessageConverter)Activator.CreateInstance(type, true);

        mConverters.Add(type, lType);
      }
    }

    public static IMessageConverter GetConverter(string typeName)
    {
      if(string.IsNullOrWhiteSpace(typeName))
        return GetConverter<ModuleWebControlConverter>();

      var lType = Type.GetType(typeName, true);
      
      return (IMessageConverter)Activator.CreateInstance(lType);
    }

    public static IMessageConverter GetConverter<T>()
    {
      return mConverters.Single(p => p.Key == typeof(T)).Value;
    }
  }
}
