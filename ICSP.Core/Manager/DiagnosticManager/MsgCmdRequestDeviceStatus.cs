﻿using System.Linq;

using ICSP.Core.Constants;
using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DiagnosticManager
{
  /// <summary>
  /// This message is used by the IDE to request the status of the specified device.
  /// The master responds with Output ON messages for each output channel that is on, 
  /// Feedback ON messages for each feedback channel that is on, etc.
  /// See the Diagnostic Manager specification for more information.
  /// </summary>
  [MsgCmd(DiagnosticManagerCmd.RequestDeviceStatus)]
  public class MsgCmdRequestDeviceStatus : ICSPMsg
  {
    public const int MsgCmd = DiagnosticManagerCmd.RequestDeviceStatus;

    private MsgCmdRequestDeviceStatus()
    {
    }

    public MsgCmdRequestDeviceStatus(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
        Device = AmxDevice.FromDPS(Data.Range(0, 6));
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdRequestDeviceStatus(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source)
    {
      var lRequest = new MsgCmdRequestDeviceStatus
      {
        Device = dest
      };

      var lData = dest.GetBytesDPS().ToArray();

      return lRequest.Serialize(AmxDevice.Empty, source, MsgCmd, lData);
    }
    
    public AmxDevice Device { get; set; }
    
    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device: {0:l}", GetType().Name, Device);
    }
  }
}