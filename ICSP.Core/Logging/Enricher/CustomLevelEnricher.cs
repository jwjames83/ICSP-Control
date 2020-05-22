using Serilog.Core;
using Serilog.Events;

namespace ICSP.Core.Logging.Enricher
{
  /// <summary>
  /// This is used to create a new property in Logs called 'LogLevel'
  /// So that we can map Serilog levels to LogLevel - so log files stay consistent
  /// </summary>
  public class CustomLevelEnricher : ILogEventEnricher
  {
    public const string PropertyName = "LogLevel";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
      var lLevel = string.Empty;

      switch(logEvent.Level)
      {
        case LogEventLevel.Verbose     /**/: lLevel = "Verbose"; break;
        case LogEventLevel.Debug       /**/: lLevel = "Debug"; break;
        case LogEventLevel.Information /**/: lLevel = "Info"; break;
        case LogEventLevel.Warning     /**/: lLevel = "Warning"; break;
        case LogEventLevel.Error       /**/: lLevel = "Error"; break;
        case LogEventLevel.Fatal       /**/: lLevel = "Fatal"; break;
      }

      logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(PropertyName, lLevel));
    }
  }

}
