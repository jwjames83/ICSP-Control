using System;

namespace ICSPControl
{
  public sealed class ArgumentValidation
  {
    private ArgumentValidation()
    {
    }

    public static void CheckEnumeration(Type enumType, object variable, string variableName)
    {
      CheckForNullReference(variable, "variable");
      CheckForNullReference(enumType, "enumType");
      CheckForNullReference(variableName, "variableName");

      if (!Enum.IsDefined(enumType, variable))
        throw new ArgumentException(string.Format("{0} is not a valid value for {1}.", variable.ToString(), enumType.FullName));
    }
    
    public static void CheckExpectedType(object variable, Type type)
    {
      CheckForNullReference(variable, "variable");
      CheckForNullReference(type, "type");

      if (!type.IsAssignableFrom(variable.GetType()))
        throw new ArgumentException(string.Format("The type is invalid. Expected type '{0}'.", type.FullName));
    }

    public static void CheckForEmptyString(string variable, string variableName)
    {
      CheckForNullReference(variable, variableName);
      CheckForNullReference(variableName, "variableName");

      if (variable.Length == 0)
        throw new ArgumentException(string.Format("The value of '{0}' can not be an empty string.", variableName));
    }

    public static void CheckForInvalidNullNameReference(string name, string messageName)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new InvalidOperationException(string.Format("The name for the '{0}' can not be null or an empty string.", messageName));
    }

    public static void CheckForNullReference(object variable, string variableName)
    {
      if (variableName == null)
        throw new ArgumentNullException("variableName");

      if (null == variable)
        throw new ArgumentNullException(variableName);
    }

    public static void CheckForZeroBytes(byte[] bytes, string variableName)
    {
      CheckForNullReference(bytes, "bytes");
      CheckForNullReference(variableName, "variableName");

      if (bytes.Length == 0)
        throw new ArgumentException(string.Format("The value must be greater than 0 bytes.", variableName));
    }
  }
}
