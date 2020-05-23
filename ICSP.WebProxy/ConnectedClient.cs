using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ICSP.Core;
using ICSP.Core.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ICSP.WebProxy
{
  public class ConnectedClient : WebSocketHandler
  {
    private readonly ILogger mLogger;

    private readonly Dictionary<int, ICSPManager> mManagers = new Dictionary<int, ICSPManager>();

    public ConnectedClient(ILogger<ConnectedClient> logger, ConnectionManager connectionManager) : base(logger, connectionManager)
    {
      mLogger = logger;
    }

    public override Task OnConnected(HttpContext context, WebSocket socket)
    {
      base.OnConnected(context, socket);

      var lSocketId = ConnectionManager.GetId(socket);

      // localhost:8000 -> Device Mapping 10001
      // localhost:8001 -> Device Mapping 10002
      var lConf = GetConfig(context, socket);

      // Specific parameters by QueryString ...
      // file:///C:/Tmp/WebControl/index.html?ip=172.16.126.250&device=8002

      var lHost = context.Request.Query["host"];

      // Alias
      if(string.IsNullOrWhiteSpace(lHost))
        lHost = context.Request.Query["ip"];

      if(!string.IsNullOrWhiteSpace(lHost))
        lConf.RemoteHost = lHost;

      if(ushort.TryParse(context.Request.Query["port"], out var lPort))
      {
        if(lPort > 0)
          lConf.RemotePort = lPort;
      }

      if(ushort.TryParse(context.Request.Query["device"], out var lDevice))
      {
        if(lDevice > 0)
          lConf.Device = lDevice;
      }

      var lManager = new ICSPManager();

      mManagers.Add(lSocketId, lManager);

      lManager.BlinkMessage += OnBlinkMessage;
      lManager.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;

      try
      {
        lManager.Connect(lConf.RemoteHost, lConf.RemotePort);
      }
      catch(Exception ex)
      {
        mLogger.LogError(ex.Message);

        _ = SendAsync(lSocketId, ex.Message);
      }

      return Task.CompletedTask;
    }

    public override async Task ReceiveAsync(HttpContext context, WebSocket socket, CancellationToken cancellationToken)
    {
      try
      {
        var lSocketId = ConnectionManager.GetId(socket);

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

            // Echo text or binary data to the broadcast queue
            if(socket.State == WebSocketState.Open)
            {
              mLogger.LogDebug($"Socket[{lSocketId:00}]: Received {lReceiveResult.MessageType} frame ({lReceiveResult.Count} bytes).");

              var lMsg = System.Text.Encoding.Default.GetString(lBuffer.Array, 0, lReceiveResult.Count);

              mLogger.LogInformation($"Socket[{lSocketId:00}]: Data={lMsg}");

              await SendAsync(socket, lMsg);
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
      }
      finally
      {
        await OnDisconnected(socket);

        socket.Dispose();
      }
    }

    public override Task OnDisconnected(WebSocket socket)
    {
      var lSocketId = ConnectionManager.GetId(socket);

      if(mManagers.TryGetValue(lSocketId, out var manager))
      {
        manager.Disconnect();
      };

      mManagers.Remove(lSocketId);

      return base.OnDisconnected(socket);
    }

    private void OnClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      var lSocketId = mManagers.FirstOrDefault(p => p.Value == sender).Key;

      mLogger.LogInformation($"ICSPManager[{lSocketId:00}]: OnClientOnlineStatusChanged, ClientOnline={e.ClientOnline}");

      _ = SendAsync(lSocketId, $"ClientOnline={ e.ClientOnline}");
    }

    private void OnBlinkMessage(object sender, BlinkEventArgs e)
    {
      var lSocketId = mManagers.FirstOrDefault(p => p.Value == sender).Key;

      mLogger.LogInformation($"ICSPManager[{lSocketId:00}]: OnBlinkMessage, DateText={e.DateText}");

      _ = SendAsync(lSocketId, e.DateText);
    }

    public async Task ProcessMessage(int socketId)
    {
      try
      {
        // TODO ...
        var lManager = mManagers[socketId];
      }
      catch(Exception ex)
      {
        mLogger.LogError(ex.Message);
      }
    }

    public Configuration.ProxyDeviceConfig GetConfig(HttpContext context, WebSocket socket)
    {
      foreach(var item in Program.ProxyConfig.Devices)
      {
        if(item.LocalPort == context.Connection.LocalPort)
        {
          return item;
        }
      }

      foreach(var item in Program.ProxyConfig.Default)
      {
        if(item.LocalPort == context.Connection.LocalPort)
        {
          return new Configuration.ProxyDeviceConfig()
          {
            Device = 0,
            RemoteHost = item.RemoteHost,
            RemotePort = item.RemotePort
          };
        }
      }

      // First Default ...
      var lDefault = Program.ProxyConfig.Default[0];

      return new Configuration.ProxyDeviceConfig()
      {
        Device = 0,
        RemoteHost = lDefault.RemoteHost,
        RemotePort = lDefault.RemotePort
      };
    }
  }
}
