using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
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

    public MsgCmdLevelValueDevMaster(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(Data.Range(0, 6));

        Level = Data.GetBigEndianInt16(6);

        ValueType = (LevelValueType)Data[8];

        switch(ValueType)
        {
          // 1 Data
          case LevelValueType.Byte: Value = Data[9]; break;
          case LevelValueType.Char: Value = Data[9]; break;

          // 2 Data
          case LevelValueType.Integer: Value = Data.GetBigEndianInt16(9); break;
          case LevelValueType.SInteger: Value = Data.GetBigEndianInt16(9); break;

          // 4 Data
          case LevelValueType.ULong: Value = Data.GetBigEndianInt32(9); break;
          case LevelValueType.Long: Value = Data.GetBigEndianInt32(9); break;
          case LevelValueType.Float: Value = Data.GetBigEndianInt32(9); break;

          // 8 Data
          case LevelValueType.Double: break;
        }
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdLevelValueDevMaster(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, ushort level, ushort value)
    {
      var lRequest = new MsgCmdLevelValueDevMaster
      {
        Device = source,
        Level = level,
        ValueType = LevelValueType.Integer,
        Value = value
      };

      var lData = source.GetBytesDPS().
        Concat(ArrayExtensions.Int16ToBigEndian(level)).
        Concat(ArrayExtensions.Int16To8Bit((byte)lRequest.ValueType)).
        Concat(ArrayExtensions.Int16ToBigEndian((ushort)lRequest.Value)).
        ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    public ushort Level { get; set; }
    
    public LevelValueType ValueType { get; set; }

    public int Value { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device   : {1:l}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} Level    : {1}", GetType().Name, Level);
      Logger.LogDebug(false, "{0:l} ValueType: {1:l}", GetType().Name, ValueType);
      Logger.LogDebug(false, "{0:l} Value    : {1}", GetType().Name, Value);
    }
  }
}
