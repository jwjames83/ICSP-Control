namespace ICSP.Core
{
  public enum StatusType : ushort
  {
    /// <summary>
    /// When a device receives this status of the master all channels on<br/>
    /// the device should be turned off, all levels should be set to zero,<br/>
    /// and the device shall enter the "unconnected" state<br/>
    /// (see the Device and Master CommunicationSpecification)
    /// </summary>
    Reset = 0,

    /// <summary>
    /// When a device receives this status of the master,<br/>
    /// it must stop sending messages to the master until the master's status<br/>
    /// becomes Normal. No channels are turned off.
    /// </summary>
    Reload = 1,

    /// <summary>
    /// Undefined
    /// </summary>
    Undefined = 2,

    /// <summary>
    /// Upon entry in to the Normal state, the device must report that it  is on-line<br/>
    /// and any channels that are ON, or non-zero levels must be reported to the master.<br/>
    /// </summary>
    Normal = 3
  }
}
