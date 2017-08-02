using System;
using System.Net;

namespace ICSP.Client
{
  public sealed class ClientConnectedEventArgs : EventArgs
  {
    public ClientConnectedEventArgs(IPAddress ipAddress)
    {
      IpAddress = ipAddress;
    }

    public IPAddress IpAddress { get; private set; }
  }
}
