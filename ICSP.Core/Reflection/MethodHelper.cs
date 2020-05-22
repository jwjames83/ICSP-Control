using System.Reflection;
using System.Text.RegularExpressions;

namespace ICSP.Core.Reflection
{
  public static class MethodHelper
  {
    private static readonly Regex AsychMethodName = new Regex(@"^<*(?<Name>\w+)>\w+?");

    public static (string Type, string Name) GetMethodName(this MethodBase method)
    {
      if(method == null)
        return (null, null);

      // <<Main>b__0>d
      var lDeclaringType = method.DeclaringType?.Name;

      // MoveNext
      // <OnProcess>b__13_0
      // <ProcessAsync>d__4
      var lMethodName = method.Name;

      // Fix Asynch-Method / Anonymous Task-Methods
      if(lMethodName == "MoveNext" || lMethodName.Contains("<"))
      {
        if(lMethodName == "MoveNext")
        {
          // <<Main>b__0>d      => Main
          // <ProcessAsync>d__4 => ProcessAsync
          lMethodName = AsychMethodName.Match(lDeclaringType).Groups["Name"].Value;

          var lType = method.DeclaringType;

          while(lType?.Name?.Contains("<") ?? false)
            lType = lType.DeclaringType;

          if(lType != null)
            lDeclaringType = lType.Name;
        }
        else
        {
          // <<Main>b__0>d      => Main
          // <ProcessAsync>d__4 => ProcessAsync
          lMethodName = AsychMethodName.Match(method.Name).Groups["Name"].Value;

          if(lDeclaringType.Contains("<"))
          {
            var lType = method.DeclaringType;

            while(lType?.Name?.Contains("<") ?? false)
              lType = lType.DeclaringType;

            if(lType != null)
              lDeclaringType = lType.Name;
          }
        }
      }

      return (lDeclaringType, lMethodName);
    }
  }
}
