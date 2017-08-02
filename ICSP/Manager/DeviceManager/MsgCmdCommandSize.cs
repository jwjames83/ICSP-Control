using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is the response, from a device, to the Request Command Size message.
  /// It is sent by a device/port upon reporting if the device/port supports more than 64 byte commands or 
  /// more than 8-bit character commands. It returns the maximum number of elements/command 
  /// the device supports and the types of strings supported.
  /// Note that when transferring messages, the size of the message will be determined by 
  /// the smaller of the maximum command size supported by the device and maximum packet size 
  /// supported by the low-level bus protocol.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.CommandSize)]
  public class MsgCmdCommandSize : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.CommandSize;

    private MsgCmdCommandSize()
    {
    }

    public MsgCmdCommandSize(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));

        ValueType = (EncodingType)msg.Data[6];

        Length = msg.Data.GetBigEndianInt16(7);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, EncodingType valueType, ushort length)
    {
      var lRequest = new MsgCmdCommandSize();

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
      Logger.LogDebug(false, "{0} Device: {1}", GetType().Name, Device);
      Logger.LogDebug(false, "{0} Length: {1}", GetType().Name, Length);
    }
  }
}