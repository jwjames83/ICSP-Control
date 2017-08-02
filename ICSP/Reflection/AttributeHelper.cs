using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ICSP.Reflection
{
  public class AttributeHelper
  {
    #region Constructors

    private AttributeHelper()
    {
    }

    #endregion

    #region Methods

    #region Contains

    public static bool Contains<T>(List<Attribute> attributes)
    {
      return (Get<T>(attributes) != null);
    }

    public static bool Contains<T>(AttributeCollection attributes)
    {
      return (Get<T>(attributes) != null);
    }

    public static bool Contains<T>(MemberDescriptor memberDescriptor)
    {
      return (Get<T>(memberDescriptor.Attributes) != null);
    }

    public static bool Contains<T>(object[] attributes)
    {
      return (Get<T>(attributes) != null);
    }

    public static bool Contains<T>(MemberInfo memberInfo)
    {
      return (Get<T>(memberInfo) != null);
    }

    public static bool ContainsByName<TAttribute>(object[] attributes)
    {
      var lFullName = typeof(TAttribute).FullName;

      foreach (object attribute in attributes)
      {
        if (lFullName == attribute.GetType().FullName)
          return true;
      }

      return false;
    }

    #endregion

    #region Get

    public static T Get<T>(List<Attribute> attributes)
    {
      foreach (object attribute in attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          return (T)attribute;
      }

      return default(T);
    }

    public static T Get<T>(AttributeCollection attributes)
    {
      foreach (object attribute in attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          return (T)attribute;
      }

      return default(T);
    }

    public static T Get<T>(MemberDescriptor memberDescriptor)
    {
      foreach (object attribute in memberDescriptor.Attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          return (T)attribute;
      }

      return default(T);
    }

    public static T Get<T>(Assembly assembly)
    {
      foreach (object attribute in assembly.GetCustomAttributes(typeof(T), true))
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          return (T)attribute;
      }

      return default(T);
    }

    public static T Get<T>(MemberInfo memberInfo)
    {
      foreach (object attribute in memberInfo.GetCustomAttributes(typeof(T), true))
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          return (T)attribute;
      }

      return default(T);
    }

    public static T Get<T>(object[] attributes)
    {
      foreach (object attribute in attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          return (T)attribute;
      }

      return default(T);
    }

    public static TAttributeType Get<TAttributeType>(Type type, bool inherit)
    {
      return (TAttributeType)Get(type, typeof(TAttributeType), inherit);
    }

    public static object Get(Type type, Type attributeType, bool inherit)
    {
      var lCustomAttributes = type.GetCustomAttributes(attributeType, inherit);
      
      if (lCustomAttributes.Length != 1)
        return null;

      return lCustomAttributes[0];
    }

    #endregion

    #region GetList

    public static T[] GetList<T>(object[] attributes)
    {
      var lAttributes = new List<T>();

      foreach (object attribute in attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          lAttributes.Add((T)attribute);
      }

      return lAttributes.ToArray();
    }

    public static T[] GetList<T>(List<Attribute> attributes)
    {
      var lAttributes = new List<T>();

      foreach (object attribute in attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          lAttributes.Add((T)attribute);
      }

      return lAttributes.ToArray();
    }

    public static T[] GetList<T>(AttributeCollection attributes)
    {
      var lAttributes = new List<T>();

      foreach (object attribute in attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          lAttributes.Add((T)attribute);
      }

      return lAttributes.ToArray();
    }

    public static T[] GetList<T>(MemberDescriptor memberDescriptor)
    {
      var lAttributes = new List<T>();

      foreach (object attribute in memberDescriptor.Attributes)
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          lAttributes.Add((T)attribute);
      }

      return lAttributes.ToArray();
    }

    public static T[] GetList<T>(Assembly assembly)
    {
      var lAttributes = new List<T>();

      foreach (object attribute in assembly.GetCustomAttributes(typeof(T), true))
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          lAttributes.Add((T)attribute);
      }

      return lAttributes.ToArray();
    }

    public static T[] GetList<T>(MemberInfo memberInfo)
    {
      var lAttributes = new List<T>();

      foreach (object attribute in memberInfo.GetCustomAttributes(typeof(T), true))
      {
        if (typeof(T).IsAssignableFrom(attribute.GetType()))
          lAttributes.Add((T)attribute);
      }

      return lAttributes.ToArray();
    }

    #endregion

    #endregion
  }
}
