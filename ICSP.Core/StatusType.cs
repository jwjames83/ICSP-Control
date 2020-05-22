namespace ICSP.Core
{
  public enum StatusType : ushort
  {
    /// <summary>
    /// When a device receives this status of the master all channels on 
    /// the device should be turned off, all levels should be set to zero, 
    /// and the device shall enter the “unconnected” state 
    /// (see the Device & Master CommunicationSpecification)
    /// </summary>
    Reset = 0,

    /// <summary>
    /// When a device receives this status of the master, 
    /// it must stop sending messages to the master until the master's status 
    /// becomes Normal. No channels are turned off.
    /// </summary>
    Reload = 1,

    /// <summary>
    /// Undefined
    /// </summary>
    Undefined = 2,

    /// <summary>
    /// Upon entry in to the Normal state, the device must report that it  is on-line 
    /// and any channels that are ON, or non-zero levels must be reported to the master.
    /// </summary>
    Normal = 3
  }
}
