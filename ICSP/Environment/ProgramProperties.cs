using System;
using System.IO;
using System.Reflection;

using ICSP.Reflection;

namespace ICSP.Environment
{
  public static class ProgramProperties
  {
    public static T GetAssemblyAttribute<T>(Assembly assembly)
    {
      return GetAssemblyAttribute<T>(assembly, true);
    }

    public static T GetAssemblyAttribute<T>(Assembly assembly, bool throwExceptionIfNotExists)
    {
      T local = AttributeHelper.Get<T>(assembly);
      
      if (local == null && throwExceptionIfNotExists)
        throw new Exception(string.Format("Assembly {0} hat kein {1}.", assembly.FullName, typeof(T).Name));

      return local;
    }

    public static T GetEntryAssemblyAttribute<T>()
    {
      return GetAssemblyAttribute<T>(Assembly.GetEntryAssembly());
    }

    public static T GetEntryAssemblyAttribute<T>(bool throwExceptionIfNotExists)
    {
      return GetAssemblyAttribute<T>(Assembly.GetEntryAssembly(), throwExceptionIfNotExists);
    }

    #region Properties

    public static string BaseDirectory
    {
      get
      {
        return AppDomain.CurrentDomain.BaseDirectory;
      }
    }

    public static string Company
    {
      get
      {
        return GetEntryAssemblyAttribute<AssemblyCompanyAttribute>().Company;
      }
    }

    public static string Copyright
    {
      get
      {
        return GetEntryAssemblyAttribute<AssemblyCopyrightAttribute>().Copyright;
      }
    }

    public static string Description
    {
      get
      {
        return GetEntryAssemblyAttribute<AssemblyDescriptionAttribute>().Description;
      }
    }

    public static string ExecutablePath
    {
      get
      {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
      }
    }

    public static string ExeFileName
    {
      get
      {
        return AppDomain.CurrentDomain.FriendlyName;
      }
    }

    public static string Product
    {
      get
      {
        return GetEntryAssemblyAttribute<AssemblyProductAttribute>().Product;
      }
    }

    public static string Title
    {
      get
      {
        return GetEntryAssemblyAttribute<AssemblyTitleAttribute>().Title;
      }
    }

    public static Version Version
    {
      get
      {
        return Assembly.GetEntryAssembly().GetName().Version;

        // return Application.ProductVersion;
      }
    }

    #endregion
  }
}
