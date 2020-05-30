using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSP.WebProxy.Proxy;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ICSP.WebProxy
{
  public class WebSocketProxyClient : WebSocketHandler
  {
    private readonly ILogger mLogger;

    private readonly IServiceProvider mServiceProvider;

    public event EventHandler<MessageEventArgs> OnMessage;

    public WebSocketProxyClient(IServiceProvider provider, ILogger<WebSocketProxyClient> logger, ConnectionManager connectionManager) : base(logger, connectionManager)
    {
      mLogger = logger;

      mServiceProvider = provider;
    }

    public override async Task ReceiveAsync(HttpContext context, WebSocket socket, CancellationToken cancellationToken)
    {
      try
      {
        using var lServiceScope = mServiceProvider.CreateScope();

        using var lManager = lServiceScope.ServiceProvider.GetService<ProxyClient>();

        var lSocketId = ConnectionManager.GetId(socket);

        await lManager.InvokeAsync(context, socket, lSocketId);

        var lBuffer = WebSocket.CreateClientBuffer(4096, 4096);

        while(socket.State != WebSocketState.Closed && !cancellationToken.IsCancellationRequested)
        {
          var lReceiveResult = await socket.ReceiveAsync(lBuffer, cancellationToken);

          // If the token is cancelled while ReceiveAsync is blocking, the socket state changes to aborted and it can't be used
          if(!cancellationToken.IsCancellationRequested)
          {
            // The client is notifying us that the connection will close; send acknowledgement
            if(socket.State == WebSocketState.CloseReceived && lReceiveResult.MessageType == WebSocketMessageType.Close)
            {
              mLogger.LogInformation($"Socket[{lSocketId:00}]: Close received from client");

              await OnDisconnected(socket);

              // The socket state changes to closed at this point
            }

            if(socket.State == WebSocketState.Open)
            {
              var lMsg = Encoding.Default.GetString(lBuffer.Array, 0, lReceiveResult.Count);

              OnMessage?.Invoke(this, new MessageEventArgs(lMsg, lSocketId));
            }
          }
        }

        mLogger.LogInformation($"Socket[{lSocketId:00}]: Ending processing loop in state {socket.State}");
      }
      catch(OperationCanceledException)
      {
        // Normal upon task/token cancellation, disregard
      }
      catch(Exception ex)
      {
        mLogger.LogError(ex.Message);

        if(socket.State == WebSocketState.Open)
        {
          var lMsg = $"Error: {ex.Message}";

          await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(lMsg)), WebSocketMessageType.Text, true, CancellationToken.None);
        }
      }
      finally
      {
        await OnDisconnected(socket);

        socket.Dispose();
      }
    }
  }
}
