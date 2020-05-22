using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using ICSP.Core;
using ICSP.Core.Manager.DeviceManager;

namespace ICSP.WsProxy
{
  public class ConnectedClient
  {
    public event EventHandler<string> OnMessage;

    private CancellationTokenSource SocketLoopTokenSource;

    public ConnectedClient(int socketId, WebSocketContext context)
    {
      SocketId = socketId;

      Context = context;

      ICSPManager = new ICSPManager();

      // TODO: ...
      // var lDevInfo = new DeviceInfoData(11011, 0, ICSPManager.CurrentRemoteIpAddress);

      ICSPManager.BlinkMessage += ICSPManager_BlinkMessage;

      //ICSPManager.CreateDeviceInfo(lDevInfo);

      SocketLoopTokenSource = new CancellationTokenSource();
    }

    private void ICSPManager_BlinkMessage(object sender, BlinkEventArgs e)
    {
      Console.WriteLine(e.DateTime);
    }

    public int SocketId { get; private set; }

    public WebSocketContext Context { get; set; }

    public WebSocket Socket { get => Context?.WebSocket; }

    public ICSPManager ICSPManager { get; private set; }

    public async Task StartReceiveAsync()
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
            // The client is notifying us that the connection will close; send acknowledgement
            if(Socket.State == WebSocketState.CloseReceived && lReceiveResult.MessageType == WebSocketMessageType.Close)
            {
              Console.WriteLine($"Socket {SocketId}: RequestUri={Context.RequestUri}");
              Console.WriteLine($"Socket {SocketId}: Acknowledging Close frame received from client");

              await Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Acknowledge Close frame", CancellationToken.None);

              // The socket state changes to closed at this point
            }

            // Echo text or binary data to the broadcast queue
            if(Socket.State == WebSocketState.Open)
            {
              Console.WriteLine($"Socket {SocketId}: Received {lReceiveResult.MessageType} frame ({lReceiveResult.Count} bytes).");
              Console.WriteLine($"Socket {SocketId}: Echoing data to client.");

              await Socket.SendAsync(new ArraySegment<byte>(lBuffer.Array, 0, lReceiveResult.Count), lReceiveResult.MessageType, lReceiveResult.EndOfMessage, CancellationToken.None);
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
      }
    }
  }
}
