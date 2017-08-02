using System;
using System.Linq;
using System.Reflection;
using System.Text;

using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  public abstract class MsgBaseCmdText<T> : ICSPMsg
  {
    protected MsgBaseCmdText()
    {
    }

    public MsgBaseCmdText(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));

        ValueType = (EncodingType)msg.Data[6];

        Length = msg.Data.GetBigEndianInt16(7);

        switch(ValueType)
        {
          case EncodingType.Default: Text = AmxUtils.GetString(msg.Data, 9, Length); break;
          case EncodingType.Unicode: Text = AmxUtils.GetUnicodeString(msg.Data, 9, Length); break;
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

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, string text)
    {
      var lRequest = CreateType();

      lRequest.Device = device;
      lRequest.ValueType = EncodingType.Default;
      lRequest.Length = (ushort)text?.Length;
      lRequest.Text = text;

      var lBytes = Encoding.Default.GetBytes(lRequest.Text);

      var lData = device.GetBytesDPS().
        Concat(ArrayExtensions.Int16To8Bit((byte)lRequest.ValueType)).
        Concat(ArrayExtensions.Int16ToBigEndian(lRequest.Length)).
        Concat(lBytes).
        ToArray();

      return lRequest.Serialize(device, source, lRequest.MsgCmd, lData);
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
      Logger.LogDebug(false, "{0} Device: {1}", GetType().Name, Device);
      Logger.LogDebug(false, "{0} Text  : {1}", GetType().Name, Text);
    }
  }
}
