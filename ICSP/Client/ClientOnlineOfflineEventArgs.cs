using System;

namespace ICSP.Client
{
  public sealed class ClientOnlineOfflineEventArgs : EventArgs
  {
    public ClientOnlineOfflineEventArgs(int clientId, bool clientOnline, string ipAddress)
    {
      ClientId = clientId;

      ClientOnline = clientOnline;

      IpAddress = ipAddress;
    }

    public int ClientId { get; }

    public bool ClientOnline { get; }

    public string IpAddress { get; }
  }
}
