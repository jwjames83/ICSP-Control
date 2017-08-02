using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is used to indicate, to the master, that a device/port/level value has changed.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.LevelValueDevMaster)]
  public class MsgCmdLevelValueDevMaster : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.LevelValueDevMaster;

    private MsgCmdLevelValueDevMaster()
    {
    }

    public MsgCmdLevelValueDevMaster(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));

        Level = msg.Data.GetBigEndianInt16(6);

        ValueType = (LevelValueType)msg.Data[8];

        switch(ValueType)
        {
          // 1 Bytes
          case LevelValueType.Byte: Value = msg.Data[9]; break;
          case LevelValueType.Char: Value = msg.Data[9]; break;

          // 2 Bytes
          case LevelValueType.Integer: Value = msg.Data.GetBigEndianInt16(9); break;
          case LevelValueType.SInteger: Value = msg.Data.GetBigEndianInt16(9); break;

          // 4 Bytes
          case LevelValueType.ULong: Value = msg.Data.GetBigEndianInt32(9); break;
          case LevelValueType.Long: Value = msg.Data.GetBigEndianInt32(9); break;
          case LevelValueType.Float: Value = msg.Data.GetBigEndianInt32(9); break;

          // 8 Bytes
          case LevelValueType.Double: break;
        }
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, ushort level, ushort value)
    {
      var lRequest = new MsgCmdLevelValueDevMaster();

      lRequest.Device = device;
      lRequest.Level = level;
      lRequest.ValueType = LevelValueType.Integer;
      lRequest.Value = value;

      var lData = device.GetBytesDPS().
        Concat(ArrayExtensions.Int16ToBigEndian(level)).
        Concat(ArrayExtensions.Int16To8Bit((byte)lRequest.ValueType)).
        Concat(ArrayExtensions.Int16ToBigEndian(lRequest.Value)).
        ToArray();

      return lRequest.Serialize(device, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    public ushort Level { get; set; }
    
    public LevelValueType ValueType { get; set; }

    public int Value { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} Device   : {1}", GetType().Name, Device);
      Logger.LogDebug(false, "{0} Level    : {1}", GetType().Name, Level);
      Logger.LogDebug(false, "{0} ValueType: {1}", GetType().Name, ValueType);
      Logger.LogDebug(false, "{0} Value    : {1}", GetType().Name, Value);
    }
  }
}
