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

    // None = 0,   // 0000 0000

    /// <summary>
    /// If set, then message is a broadcast message
    /// </summary>
    Broadcast = 1, // 0000 0001

    /// <summary>
    /// If set, then receiver should reply to message.
    /// A new Master or non-configured device connecting to de system uses this.
    /// </summary>
    Newbee = 2,    // 0000 0010


    Unconfigured     /**/ = 4,   // 0000 0100
    Response         /**/ = 8,   // 0000 1000
    RequestResponse  /**/ = 16,  // 0001 0000
    IgnoreAddress    /**/ = 32,  // 0010 0000

    Timeout_1        /**/ = 0,   // 00 0000 0000
    Timeout_2        /**/ = 256, // 01 0000 0000
    Timeout_3        /**/ = 512, // 10 0000 0000
    Timeout_4        /**/ = 768, // 11 0000 0000

    DefaultTimeout   /**/ = 512, // 10 0000 0000

    Version_02 = 0x0200,
  }
}
