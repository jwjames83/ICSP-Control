using System;

namespace ICSP.Core
{
  /// <summary>
  /// Flag (Version, Type)<br/>
  /// Can be one of two types of flags.<br/>
  /// One is a broadcast flag. The broadcast flag will send a broadcast message to all devices on a network.<br/>
  /// A newbie flag is placed when a device is added to the network.<br/>
  /// This will then cause a response from the master indicating that it received the message from the newbie device.
  /// </summary>
  [Flags]
  public enum ICSPMsgFlag
  {
    // Flags: 0x0200 (512) - 0010 | 0000 0000
    // Flags: 0x0201 (513) - 0010 | 0000 0001
    // Flags: 0x0208 (520) - 0010 | 0000 1000
    // Flags: 0x0208 (530) - 0010 | 0001 0010

    // MsgCmdBlinkMessage                 - 513 (Broadcast)
    // MsgCmdPingRequest                  - 512 (Default)

    // MsgCmdAck                          - 520 (???)
    // FileTransfer                       - 520 (???) - Ready ?

    // MsgCmdDeviceInfo create ->         - 530 (???, Newbee)
    // MsgCmdDeviceInfo                   - 512 (Default)
    // MsgCmdDynamicDeviceAddressResponse - 512 (Default)
    // MsgCmdFileTransfer                 - 512 (Default)
    // MsgCmdPortCountBy                  - 512 (Default)
    // MsgCmdRequestDevicesOnlineEOT      - 512 (Default)

    None = 0,     // 0000 0000

    /// <summary>
    /// If set, then message is a broadcast message
    /// </summary>
    Broadcast = 1, // 0000 0001

    /// <summary>
    /// If set, then receiver should reply to message.
    /// A new Master or non-configured device connecting to de system uses this.
    /// </summary>
    Newbee = 2,    // 0000 0010

    Version_02 = 0x0200,
  }
}
