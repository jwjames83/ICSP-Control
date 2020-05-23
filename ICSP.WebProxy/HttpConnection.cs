using System;
using System.Net.WebSockets;

using Microsoft.AspNetCore.Http;

namespace ICSP.WebProxy
{
  public class HttpConnection
  {
    public HttpConnection(HttpContext context, WebSocket socket, int socketId)
    {
      Context = context ?? throw new ArgumentNullException(nameof(context));

      Socket = socket ?? throw new ArgumentNullException(nameof(socket));

      SocketId = socketId;
    }

    public HttpContext Context { get; }

    public WebSocket Socket { get; }

    public int SocketId { get; }
  }
}
