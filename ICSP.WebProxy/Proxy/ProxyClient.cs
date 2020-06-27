using System;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core;
using ICSP.Core.Client;
using ICSP.Core.IO;
using ICSP.Core.Manager.DeviceManager;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Converter;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ICSP.WebProxy.Proxy
{
  public class ProxyClient : IDisposable
  {
    private readonly ILogger mLogger;

    private readonly IServiceProvider mServiceProvider;

    private WebSocketProxyClient mConnectedClient;

    private readonly ICSPConnectionManager mConnectionManager;

    private readonly FileTransferPostProcessor mPostProcessor;

    private bool mIsDisposed;

    private Timer mConnectionTimer;

    private int mCurrentFileCount;
    private int mCurrentFileSize;

    public ProxyClient(ILogger<ProxyClient> logger, IServiceProvider provider, ICSPConnectionManager connectionManager, WebSocketProxyClient connectedClient)
    {
      mLogger = logger;

      mServiceProvider = provider;

      mConnectionManager = connectionManager;

      mConnectedClient = connectedClient;

      mConnectedClient.OnMessage += OnWebMessage;

      mPostProcessor = new FileTransferPostProcessor(this);
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
    }

    protected virtual void Dispose(bool disposing)
    {
      if(!mIsDisposed)
      {
        if(disposing)
        {
          // Verwalteten Zustand (verwaltete Objekte) entsorgen
          StopConnectionTimer();

          ManagerRemoveEventHandlers();

          Manager?.Disconnect();
          Manager?.Dispose();

          if(Manager?.IsDisposed ?? false)
            mConnectionManager.Remove(Manager);

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

      var lType = string.IsNullOrWhiteSpace(DeviceConfig.Converter) ? typeof(ModuleWebControlConverter) : Type.GetType(DeviceConfig.Converter, true);

      Converter = mServiceProvider.GetServices<IMessageConverter>().FirstOrDefault(p => p.GetType() == lType);

      Converter.Client = this;

      Converter.Device = DeviceConfig.Device;

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

      Manager = mConnectionManager.GetOrCreate(DeviceConfig.RemoteHost, DeviceConfig.RemotePort);

      if(!string.IsNullOrWhiteSpace(DeviceConfig.BaseDirectory))
        Manager.FileManager.SetBaseDirectory(DeviceConfig.BaseDirectory);

      // TODO: Only one Processor for shared web connections!
      // mPostProcessor.ProcessFiles();

      ManagerAddEventHandlers();

      // TODO: 
      // Before connect, wait a shortly seconds.
      // WebControl does transmit 'PortCount' from project.settings.portCount on WebSocket online-event.
      _ = Task.Run(async () =>
      {
          await Task.Delay(1000);

          if(Manager.IsConnected)
          {
            await CreateDeviceInfoAsync();
          }
          else
          {
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
          }
        });

      await Task.CompletedTask;
    }

    private async void OnWebMessage(object sender, MessageEventArgs e)
    {
      if(e.SocketId != SocketId)
        return;

      LogInformation($"Message={e.Message}");

      var lMsg = Converter.ToDevMessage(e.Message);

      if(lMsg != null)
        await Manager.SendAsync(lMsg);
    }

    public async Task SendAsync(string message)
    {
      if(Socket?.State != WebSocketState.Open || string.IsNullOrWhiteSpace(message))
        return;

      LogDebug($"Send: {message}");

      await Socket?.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    #region Manager events

    private void ManagerAddEventHandlers()
    {
      if(Manager != null)
      {
        Manager.Disposing += OnManagerDisposing;
        Manager.Disconnecting += OnManagerDisconnecting;

        Manager.ClientOnlineStatusChanged += OnManagerClientOnlineStatusChanged;
        Manager.DeviceOnline += OnManagerDeviceOnline;
        Manager.DeviceOffline += OnManagerDeviceOffline;

        Manager.ChannelEvent += OnManagerChannelEvent;
        Manager.StringEvent += OnManagerStringEvent;
        Manager.CommandEvent += OnManagerCommandEvent;
        Manager.LevelEvent += OnManagerLevelEvent;

        // =====================================================================
        // FileManager-Events
        // =====================================================================

        Manager.FileManager.OnTransferFileData += OnTransferFileData;

        //mICSPManager.FileManager.OnTransferFileDataComplete += OnTransferFileDataComplete;
        //mICSPManager.FileManager.OnTransferFileDataCompleteAck += OnTransferFileDataCompleteAck;
        Manager.FileManager.OnTransferFilesInitialize += OnTransferFilesInitialize;
        Manager.FileManager.OnTransferFilesComplete += OnTransferFilesComplete;

        Manager.FileManager.OnGetDirectoryInfo += OnGetDirectoryInfo;
        Manager.FileManager.OnDirectoryInfo += OnDirectoryInfo;
        Manager.FileManager.OnDirectoryItem += OnDirectoryItem;
        Manager.FileManager.OnDeleteFile += OnDeleteFile;
        Manager.FileManager.OnCreateDirectory += OnCreateDirectory;

        // mICSPManager.FileManager.OnTransferSingleFile += OnTransferSingleFile;
        // mICSPManager.FileManager.OnTransferSingleFileAck += OnTransferSingleFileAck;
        Manager.FileManager.OnTransferSingleFileInfo += OnTransferSingleFileInfo;
        // mICSPManager.FileManager.OnTransferSingleFileInfoAck += OnTransferSingleFileInfoAck;
        Manager.FileManager.OnTransferGetFileAccessToken += OnTransferGetFileAccessToken;
        Manager.FileManager.OnTransferGetFileAccessTokenAck += OnTransferGetFileAccessTokenAck;
        Manager.FileManager.OnTransferGetFile += OnTransferGetFile;

      }
    }

    private void ManagerRemoveEventHandlers()
    {
      if(Manager != null)
      {
        Manager.Disposing -= OnManagerDisposing;
        Manager.Disconnecting -= OnManagerDisconnecting;

        Manager.Disconnecting -= OnManagerDisconnecting;
        Manager.ClientOnlineStatusChanged -= OnManagerClientOnlineStatusChanged;
        Manager.DeviceOnline -= OnManagerDeviceOnline;
        Manager.DeviceOffline -= OnManagerDeviceOffline;

        Manager.ChannelEvent -= OnManagerChannelEvent;
        Manager.StringEvent -= OnManagerStringEvent;
        Manager.CommandEvent -= OnManagerCommandEvent;
        Manager.LevelEvent -= OnManagerLevelEvent;

        // =====================================================================
        // FileManager-Events
        // =====================================================================

        Manager.FileManager.OnTransferFileData -= OnTransferFileData;

        //mICSPManager.FileManager.OnTransferFileDataComplete -=OnTransferFileDataComplete;
        //mICSPManager.FileManager.OnTransferFileDataCompleteAck -=OnTransferFileDataCompleteAck;
        Manager.FileManager.OnTransferFilesInitialize -= OnTransferFilesInitialize;
        Manager.FileManager.OnTransferFilesComplete -= OnTransferFilesComplete;

        Manager.FileManager.OnGetDirectoryInfo -= OnGetDirectoryInfo;
        Manager.FileManager.OnDirectoryInfo -= OnDirectoryInfo;
        Manager.FileManager.OnDirectoryItem -= OnDirectoryItem;
        Manager.FileManager.OnDeleteFile -= OnDeleteFile;
        Manager.FileManager.OnCreateDirectory -= OnCreateDirectory;

        // mICSPManager.FileManager.OnTransferSingleFile -=OnTransferSingleFile;
        // mICSPManager.FileManager.OnTransferSingleFileAck -=OnTransferSingleFileAck;
        Manager.FileManager.OnTransferSingleFileInfo -= OnTransferSingleFileInfo;
        // mICSPManager.FileManager.OnTransferSingleFileInfoAck -=OnTransferSingleFileInfoAck;
        Manager.FileManager.OnTransferGetFileAccessToken -= OnTransferGetFileAccessToken;
        Manager.FileManager.OnTransferGetFileAccessTokenAck -= OnTransferGetFileAccessTokenAck;
        Manager.FileManager.OnTransferGetFile -= OnTransferGetFile;
      }
    }

    private async Task CreateDeviceInfoAsync()
    {
      if(Manager != null && DeviceConfig.Device > 0)
      {
        if(Manager.Devices.TryGetValue(DeviceConfig.Device, out var lDeviceInfo))
        {
          Converter.System = lDeviceInfo.System;

          Converter.Dest = new AmxDevice(0, 1, lDeviceInfo.System);
        }
        else
        {
          lDeviceInfo = new DeviceInfoData(DeviceConfig.Device, Manager.CurrentLocalIpAddress);

          if(!string.IsNullOrWhiteSpace(DeviceConfig.DeviceVersion))
            lDeviceInfo.Version = DeviceConfig.DeviceVersion.ToLower();

          if(DeviceConfig.DeviceId > 0)
            lDeviceInfo.DeviceId = DeviceConfig.DeviceId;

          if(!string.IsNullOrWhiteSpace(DeviceConfig.DeviceName))
            lDeviceInfo.Name = DeviceConfig.DeviceName;

          Converter.Dest = Manager.SystemDevice;

          await Manager?.CreateDeviceInfoAsync(lDeviceInfo, DeviceConfig.PortCount);
        }
      }
    }

    private void OnManagerDisposing(object sender, CancelEventArgs e)
    {
      // Closing or Disposing calls by other ProxyClient-instances
      // Prevent Closing or disposing
      e.Cancel = true;
    }

    private void OnManagerDisconnecting(object sender, CancelEventArgs e)
    {
      // Closing or Disposing calls by other ProxyClient-instances
      // Prevent Closing or disposing
      e.Cancel = true;
    }

    private async void OnManagerClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      LogInformation($"ClientOnline={e.ClientOnline}");

      if(e.ClientOnline)
      {
        StopConnectionTimer();

        await CreateDeviceInfoAsync();
      }
      else
      {
        // StartConnectionTimer();

        Socket?.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Device Offline", CancellationToken.None);
      }
    }

    private async void OnManagerDeviceOnline(object sender, DeviceInfoData e)
    {
      LogInformation($"Device={e.Device}, System={e.System}, Name={e.Name}");

      Converter.System = e.System;

      Converter.Dest = new AmxDevice(0, 1, e.System);

      var lMsg = Converter.DeviceOnline();

      if(lMsg != null)
        await Manager.SendAsync(lMsg);
    }

    private async void OnManagerDeviceOffline(object sender, DeviceInfoData e)
    {
      LogInformation($"Device={e.Device}, System={e.System}, Name={e.Name}");

      await SendAsync(Converter.DeviceOffline());

      Socket?.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Device Offline", CancellationToken.None);
    }

    private async void OnManagerChannelEvent(object sender, ChannelEventArgs e)
    {
      LogDebug($"Data: Type=ChannelEvent, Port={e.Device.Port}, Channel={e.Channel}, Enabled={e.Enabled}");

      await SendAsync(Converter.FromChannelEvent(e));
    }

    private async void OnManagerLevelEvent(object sender, LevelEventArgs e)
    {
      LogDebug($"Data: Type=LevelEvent, Port={e.Device.Port}, Level={e.Level}");

      await SendAsync(Converter.FromLevelEvent(e));
    }

    private async void OnManagerCommandEvent(object sender, CommandEventArgs e)
    {
      LogDebug($"Data: Type=Command, Port={e.Device.Port}, Text={e.Text}");

      await SendAsync(Converter.FromCommandEvent(e));
    }

    private async void OnManagerStringEvent(object sender, StringEventArgs e)
    {
      LogDebug($"Data: Type=String, Port={e.Device.Port}, Text={e.Text}");

      await SendAsync(Converter.FromStringEvent(e));
    }

    #endregion

    #region FileManager-Events

    private void OnTransferFileData(object sender, TransferFileDataEventArgs e)
    {
      mCurrentFileSize += e.JunkSize;
    }

    private void OnTransferFileDataComplete(object sender, EventArgs e)
    {
      LogDebug($"OnTransferFileDataComplete");
    }

    private void OnTransferFileDataCompleteAck(object sender, EventArgs e)
    {
      LogDebug($"OnTransferFileDataCompleteAck");
    }

    private void OnTransferFilesInitialize(object sender, TransferFilesInitializeEventArgs e)
    {
      LogDebug($"OnTransferFilesInitialize: FileCount={e.FileCount}");

      mCurrentFileCount = 0;
      mCurrentFileSize = 0;
    }

    private async void OnTransferFilesComplete(object sender, EventArgs e)
    {
      mCurrentFileCount = 0;
      mCurrentFileSize = 0;

      LogDebug($"OnTransferFilesComplete");

      // TODO: Only one Processor for shared web connections!
      mPostProcessor.ProcessFiles();

      await SendAsync(Converter.OnTransferFilesComplete());
    }

    private void OnGetDirectoryInfo(object sender, GetDirectoryInfoEventArgs e)
    {
      LogDebug($"OnGetDirectoryInfo: Path={e.Path}");
    }

    private void OnDirectoryInfo(object sender, DirectoryInfoEventArgs e)
    {
      LogDebug($"OnDirectoryInfo: FullPath={e.FullPath}");
    }

    private void OnDirectoryItem(object sender, DirectoryItemEventArgs e)
    {
      LogDebug($"OnDirectoryItem: FileName={e.FileName}");
    }

    private void OnDeleteFile(object sender, DeleteFileEventArgs e)
    {
      LogDebug($"OnDeleteFile: FileName={e.FileName}");
    }

    private void OnCreateDirectory(object sender, CreatDirectoryEventArgs e)
    {
      LogDebug($"OnCreateDirectory: Directory={e.Directory}");
    }

    private void OnTransferSingleFile(object sender, EventArgs e)
    {
      LogDebug($"OnTransferSingleFile");
    }

    private void OnTransferSingleFileAck(object sender, EventArgs e)
    {
      LogDebug($"OnTransferSingleFileAck");
    }

    private void OnTransferSingleFileInfo(object sender, TransferSingleFileInfoEventArgs e)
    {
      mCurrentFileCount++;
      mCurrentFileSize = 0;

      LogDebug($"OnTransferSingleFileInfo: FileSize={e.FileSize} bytes, FileName={e.FileName}");
    }

    private void OnTransferSingleFileInfoAck(object sender, EventArgs e)
    {
      LogDebug($"OnTransferSingleFileInfoAck");
    }

    private void OnTransferGetFileAccessToken(object sender, TransferGetFileAccessTokenEventArgs e)
    {
      LogDebug($"OnTransferGetFileAccessToken: Size={e.Size} bytes, FileName={e.FileName}");
    }

    private void OnTransferGetFileAccessTokenAck(object sender, EventArgs e)
    {
      LogDebug($"OnTransferGetFileAccessTokenAck");
    }

    private void OnTransferGetFile(object sender, EventArgs e)
    {
      LogDebug($"OnTransferGetFile");
    }

    #endregion

    #region Log & Connection stuff

    internal void LogError(string message, [CallerMemberName]string callerName = "(Caller name not set)")
    {
      mLogger.LogError($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}");
    }

    internal void LogInformation(string message, [CallerMemberName]string callerName = "(Caller name not set)")
    {
      mLogger.LogInformation($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}".Replace("\u0002", "[$02]").Replace("\u0003", "[$03]"));
    }

    internal void LogDebug(string message, [CallerMemberName]string callerName = "(Caller name not set)")
    {
      mLogger.LogDebug($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}".Replace("\u0002", "[$02]").Replace("\u0003", "[$03]"));
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
