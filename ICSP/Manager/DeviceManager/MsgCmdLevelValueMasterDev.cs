﻿using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is used to force a level value change from the master.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.LevelValueMasterDev)]
  public class MsgCmdLevelValueMasterDev : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.LevelValueMasterDev;

    private MsgCmdLevelValueMasterDev()
    {
    }

    public MsgCmdLevelValueMasterDev(ICSPMsgData msg) : base(msg)
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
      var lRequest = new MsgCmdLevelValueMasterDev
      {
        Device = device,
        Level = level,
        ValueType = LevelValueType.Integer,
        Value = value
      };

      var lData = device.GetBytesDPS().
        Concat(ArrayExtensions.Int16ToBigEndian(level)).
        Concat(ArrayExtensions.Int16To8Bit((byte)lRequest.ValueType)).
        Concat(ArrayExtensions.Int16ToBigEndian((ushort)lRequest.Value)).
        ToArray();

      return lRequest.Serialize(device, source, MsgCmd, lData);
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