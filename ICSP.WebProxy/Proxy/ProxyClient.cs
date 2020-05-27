using System;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core;
using ICSP.Core.Client;
using ICSP.Core.Environment;
using ICSP.Core.Manager.DeviceManager;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Converter;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ICSP.WebProxy.Proxy
{
  public class ProxyClient : IDisposable
  {
    private readonly ILogger mLogger;

    private WebSocketProxyClient mConnectedClient;

    private bool mIsDisposed;

    private Timer mConnectionTimer;

    public ProxyClient(ILogger<ProxyClient> logger, WebSocketProxyClient connectedClient)
    {
      mLogger = logger;

      mConnectedClient = connectedClient;

      mConnectedClient.OnMessage += OnWebMessage;
    }

    public HttpContext Context { get; private set; }

    public WebSocket Socket { get; private set; }

    public int SocketId { get; private set; }

    public ICSPManager Manager { get; private set; }

    public ProxyConnectionConfig DeviceConfig { get; private set; }

    public IMessageConverter Converter { get; private set; }

    public void Dispose()
    {
      Dispose(true);

      // GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if(!mIsDisposed)
      {
        if(disposing)
        {
          // Verwalteten Zustand (verwaltete Objekte) entsorgen
          StopConnectionTimer();

          Manager?.Disconnect();
          Manager?.Dispose();
          Manager = null;

          if(mConnectedClient != null)
          {
            mConnectedClient.OnMessage -= OnWebMessage;
            mConnectedClient = null;
          }
        }

        mIsDisposed = true;
      }
    }

    public async Task InvokeAsync(HttpContext context, WebSocket socket, int socketId)
    {
      Context = context;
      Socket = socket;
      SocketId = socketId;

      // localhost:8000 -> Device Mapping 10001
      // localhost:8001 -> Device Mapping 10002
      DeviceConfig = ProxyConfigManager.GetConfig(context, socket);

      // TODO: Get type of Translator from ProxyConfig ...
      Converter = MessageConverterFactory.GetConverter(DeviceConfig.Converter);

      Converter.Device = DeviceConfig.Device;
      Converter.System = DeviceConfig.Device;

      Converter.Dest = new AmxDevice(0, 1, 0);

      // Specific parameters by QueryString ...
      // file:///C:/Tmp/WebControl/index.html?ip=172.16.126.250&device=8002

      var lHost = context.Request.Query["host"];

      // Alias
      if(string.IsNullOrWhiteSpace(lHost))
        lHost = context.Request.Query["ip"];

      if(!string.IsNullOrWhiteSpace(lHost))
        DeviceConfig.RemoteHost = lHost;

      if(ushort.TryParse(context.Request.Query["port"], out var lPort))
      {
        if(lPort > 0)
          DeviceConfig.RemotePort = lPort;
      }

      if(ushort.TryParse(context.Request.Query["device"], out var lDevice))
      {
        if(lDevice > 0)
          DeviceConfig.Device = lDevice;
      }

      Manager = new ICSPManager();

      Manager.BlinkMessage += OnBlinkMessageAsync;
      Manager.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
      Manager.DeviceOnline += OnManagerDeviceOnline;
      Manager.DeviceOffline += OnManagerDeviceOffline;

      Manager.ChannelEvent += OnManagerChannelEvent;
      Manager.StringEvent += OnManagerStringEvent;
      Manager.CommandEvent += OnManagerCommandEvent;
      Manager.LevelEvent += OnManagerLevelEvent;

      // Start monitoring auto reconnect
      StartConnectionTimer();

      try
      {
        LogInformation($"Try connect, Host={DeviceConfig.RemoteHost}, Port={DeviceConfig.RemotePort}");

        await Manager.ConnectAsync(DeviceConfig.RemoteHost, DeviceConfig.RemotePort);
      }
      catch(Exception ex)
      {
        LogError(ex.Message);

        await SendAsync(ex.Message);
      }

      await Task.CompletedTask;
    }

    private async void OnWebMessage(object sender, MessageEventArgs e)
    {
      if(e.SocketId != SocketId)
        return;

      LogInformation($"Message={e.Message}");

      var lMsg = Converter.ToDevMessage(e.Message);

      if(lMsg != null)
        Manager.Send(lMsg);

      // Echo ...
      await SendAsync("Echo: " + e.Message);
    }

    #region Manager events

    private async void OnClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      if(e.ClientOnline)
      {
        StopConnectionTimer();

        if(Manager != null && DeviceConfig.Device > 0)
        {
          var lDeviceInfo = new DeviceInfoData(DeviceConfig.Device, Manager.CurrentLocalIpAddress);

          if(!string.IsNullOrWhiteSpace(DeviceConfig.DeviceName))
            lDeviceInfo.Name = DeviceConfig.DeviceName;

          Manager?.CreateDeviceInfo(lDeviceInfo, DeviceConfig.PortCount);

          Converter.Dest = new AmxDevice(0, 1, Manager.CurrentSystem);
        }
      }
      else
      {
        StartConnectionTimer();
      }

      LogInformation($"ClientOnline={e.ClientOnline}");

      await SendAsync($"ClientOnline={ e.ClientOnline}");
    }

    private void OnManagerDeviceOnline(object sender, DeviceInfoData e)
    {
      LogInformation($"Device={e.Device}, System={e.System}, Name={e.Name}");

      Converter.System = e.System;

      Converter.Dest = new AmxDevice(0, 1, e.System);
    }

    private void OnManagerDeviceOffline(object sender, DeviceInfoData e)
    {
      LogInformation($"Device={e.Device}, System={e.System}, Name={e.Name}");
    }

    private async void OnManagerChannelEvent(object sender, ChannelEventArgs e)
    {
      LogInformation($"Data: Port={e.Device.Port}, Channel={e.Channel}, Enabled={e.Enabled}");
      LogInformation($"Send: {Converter.FromChannelEvent(e)}");

      await SendAsync(Converter.FromChannelEvent(e));
    }

    private async void OnManagerLevelEvent(object sender, LevelEventArgs e)
    {
      LogInformation($"Data: Port={e.Device.Port}, Level={e.Level}");
      LogInformation($"Send: {Converter.FromLevelEvent(e)}");

      await SendAsync(Converter.FromLevelEvent(e));
    }

    private async void OnManagerCommandEvent(object sender, CommandEventArgs e)
    {
      LogInformation($"Data: {e.Text}");
      LogInformation($"Send: {Converter.FromCommandEvent(e)}");

      await SendAsync(Converter.FromCommandEvent(e));
    }

    private async void OnManagerStringEvent(object sender, StringEventArgs e)
    {
      LogInformation($"Data: {e.Text}");
      LogInformation($"Send: {Converter.FromStringEvent(e)}");

      await SendAsync(Converter.FromStringEvent(e));
    }

    private async void OnBlinkMessageAsync(object sender, BlinkEventArgs e)
    {
      // LogInformation($"DateText={e.DateText}");

      // await SendAsync(e.DateText);

      await Task.CompletedTask;
    }

    #endregion

    public async Task SendAsync(string message)
    {
      if(Socket?.State != WebSocketState.Open)
        return;

      await Socket?.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    #region Log & Connection stuff

    private void LogError(string message, [CallerMemberName]string callerName = "(Caller name not set)")
    {
      mLogger.LogError($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}");
    }

    private void LogInformation(string message, [CallerMemberName]string callerName = "(Caller name not set)")
    {
      mLogger.LogInformation($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}");
    }

    private void StartConnectionTimer()
    {
      if(mIsDisposed)
        return;

      mConnectionTimer = new Timer(OnConnectionTimerElapsedAsync, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    private void StopConnectionTimer()
    {
      if(mConnectionTimer != null)
      {
        mConnectionTimer.Dispose();
        mConnectionTimer = null;
      }
    }

    private async void OnConnectionTimerElapsedAsync(object state)
    {
      StopConnectionTimer();

      if(mIsDisposed)
        return;

      if(!Manager?.IsConnected ?? false)
      {
        LogInformation($"Try Reconnect, Host={DeviceConfig.RemoteHost}, Port={DeviceConfig.RemotePort}");

        try
        {
          await Manager?.ConnectAsync(DeviceConfig.RemoteHost, DeviceConfig.RemotePort);
        }
        catch(Exception ex)
        {
          LogError(ex.Message);

          await SendAsync(ex.Message);
        }
      }

      if(!Manager?.IsConnected ?? false)
        StartConnectionTimer();
    }

    #endregion
  }
}
