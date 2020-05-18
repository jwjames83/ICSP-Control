using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is the response from a master to the Request Level Size message. 
  /// It is sent by a device/port upon reporting if the device/port/level supports 
  /// more BYTE (Type 0x10) levels. It returns a list of data types supported by Level.
  /// Note that when transferring levels, the master will typecast from larger sized 
  /// types to the largest type supported by the device. For example, 
  /// if the users Axcess2 program sends a FLOAT to a device that supports only 
  /// BYTE, CHAR, and INTEGER types, the master typecasts the FLOAT to an INTEGER 
  /// before sending to the device.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.LevelSize)]
  public class MsgCmdLevelSize : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.LevelSize;

    private MsgCmdLevelSize()
    {
    }

    public MsgCmdLevelSize(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(Data.Range(0, 6));

        Level = Data.GetBigEndianInt16(6);

        ValueTypeCount = Data[8];
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdLevelSize(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, ushort level, ushort count)
    {
      var lRequest = new MsgCmdLevelSize
      {
        Device = device,
        Level = level
      };

      var lData = device.GetBytesDPS().
        Concat(ArrayExtensions.Int16ToBigEndian(level)).
        Concat(ArrayExtensions.Int16To8Bit((byte)count)).
        ToArray();

      return lRequest.Serialize(device, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    public ushort Level { get; set; }

    /// <summary>
    ///  Unsigned 8-bit value.
    ///  This is the count of ValueTypes bytes to follow where each byte represents a supported level type.
    /// </summary>
    public byte ValueTypeCount { get; set; }

    /// <summary>
    /// List of Unsigned 8-bit array of ValueTypes.
    /// </summary>
    public byte[] ValueTypes { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device        : {1:l}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} Level         : {1}", GetType().Name, Level);
      Logger.LogDebug(false, "{0:l} ValueTypeCount: {1}", GetType().Name, ValueTypeCount);
    }
  }
}
