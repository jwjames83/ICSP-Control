using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSP.WsProxy.Properties;

namespace ICSP.WsProxy
{
  public static class WebSocketServer
  {
    // note that Microsoft plans to deprecate HttpListener,
    // and for .NET Core they don't even support SSL/TLS
    // https://github.com/dotnet/platform-compat/issues/88

    private static HttpListener Listener;

    private static CancellationTokenSource SocketLoopTokenSource;
    private static CancellationTokenSource ListenerLoopTokenSource;

    private static int SocketCounter = 0;

    private static bool ServerIsRunning = true;

    // The key is a socket id
    private static readonly ConcurrentDictionary<int, ConnectedClient> Clients = new ConcurrentDictionary<int, ConnectedClient>();

    private static readonly ReadOnlyMemory<byte> HtmlPage = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(Resources.HTML));

    public static void Start(string uriPrefix)
    {
      SocketLoopTokenSource = new CancellationTokenSource();
      ListenerLoopTokenSource = new CancellationTokenSource();

      Listener = new HttpListener();

      Listener.Prefixes.Add(uriPrefix);
      Listener.Start();

      if(Listener.IsListening)
      {
        Console.WriteLine("Connect browser for a basic echo-back web page.");
        Console.WriteLine($"Server listening: {uriPrefix}");

        // Listen on a separate thread so that Listener.Stop can interrupt GetContextAsync
        Task.Run(() => ListenerProcessingLoopAsync().ConfigureAwait(false));
      }
      else
      {
        Console.WriteLine("Server failed to start.");
      }
    }

    public static async Task StopAsync()
    {
      if(Listener?.IsListening ?? false && ServerIsRunning)
      {
        Console.WriteLine("\nServer is stopping.");

        // prevent new connections during shutdown
        ServerIsRunning = false;

        // Also cancels processing loop tokens (abort ReceiveAsync)
        await CloseAllSocketsAsync();

        // Safe to stop now that sockets are closed
        ListenerLoopTokenSource.Cancel();
        Listener.Stop();
        Listener.Close();
      }
    }

    private static async Task ListenerProcessingLoopAsync()
    {
      var lCancellationToken = ListenerLoopTokenSource.Token;

      try
      {
        while(!lCancellationToken.IsCancellationRequested)
        {
          // Wait for an incoming request
          var lContext = await Listener.GetContextAsync();

          if(ServerIsRunning)
          {
            // HTTP is only the initial connection; upgrade to a client-specific websocket
            if(lContext.Request.IsWebSocketRequest)
            {
              try
              {
                var lWsContext = await lContext.AcceptWebSocketAsync(subProtocol: null);

                var lSocketId = Interlocked.Increment(ref SocketCounter);

                var lClient = new ConnectedClient(lSocketId, lWsContext);

                Clients.TryAdd(lSocketId, lClient);

                Console.WriteLine($"Socket {lSocketId}: New connection. RequestUri={lWsContext.RequestUri}");

                _ = Task.Run(() => lClient.StartReceiveAsync().ConfigureAwait(false));
              }
              catch(Exception)
              {
                // Server error if upgrade from HTTP to WebSocket fails
                lContext.Response.StatusCode = 500;
                lContext.Response.StatusDescription = "WebSocket upgrade failed";
                lContext.Response.Close();

                return;
              }
            }
            else
            {
              if(lContext.Request.AcceptTypes.Contains("text/html"))
              {
                Console.WriteLine("Sending HTML to client.");

                lContext.Response.ContentType = "text/html; charset=utf-8";
                lContext.Response.StatusCode = 200;
                lContext.Response.StatusDescription = "OK";
                lContext.Response.ContentLength64 = HtmlPage.Length;

                await lContext.Response.OutputStream.WriteAsync(HtmlPage, CancellationToken.None);
                await lContext.Response.OutputStream.FlushAsync(CancellationToken.None);
              }
              else
              {
                lContext.Response.StatusCode = 400;
              }

              lContext.Response.Close();
            }
          }
          else
          {
            // HTTP 409 Conflict (with server's current state)
            lContext.Response.StatusCode = 409;
            lContext.Response.StatusDescription = "Server is shutting down";
            lContext.Response.Close();

            return;
          }
        }
      }
      catch(HttpListenerException ex) when(ServerIsRunning)
      {
        Program.ReportException(ex);
      }
    }

