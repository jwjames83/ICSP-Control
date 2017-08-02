using System;
using System.Globalization;

namespace ICSP.Reflection
{
  public class TypeInfo
  {
    #region Attributes

    private const string NeutralCulture = "neutral";
    private const string VersionAttribute = "Version=";
    private const string CultureAttribute = "Culture=";
    private const string PublicKeyAttribute = "PublicKeyToken=";

    #endregion

    #region Constructors

    public TypeInfo(string qualifiedTypeName)
    {
      Culture = CultureInfo.CurrentCulture;

      if (string.IsNullOrWhiteSpace(qualifiedTypeName))
        throw new Exception(string.Format("TypeInfo: Ungueltiger Typname", new object[0]));

      var lStrArray = qualifiedTypeName.Split(new char[] { ',' });

      // ClassName
      if (lStrArray.Length > 0)
        ClassName = lStrArray[0].Trim();

      // AssemblyName
      if (lStrArray.Length > 1)
        AssemblyName = lStrArray[1].Trim();

      // Version
      if (lStrArray.Length > 2)
      {
        var lVersion = lStrArray[2].Trim();
        if (lVersion.StartsWith(VersionAttribute))
          Version = lVersion.Substring("Version=".Length, lVersion.Length - "Version=".Length).Trim();
      }

      // Culture
      if (lStrArray.Length > 3)
      {
        string lCultureName = null;
        string lCulture = lStrArray[3].Trim();

        if (lCulture.StartsWith(CultureAttribute))
          lCultureName = lCulture.Substring(CultureAttribute.Length, lCulture.Length - CultureAttribute.Length).Trim();

        if (lCultureName == NeutralCulture)
          Culture = CultureInfo.InvariantCulture;
        else if (lCultureName != null)
          Culture = new CultureInfo(lCultureName);
      }

      // PublicKey
      if (lStrArray.Length > 4)
      {
        string lPublicKey = lStrArray[2].Trim();
        if (lPublicKey.StartsWith(PublicKeyAttribute))
          PublicKey = lPublicKey.Substring(PublicKeyAttribute.Length, lPublicKey.Length - PublicKeyAttribute.Length).Trim();
      }
    }

    #endregion

    #region Properties

    public string ClassName { get; private set; }

    public string AssemblyName { get; private set; }

    public string Version { get; private set; }

    public CultureInfo Culture { get; private set; }

    public string PublicKey { get; private set; }

    #endregion
  }
}
