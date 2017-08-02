using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
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

    public MsgCmdRequestLevelSize(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(msg.Data.Range(0, 6));

        Level = msg.Data.GetBigEndianInt16(6);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, ushort level)
    {
      var lRequest = new MsgCmdRequestLevelSize();

      lRequest.Device = device;
      lRequest.Level = level;

      var lData = device.GetBytesDPS().
        Concat(ArrayExtensions.Int16ToBigEndian(level)).
        ToArray();
      
      return lRequest.Serialize(device, source, MsgCmd, lData);
    }

    public AmxDevice Device { get; set; }

    public ushort Level { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} Device: {1}", GetType().Name, Device);
      Logger.LogDebug(false, "{0} Level : {1}", GetType().Name, Level);
    }
  }
}
