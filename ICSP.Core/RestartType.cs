namespace ICSP.Core
{
  public enum RestartType : ushort
  {
    /// <summary>
    /// 0: Device reboot.
    /// The device should perform the “coldest” boot possible.
    /// </summary>
    DeviceReboot = 0,

    /// <summary>
    ///  1: NetLinx restart.
    /// Shutdown and restart the NetLinx interpreter.
    /// </summary>
    NetLinxRestart = 1,

    /// <summary>
    /// 65535: Smart reboot.
    /// The device should perform the necessary steps to activate any previous configuration parameters.
    /// Including, but not limited to, “cold” rebooting the device.
    /// For example, if the IP address configuration has changed, and a reboot is 
    /// required to begin using the new IP, then the device should reboot.
    /// </summary>
    SmartReboot = 65535
  }
}
