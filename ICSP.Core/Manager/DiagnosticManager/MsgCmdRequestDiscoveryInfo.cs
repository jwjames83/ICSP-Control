
using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DiagnosticManager
{
  /// <summary>
  /// Unknown: (Probably IP Device Discovery -> Info)
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.RequestDiscoveryInfo)]
  public class MsgCmdRequestDiscoveryInfo : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.RequestDiscoveryInfo;

    private MsgCmdRequestDiscoveryInfo()
    {
    }

    public MsgCmdRequestDiscoveryInfo(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 2)
        Unknown = Data.GetBigEndianInt16(0);
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestDiscoveryInfo(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice source, ushort unknown)
    {
      var lDest = new AmxDevice(0, 1, source.System);

      var lRequest = new MsgCmdRequestDiscoveryInfo
      {
        Unknown = unknown
      };

      var lData = ArrayExtensions.Int16ToBigEndian(unknown);

      return lRequest.Serialize(lDest, source, MsgCmd, lData);
    }
    
    public ushort Unknown { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Unknown: {0}", GetType().Name, Unknown);
    }
  }
}