using System.Linq;
using System.Text;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DiagnosticManager
{
  [MsgCmd(DiagnosticManagerCmd.InternalDiagnosticString)]
  public class MsgCmdInternalDiagnosticString : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.InternalDiagnosticString;

    private MsgCmdInternalDiagnosticString()
    {
    }

    public MsgCmdInternalDiagnosticString(ICSPMsgData msg) : base(msg)
    {
      var lOffset = 0;

      if(msg.Data.Length > 0)
      {
        ObjectId = msg.Data.GetBigEndianInt16(0);

        Severity = msg.Data.GetBigEndianInt16(2);

        lOffset = 4;

        Text = AmxUtils.GetNullStr(msg.Data, ref lOffset);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, ushort objectId, ushort severity, string text)
    {
      var lRequest = new MsgCmdInternalDiagnosticString();

      lRequest.ObjectId = objectId;
      lRequest.Severity = severity;
      lRequest.Text = text;

      var lBytes = Encoding.Default.GetBytes(lRequest.Text + "\0");
      
      var lData = ArrayExtensions.Int16ToBigEndian(lRequest.ObjectId).
        Concat(ArrayExtensions.Int16ToBigEndian(lRequest.Severity)).
        Concat(lBytes).
        ToArray();

      return lRequest.Serialize(device, source, MsgCmd, lData);
    }

    /// <summary>
    /// Unsigned 16-bit value. Number of characters in string
    /// Values defined in the Constants & IDs specification document.
    /// </summary>
    public ushort ObjectId { get; set; }

    /// <summary>
    ///  Unsigned 16-bit value.
    /// </summary>
    public ushort Severity { get; set; }

    /// <summary>
    /// Containing text description of the error
    /// </summary>
    public string Text { get; private set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0} ObjectId: {1}", GetType().Name, ObjectId);
      Logger.LogDebug(false, "{0} Severity: {1}", GetType().Name, Severity);
      Logger.LogDebug(false, "{0} Text    : {1}", GetType().Name, Text);
    }
  }
}