    /*
    private static async Task SocketProcessingLoopAsync(ConnectedClient client)
    {
      var lSocket = client.Socket;
      var lLoopToken = SocketLoopTokenSource.Token;

      try
      {
        var lBuffer = WebSocket.CreateServerBuffer(4096);

        while(lSocket.State != WebSocketState.Closed && lSocket.State != WebSocketState.Aborted && !lLoopToken.IsCancellationRequested)
        {
          var lReceiveResult = await client.Socket.ReceiveAsync(lBuffer, lLoopToken);

          // If the token is cancelled while ReceiveAsync is blocking, the socket state changes to aborted and it can't be used
          if(!lLoopToken.IsCancellationRequested)
          {
            // The client is notifying us that the connection will close; send acknowledgement
            if(client.Socket.State == WebSocketState.CloseReceived && lReceiveResult.MessageType == WebSocketMessageType.Close)
            {
              Console.WriteLine($"Socket {client.Context.RequestUri}: RequestUri");
              Console.WriteLine($"Socket {client.SocketId}: Acknowledging Close frame received from client");              

              await lSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Acknowledge Close frame", CancellationToken.None);

              // The socket state changes to closed at this point
            }

            // Echo text or binary data to the broadcast queue
            if(client.Socket.State == WebSocketState.Open)
            {
              Console.WriteLine($"Socket {client.SocketId}: Received {lReceiveResult.MessageType} frame ({lReceiveResult.Count} bytes).");
              Console.WriteLine($"Socket {client.SocketId}: Echoing data to client.");

              await lSocket.SendAsync(new ArraySegment<byte>(lBuffer.Array, 0, lReceiveResult.Count), lReceiveResult.MessageType, lReceiveResult.EndOfMessage, CancellationToken.None);
            }
          }
        }
      }
      catch(OperationCanceledException)
      {
        // Normal upon task/token cancellation, disregard
      }
      catch(Exception ex)
      {
        Console.WriteLine($"Socket {client.SocketId}:");

        Program.ReportException(ex);
      }
      finally
      {
        Console.WriteLine($"Socket {client.SocketId}: Ended processing loop in state {lSocket.State}");

        // Don't leave the socket in any potentially connected state
        if(client.Socket.State != WebSocketState.Closed)
          client.Socket.Abort();

        // By this point the socket is closed or aborted, the ConnectedClient object is useless
        if(Clients.TryRemove(client.SocketId, out _))
          lSocket.Dispose();
      }
    }
    */

    private static async Task CloseAllSocketsAsync()
    {
      // We can't dispose the sockets until the processing loops are terminated,
      // but terminating the loops will abort the sockets, preventing graceful closing.
      var lDisposeQueue = new List<WebSocket>(Clients.Count);

      while(Clients.Count > 0)
      {
        var lClient = Clients.ElementAt(0).Value;

        Console.WriteLine($"Closing Socket {lClient.SocketId}");
        Console.WriteLine("... ending broadcast loop");

        if(lClient.Socket.State != WebSocketState.Open)
        {
          Console.WriteLine($"... socket not open, state = {lClient.Socket.State}");
        }
        else
        {
          var lTimeout = new CancellationTokenSource(Program.CLOSE_SOCKET_TIMEOUT_MS);

          try
          {
            Console.WriteLine("... starting close handshake");

            await lClient.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", lTimeout.Token);
          }
          catch(OperationCanceledException ex)
          {
            Program.ReportException(ex);

            // Normal upon task/token cancellation, disregard
          }
        }

        if(Clients.TryRemove(lClient.SocketId, out _))
        {
          // Only safe to Dispose once, so only add it if this loop can't process it again
          lDisposeQueue.Add(lClient.Socket);
        }

        Console.WriteLine("... done");
      }

      // Now that they're all closed, terminate the blocking ReceiveAsync calls in the SocketProcessingLoop threads
      SocketLoopTokenSource.Cancel();

      // Dispose all resources
      foreach(var socket in lDisposeQueue)
        socket.Dispose();
    }
  }
}