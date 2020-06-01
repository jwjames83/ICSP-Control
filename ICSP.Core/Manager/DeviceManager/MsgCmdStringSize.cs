using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
{
  /// <summary>
  /// This message is the response from a master to the Request String Size message.
  /// It is sent by a device/port upon reporting if the device/port supports more than 64 byte strings or 
  /// more than 8-bit character strings. It returns the maximum number of elements/string 
  /// the device supports and the types of strings supported.
  /// Note that when transferring messages, the size of the message will be determined by 
  /// the smaller of the maximum string size supported by the device and maximum packet size 
  /// supported by the low-level bus protocol.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.StringSize)]
  public class MsgCmdStringSize : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.StringSize;

    private MsgCmdStringSize()
    {
    }

    public MsgCmdStringSize(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(Data.Range(0, 6));

        ValueType = (EncodingType)Data[6];

        Length = Data.GetBigEndianInt16(7);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdStringSize(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, EncodingType valueType, ushort length)
    {
      var lRequest = new MsgCmdStringSize
      {
        Device = source,
        ValueType = valueType,
        Length = length
      };

      var lData = source.GetBytesDPS().
        Concat(ArrayExtensions.Int16To8Bit((byte)lRequest.ValueType)).
        Concat(ArrayExtensions.Int16ToBigEndian(lRequest.Length)).
        ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    /// <summary>
    /// The String ValueType
    /// </summary>
    public EncodingType ValueType { get; set; }

    /// <summary>
    ///  Unsigned 16-bit value
    /// </summary>
    public ushort Length { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} Length: {1}", GetType().Name, Length);
    }
  }
}