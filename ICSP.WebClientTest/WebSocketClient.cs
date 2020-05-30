using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSP.WebClientTest
{
  public class WebSocketClient
  {
    private ClientWebSocket Socket;
    private CancellationTokenSource SocketLoopTokenSource;

    public int ID;

    public event EventHandler<string> OnMessage;

    private int mTaskConnectAsyncRunning = 0;
    private int mTaskStopAsyncRunning = 0;

    public async Task StartAsync(string wsUri)
    {
      await StartAsync(new Uri(wsUri));
    }

    public async Task StartAsync(Uri wsUri)
    {
      // Ensure that method would be called only once
      if(Interlocked.Exchange(ref mTaskConnectAsyncRunning, 1) != 0)
        return;

      try
      {
        Console.WriteLine($"Connecting to server {wsUri.ToString()}");

        SocketLoopTokenSource = new CancellationTokenSource();

        try
        {
          Socket = new ClientWebSocket();

          await Socket?.ConnectAsync(wsUri, CancellationToken.None);

          if(Socket?.State == WebSocketState.Open)
            await SocketProcessingLoopAsync();
        }
        catch(OperationCanceledException)
        {
          // Normal upon task/token cancellation, disregard
        }
      }
      finally
      {
        Interlocked.Exchange(ref mTaskConnectAsyncRunning, 0);
      }
    }

    public async Task StopAsync()
    {
      // Ensure that method would be called only once
      if(Interlocked.Exchange(ref mTaskStopAsyncRunning, 1) != 0)
        return;

      try
      {
        Console.WriteLine($"Closing connection");

        if(Socket == null || Socket.State != WebSocketState.Open)
          return;

        // close the socket first, because ReceiveAsync leaves an invalid socket (state = aborted) when the token is cancelled
        var lTimeout = new CancellationTokenSource(Program.CLOSE_SOCKET_TIMEOUT_MS);

        try
        {
          // after this, the socket state which change to CloseSent
          await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing", lTimeout.Token);

          // now we wait for the server response, which will close the socket
          while(Socket.State != WebSocketState.Closed && !lTimeout.Token.IsCancellationRequested) ;
        }
        catch(OperationCanceledException)
        {
          // Normal upon task/token cancellation, disregard
        }

        // Whether we closed the socket or timed out, we cancel the token causing RecieveAsync to abort the socket
        SocketLoopTokenSource.Cancel();

        // The finally block at the end of the processing loop will dispose and null the Socket object
      }
      finally
      {
        Interlocked.Exchange(ref mTaskStopAsyncRunning, 0);
      }
    }

    public WebSocketState State
    {
      get => Socket?.State ?? WebSocketState.None;
    }

    private async Task SocketProcessingLoopAsync()
    {
      try
      {
        var lBuffer = WebSocket.CreateClientBuffer(4096, 4096);

        while(Socket.State != WebSocketState.Closed && !SocketLoopTokenSource.Token.IsCancellationRequested)
        {
          var lReceiveResult = await Socket.ReceiveAsync(lBuffer, SocketLoopTokenSource.Token);

          // If the token is cancelled while ReceiveAsync is blocking, the socket state changes to aborted and it can't be used
          if(!SocketLoopTokenSource.Token.IsCancellationRequested)
          {
            // The server is notifying us that the connection will close; send acknowledgement
            if(Socket.State == WebSocketState.CloseReceived && lReceiveResult.MessageType == WebSocketMessageType.Close)
            {
              Console.WriteLine($"\nAcknowledging Close frame received from server");

              await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Acknowledge Close frame", CancellationToken.None);
            }

            // Display text or binary data
            if(Socket.State == WebSocketState.Open && lReceiveResult.MessageType != WebSocketMessageType.Close)
            {
              var lMessage = Encoding.UTF8.GetString(lBuffer.Array, 0, lReceiveResult.Count);

              OnMessage?.Invoke(this, lMessage);

              Console.Write(lMessage);
            }
          }
        }

        Console.WriteLine($"Ending processing loop in state {Socket.State}");
      }
      catch(OperationCanceledException)
      {
        // Normal upon task/token cancellation, disregard
      }
      catch(Exception ex)
      {
        Program.ReportException(ex);
      }
      finally
      {
        Socket.Dispose();
        Socket = null;
      }
    }

    public async Task SendAsync(string message)
    {
      try
      {
        var lBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));

        await Socket.SendAsync(lBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
      }
      catch(Exception ex)
      {
        Program.ReportException(ex);
      }
    }
  }
}
