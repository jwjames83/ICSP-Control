using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
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

    public MsgCmdStringSize(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));

        ValueType = (EncodingType)msg.Data[6];

        Length = msg.Data.GetBigEndianInt16(7);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, EncodingType valueType  , ushort length)
    {
      var lRequest = new MsgCmdStringSize();

      lRequest.Device = device;
      lRequest.ValueType = valueType;
      lRequest.Length = length;

      var lData = device.GetBytesDPS().
        Concat(ArrayExtensions.Int16To8Bit((byte)lRequest.ValueType)).
        Concat(ArrayExtensions.Int16ToBigEndian(lRequest.Length)).
        ToArray();
      
      return lRequest.Serialize(device, source, MsgCmd, lData);
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