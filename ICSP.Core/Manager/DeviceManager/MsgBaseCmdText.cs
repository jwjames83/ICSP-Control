using System;
using System.Linq;
using System.Reflection;
using System.Text;

using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
{
  public abstract class MsgBaseCmdText<T> : ICSPMsg
  {
    protected MsgBaseCmdText()
    {
    }

    public MsgBaseCmdText(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(Data.Range(0, 6));

        ValueType = (EncodingType)Data[6];

        Length = Data.GetBigEndianInt16(7);

        switch(ValueType)
        {
          case EncodingType.Default: Text = AmxUtils.GetString(Data, 9, Length); break;
          case EncodingType.Unicode: Text = AmxUtils.GetUnicodeString(Data, 9, Length); break;
        }
      }
    }

    private static MsgBaseCmdText<T> CreateType()
    {
      var lConstructor =
        typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
          .Where(constructor => constructor.GetParameters().Length == 0)
          .FirstOrDefault();

      if(lConstructor != null)
        return (MsgBaseCmdText<T>)lConstructor.Invoke(null);

      throw new ApplicationException("Command need an empty constructor");
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, string text)
    {
      return CreateRequest(dest, source, text, Encoding.GetEncoding(1252));
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, string text, Encoding encoding)
    {
      var lEncoding = encoding ??= Encoding.GetEncoding(1252);

      var lRequest = CreateType();

      lRequest.Device = source;
      lRequest.ValueType = EncodingType.Default;
      lRequest.Length = (ushort)text?.Length; // (i.e.number of elements, this is not the number of bytes)
      lRequest.Text = text ?? string.Empty;

      var lBytes = lEncoding.GetBytes(lRequest.Text);

      var lData = source.GetBytesDPS().
        Concat(ArrayExtensions.Int16To8Bit((byte)lRequest.ValueType)).
        Concat(ArrayExtensions.Int16ToBigEndian((ushort)lBytes.Length)). // lBytes.Length
        Concat(lBytes).
        ToArray();

      return lRequest.Serialize(dest, source, lRequest.MsgCmd, lData);
    }

    protected abstract ushort MsgCmd { get; }

    public AmxDevice Device { get; set; }

    /// <summary>
    /// Command ValueType Specifier
    /// </summary>
    public EncodingType ValueType { get; set; }

    /// <summary>
    ///  Unsigned 16-bit value. Number of characters in string
    ///  (i.e.number of elements, this is not the number of bytes)
    /// </summary>
    public ushort Length { get; set; }

    /// <summary>
    /// Command
    /// </summary>
    public string Text { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} Text  : {1:l}", GetType().Name, Text);
    }
  }
}
