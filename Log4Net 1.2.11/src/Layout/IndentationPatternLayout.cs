using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Log4Net.Core;
using Log4Net.Layout.Pattern;
using Log4Net.Util;

namespace Log4Net.Layout
{
  public class IndentationPatternLayout : PatternLayout
  {
    private PatternConverter mMainConverter;
    
    public override void ActivateOptions()
    {
      var lParser = CreatePatternParser(ConversionPattern);

      var lInfo = new ConverterInfo()
      {
        Name = "indentation",
        Type = typeof(IndentationPatternConverter)
      };

      lParser.PatternConverters.Add("indentation", lInfo);

      mMainConverter = lParser.Parse();

      IgnoresException = false;
    }

    public override void Format(TextWriter writer, LoggingEvent loggingEvent)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");

      if (loggingEvent == null)
        throw new ArgumentNullException("loggingEvent");
            
      var lWriter = new IndentationWriter(writer);

      var lConverter = mMainConverter;

      // loop through the chain of pattern converters
      while (lConverter != null)
      {
        if (lConverter is IndentationPatternConverter)
          lWriter.SetIndentation();

        lConverter.Format(lWriter, loggingEvent);
        lConverter = lConverter.Next;
      }

      lWriter.Finish();
    }
  }

  public class IndentationWriter : TextWriter
  {
    TextWriter mWriter;

    int mIndentation = 0;

    List<string> mLines = new List<string>();

    public IndentationWriter(TextWriter writer)
    {
      mWriter = writer;
    }

    public override Encoding Encoding
    {
      get { return mWriter.Encoding; }
    }

    public override void Write(string value)
    {
      var lValues = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

      for (int i = 0; i < lValues.Length; i++)
        if (i > 0) lValues[i] = Environment.NewLine + lValues[i];

      mLines.AddRange(lValues);
    }

    public override void WriteLine(string value)
    {
      Write(value + Environment.NewLine);
    }

    public void Finish()
    {
      for (int i = 0; i < mLines.Count; i++)
      {
        var lLine = mLines[i];

        if (i < mLines.Count - 1) 
          lLine = mLines[i].Replace(Environment.NewLine, Environment.NewLine + new string(' ', mIndentation));

        mWriter.Write(lLine);
      }

      mLines.Clear();
    }

    public void SetIndentation()
    {
      foreach (var line in mLines)
        mIndentation += line.Length;
    }
  }
}
