using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ICSP.WebProxy
{
  public abstract class WebSocketHandler
  {
    private readonly ILogger mLogger;

    private CancellationTokenSource mReceiveAsyncCancellationTokenSource;

    public WebSocketHandler(ILogger<WebSocketHandler> logger, ConnectionManager connectionManager)
    {
      mLogger = logger;

      ConnectionManager = connectionManager;

      mReceiveAsyncCancellationTokenSource = new CancellationTokenSource();
    }

    protected ConnectionManager ConnectionManager { get; set; }

    public virtual Task OnConnected(HttpContext context, WebSocket socket)
    {
      var lSocketId = ConnectionManager.AddSocket(socket);

      mLogger.LogInformation($"Socket[{lSocketId:00}]: New connection. Port={context.Connection.LocalPort}, Path={context.Request.Path}, QueryString={context.Request.QueryString}");

      return Task.CompletedTask;
    }

    public virtual async Task OnDisconnected(WebSocket socket)
    {
      await ConnectionManager.RemoveSocket(ConnectionManager.GetId(socket));
    }

    public virtual Task ReceiveAsync(HttpContext context, WebSocket socket)
    {
      return ReceiveAsync(context, socket, mReceiveAsyncCancellationTokenSource.Token);
    }

    public abstract Task ReceiveAsync(HttpContext context, WebSocket socket, CancellationToken token);

    public async Task SendAsync(WebSocket socket, string message)
    {
      if(socket.State != WebSocketState.Open)
        return;

      await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task SendAsync(int socketId, string message)
    {
      await SendAsync(ConnectionManager.GetSocketById(socketId), message);
    }

    public async Task SendToAllAsync(string message)
    {
      foreach(var pair in ConnectionManager.GetAll())
      {
        if(pair.Value.State == WebSocketState.Open)
          await SendAsync(pair.Value, message);
      }
    }

    public async Task CloseSocketAsync(int socketId)
    {
      var lSocket = ConnectionManager.GetSocketById(socketId);

      // We can't dispose the sockets until the processing loops are terminated,
      // but terminating the loops will abort the sockets, preventing graceful closing.

      Console.WriteLine($"Closing Socket {socketId}");
      Console.WriteLine("... ending broadcast loop");

      if(lSocket == null)
        return;

      if(lSocket.State != WebSocketState.Open)
      {
        Console.WriteLine($"... socket not open, state = {lSocket.State}");
      }
      else
      {
        var lTimeout = new CancellationTokenSource(Program.CLOSE_SOCKET_TIMEOUT_MS);

        try
        {
          Console.WriteLine("... starting close handshake");

          await lSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", lTimeout.Token);
        }
        catch(OperationCanceledException ex)
        {
          mLogger.LogError(ex.Message);

          // Normal upon task/token cancellation, disregard
        }
      }

      // Now that they're all closed, terminate the blocking ReceiveAsync calls in the SocketProcessingLoop threads
      mReceiveAsyncCancellationTokenSource.Cancel();

      // Dispose resources
      lSocket.Dispose();
    }
  }
}
