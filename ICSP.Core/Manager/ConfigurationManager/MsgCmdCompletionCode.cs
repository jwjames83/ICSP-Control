using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.ConfigurationManager
{
  /// <summary>
  /// Indicates Successful/unsuccessful completion of a previous message.
  /// In the case where the device settings, such as device number, are being modified,
  /// the completion code message is generate using the "old" device information before the new settings take effect.
  /// </summary>
  [MsgCmd(ConfigurationManagerCmd.CompletionCode)]
  public class MsgCmdCompletionCode : ICSPMsg
  {
    /// Bitfield indicating success or failure of the specified message
    /// Bit 0: 0 = Failed, 1 = success
    /// Bits 1-7 must be zero.
    public const byte StatusFailed  /**/ = 0b_0_0000000;
    public const byte StatusSuccess /**/ = 0b_1_0000000;

    public const int MsgCmd = ConfigurationManagerCmd.CompletionCode;

    private MsgCmdCompletionCode()
    {
    }

    public MsgCmdCompletionCode(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        // Message
        Message = Data.GetBigEndianInt16(0);

        // Status
        Status = Data[2];
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdCompletionCode(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, ushort message, bool success)
    {
      var lRequest = new MsgCmdCompletionCode
      {
        Message = message,

        Status = success ? StatusSuccess : StatusFailed
      };

      var lData = 
        ArrayExtensions.Int16ToBigEndian(message)
        .Concat(new byte[] { lRequest.Status }).ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Message: 0x{1:X2} ({2:l})", GetType().Name, Message, ICSPMsg.GetFrindlyName(Message));
      Logger.LogDebug(false, "{0:l} Status : 0x{1:X2}", GetType().Name, Status);
    }

    #region Properties

    /// <summary>
    /// The message code (not Message ID) of the message (command) that is completed.
    /// For example, when a Set Ethernet IP Address message has been processed by a device,
    /// the device responds with a Completion Code message with wMessage set to 0x020C.
    /// </summary>
    public ushort Message { get; private set; }

    /// <summary>
    /// Bitfield indicating success or failure of the specified message:
    /// Bit 0: 0 = Failed, 1 = success
    /// Bits 1-7 must be zero.
    /// </summary>
    public byte Status { get; private set; }

    #endregion
  }
}
