using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
{
  /// <summary>
  /// This message requests a list of data types supported for the specified level. 
  /// The initial assumption that the master makes is that each device/port/level 
  /// only supports 8-bit unsigned values (BYTE Type 0x10).
  /// </summary>
  [MsgCmd(DeviceManagerCmd.RequestLevelSize)]
  public class MsgCmdRequestLevelSize : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.RequestLevelSize;

    private MsgCmdRequestLevelSize()
    {
    }

    public MsgCmdRequestLevelSize(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(Data.Range(0, 6));

        Level = Data.GetBigEndianInt16(6);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestLevelSize(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, ushort level)
    {
      var lRequest = new MsgCmdRequestLevelSize
      {
        Device = device,
        Level = level
      };

      var lData = device.GetBytesDPS().
        Concat(ArrayExtensions.Int16ToBigEndian(level)).
        ToArray();
      
      return lRequest.Serialize(device, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    public ushort Level { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {1:l}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} Level : {1}", GetType().Name, Level);
    }
  }
}
