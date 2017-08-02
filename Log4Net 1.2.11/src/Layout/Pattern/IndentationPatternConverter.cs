using System.IO;

using Log4Net.Core;

namespace Log4Net.Layout.Pattern
{
	/// <summary>
  /// Converts %indentation to string
	/// </summary>
  internal sealed class IndentationPatternConverter : PatternLayoutConverter
  {
    protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
    {
      // do nothing - %indentation is used for indentation, so nothing should be written
    }
  }
}
