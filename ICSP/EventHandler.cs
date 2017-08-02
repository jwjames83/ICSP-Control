namespace ICSP
{
  public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

  public delegate void ChannelEventHandler(object sender, ChannelEventArgs e);

  public delegate void DeviceInfoEventHandler(object sender, DeviceInfoEventArgs e);

  public delegate void PortCountEventHandler(object sender, PortCountEventArgs e);

  public delegate void DynamicDeviceCreatedEventHandler(object sender, DynamicDeviceCreatedArgs e);
}
