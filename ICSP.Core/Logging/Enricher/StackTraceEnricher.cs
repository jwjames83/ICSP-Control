﻿using System.Diagnostics;
using System.Text.RegularExpressions;

using ICSP.Core.Reflection;

using Serilog.Core;
using Serilog.Events;

namespace ICSP.Core.Logging.Enricher
{
  public class StackTraceEnricher : ILogEventEnricher
  {
    public const string PropertyName = "StackTrace";

    public static readonly Regex AsychMethodName = new Regex(@"^<(?<Name>\w+)>\w+?");

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
      var lCallerMethod = new StackTrace().GetFrame(7).GetMethod();

      var lInfo = lCallerMethod.GetMethodInfo();

      logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(PropertyName, $"({lInfo.Type?.Name}.{lInfo.MethodName}) => "));
    }
  }
}
