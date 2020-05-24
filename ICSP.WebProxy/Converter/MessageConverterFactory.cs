using System;
using System.Collections.Generic;
using System.Linq;

using ICSP.Core.Reflection;

namespace ICSP.WebProxy.Converter
{
  public static class MessageConverterFactory
  {
    private static readonly Dictionary<Type, IMessageConverter> mTranslators;

    static MessageConverterFactory()
    {
      mTranslators = new Dictionary<Type, IMessageConverter>();

      var lTypes = TypeHelper.GetImplementedClassesForInterface(typeof(IMessageConverter));

      foreach(var type in lTypes)
      {
        if(type.IsAssignableFrom(typeof(IMessageConverter)))
          throw new ArgumentException("Translator is not assignable from IMessageTranslator", nameof(type));

        var lType = (IMessageConverter)Activator.CreateInstance(type, true);

        mTranslators.Add(type, lType);
      }
    }

    public static IMessageConverter GetConverter<T>()
    {
      return mTranslators.Single(p => p.Key == typeof(T)).Value;
    }
  }
}
