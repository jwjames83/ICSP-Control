using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ICSP.WebProxy
{
  public class NotificationsMessageHandler : WebSocketHandler
  {
    private int mPort;

    public NotificationsMessageHandler(ConnectionManager connectionManager) : base(connectionManager)
    {
      Console.WriteLine();
    }

    public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
      var lMsg = System.Text.Encoding.Default.GetString(buffer, 0, result.Count);
      
      Console.WriteLine(lMsg);

      await SendMessageAsync(socket, "Trallala");

      await SendMessageToAllAsync("Trallala To All!");
    }
  }
}
