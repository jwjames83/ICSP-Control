﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ICSP.Core.Reflection
{
  public static class TypeHelper
  {
    private static readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

    public static T CreateInstance<T>()
    {
      return Activator.CreateInstance<T>();
    }

    public static object CreateInstance(string sTypeName)
    {
      return Activator.CreateInstance(GetType(sTypeName));
    }

    public static object CreateInstance(Type type)
    {
      return Activator.CreateInstance(type);
    }

    public static T CreateInstance<T>(string sTypeName, params object[] args) where T : class
    {
      return CreateInstance<T>(GetType(sTypeName), args);
    }

    public static object CreateInstance(string sTypeName, params object[] args)
    {
      return Activator.CreateInstance(GetType(sTypeName), args);
    }

    public static object CreateInstance(Type type, params object[] args)
    {
      return Activator.CreateInstance(type, args);
    }

    public static T CreateInstance<T>(Type t, params object[] args) where T : class
    {
      if(Activator.CreateInstance(t, args) is not T local)
      {
        throw new Exception(string.Format("{0} ist nicht vom Typ {1}", t.ToString(), typeof(T).ToString()));
      }

      return local;
    }

    public static string CreateQualifiedName(Type type)
    {
      return type.AssemblyQualifiedName;
    }

    public static string CreateQualifiedNameWithoutVersion(Type type)
    {
      return Assembly.CreateQualifiedName(Path.GetFileNameWithoutExtension(type.Module.ScopeName), type.FullName);
    }

    public static List<Type> GetImplementedClassesForInterface(Type interfaceType)
    {
      return GetImplementedClassesForInterface(Assembly.GetCallingAssembly(), interfaceType);
    }

    public static List<Type> GetImplementedClassesForInterface(Assembly searchAssembly, Type interfaceType)
    {
      var lList = new List<Type>();

      var lTypes = searchAssembly.GetTypes();

      for(int i = 0; i < lTypes.Length; i++)
      {
        var lInterfaces = lTypes[i].GetInterfaces();

        for(int j = 0; j < lInterfaces.Length; j++)
        {
          if(lInterfaces[j].Equals(interfaceType))
          {
            lList.Add(lTypes[i]);
            break;
          }
        }
      }

      return lList;
    }

    public static object GetObjectValue(object o, string fieldOrPropertyName)
    {
      if(o != null)
      {
        PropertyInfo property = o.GetType().GetProperty(fieldOrPropertyName);

        if(property != null)
        {
          return property.GetValue(o, null);
        }

        FieldInfo field = o.GetType().GetField(fieldOrPropertyName);

        if(field != null)
        {
          return field.GetValue(o);
        }
      }

      return null;
    }

    public static List<Type> GetSublassesOfType(Type baseClassType)
    {
      return GetSublassesOfType(baseClassType, baseClassType.Assembly);
    }

    public static List<Type> GetSublassesOfType(Type baseClassType, Assembly asm)
    {
      if((baseClassType == null) || (asm == null))
        throw new ArgumentException("BaseClassType oder Assembly ist null");

      var lTypes = new List<Type>();

      foreach(Type type in asm.GetTypes())
      {
        if(type.IsSubclassOf(baseClassType))
          lTypes.Add(type);
      }

      return lTypes;
    }

    public static Type GetType(string typeName)
    {
      if(!TypeCache.TryGetValue(typeName, out Type type))
      {
        TypeInfo info = new TypeInfo(typeName);

        if(info.AssemblyName == null)
        {
          foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
          {
            type = assembly.GetType(info.ClassName);
            if(null != type)
            {
              break;
            }
          }
        }
        else
        {
          type = Assembly.Load(info.AssemblyName).GetType(info.ClassName);
        }

        TypeCache[typeName] = type;
      }

      return type;
    }

    public static Type GetType(string sTypeName, Assembly[] assemblies)
    {
      TypeInfo info = new TypeInfo(sTypeName);

      foreach(Assembly assembly in assemblies)
      {
        Type type = assembly.GetType(info.ClassName);

        if(null != type)
        {
          return type;
        }
      }

      return null;
    }

    public static bool IsNumber(Type type)
    {
      if(((!IsTypeOrNullable<short>(type) && !IsTypeOrNullable<int>(type)) && (!IsTypeOrNullable<long>(type) && !IsTypeOrNullable<ushort>(type))) && ((!IsTypeOrNullable<uint>(type) && !IsTypeOrNullable<ulong>(type)) && (!IsTypeOrNullable<float>(type) && !IsTypeOrNullable<double>(type))))
      {
        return false;
      }
      return true;
    }

    public static bool IsReal(Type type)
    {
      if(!IsTypeOrNullable<float>(type) && !IsTypeOrNullable<double>(type))
      {
        return false;
      }

      return true;
    }

    public static bool IsRightAligned(Type type)
    {
      if(((!IsTypeOrNullable<short>(type) && !IsTypeOrNullable<int>(type)) && (!IsTypeOrNullable<long>(type) && !IsTypeOrNullable<ushort>(type))) && (((!IsTypeOrNullable<uint>(type) && !IsTypeOrNullable<ulong>(type)) && (!IsTypeOrNullable<float>(type) && !IsTypeOrNullable<double>(type))) && (!IsTypeOrNullable<DateTime>(type) && !IsTypeOrNullable<TimeSpan>(type))))
      {
        return false;
      }

      return true;
    }

    public static bool IsTypeOrNullable<T>(Type type) where T : struct
    {
      if(!(typeof(T) == type))
      {
        return (typeof(T?) == type);
      }

      return true;
    }
  }
}