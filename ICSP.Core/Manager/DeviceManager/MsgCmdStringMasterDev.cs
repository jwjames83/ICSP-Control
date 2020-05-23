﻿using ICSP.Core.Constants;

namespace ICSP.Core.Manager.DeviceManager
{
  /// <summary>
  /// The String message is generated by the master to communicate a String.
  /// The format of a String is similar to a “C Language” string, however, 
  /// the semantics are different. A String in a control system context is used to generate a “control” message.
  /// This “control” message could cause a laser disc player to begin playing a disc, display a message to the user of the system, 
  /// or any number of any other uses. The string will be converted, as necessary, to any format that the device supports as determined by the StringSize message.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.StringMasterDev)]
  public class MsgCmdStringMasterDev : MsgBaseCmdText<MsgCmdStringMasterDev>
  {
    private MsgCmdStringMasterDev()
    {
    }

    public MsgCmdStringMasterDev(byte[] buffer) : base(buffer)
    {
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdStringMasterDev(bytes);
    }

    protected override ushort MsgCmd
    {
      get
      {
        return DeviceManagerCmd.StringMasterDev;
      }
    }
  }
}