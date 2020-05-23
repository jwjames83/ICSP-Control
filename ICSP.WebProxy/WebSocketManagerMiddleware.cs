using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ICSP.WebProxy
{
  public class WebSocketManagerMiddleware
  {
    private readonly RequestDelegate mNext;
    
    public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
    {
      mNext = next;

      mWebSocketHandler = webSocketHandler;
    }

    private WebSocketHandler mWebSocketHandler { get; set; }

    public async Task Invoke(HttpContext context)
    {
      if(!context.WebSockets.IsWebSocketRequest)
        return;

      var lWebSocket = await context.WebSockets.AcceptWebSocketAsync();

      await mWebSocketHandler.OnConnected(context, lWebSocket);

      await mWebSocketHandler.ReceiveAsync(context, lWebSocket);
    }
  }
}
