using System;
using System.Collections.Generic;
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
using ICSP.Core.Manager.ConnectionManager;
using ICSP.Core.Manager.DeviceManager;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Converter;
using ICSP.WebProxy.WebControl;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

    private List<ushort> mDevices;

    private AmxDevice mDynamicDevice;

    private ushort mDevicePortCount = 1;
    private string mDeviceName;
    private string mDeviceVersion;
    private ushort mDeviceId;

    private bool mOverrideDevicePortCount;
    private bool mOverrideDeviceName;
    private bool mOverrideDeviceVersion;
    private bool mOverrideDeviceId;

    public ProxyClient(ILogger<ProxyClient> logger, IServiceProvider provider, ICSPConnectionManager connectionManager, WebSocketProxyClient connectedClient, IOptions<ProxyConfig> config)
    {
      mLogger = logger;

      mServiceProvider = provider;

      mConnectionManager = connectionManager;

      mConnectedClient = connectedClient;

      mConnectedClient.OnMessage += OnWebMessage;

      mPostProcessor = new FileTransferPostProcessor(this);

      ProxyConfig = config.Value;

      mDevices = new List<ushort>();

      CurrentDevice = string.Empty;
    }

    public static ProxyConfig ProxyConfig { get; set; }

    public HttpContext Context { get; private set; }

    public WebSocket Socket { get; private set; }

    public int SocketId { get; private set; }

    public ICSPManager Manager { get; private set; }

    public ProxyConnectionConfig ConnectionConfig { get; private set; }

    public IMessageConverter Converter { get; private set; }

    public string CurrentDevice { get; private set; }

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

      // ============================================================================================================================================
      // Setup DeviceConfig
      // ============================================================================================================================================

      // localhost:8000 -> Device Mapping 10001
      // localhost:8001 -> Device Mapping 10002
      ConnectionConfig = ProxyConfig.GetConfig(context);

      // Specific parameters by QueryString ...
      // file:///C:/Tmp/WebControl/index.html?ip=172.16.126.250&device=8002

      var lHost = context.Request.Query["host"];

      // Alias
      if(string.IsNullOrWhiteSpace(lHost))
        lHost = context.Request.Query["ip"];

      if(!string.IsNullOrWhiteSpace(lHost))
        ConnectionConfig.RemoteHost = lHost;

      if(ushort.TryParse(context.Request.Query["port"], out var lPort))
      {
        if(lPort > 0)
          ConnectionConfig.RemotePort = lPort;
      }

      if(ConnectionConfig.RemotePort == 0)
        ConnectionConfig.RemotePort = ICSPClient.DefaultPort;

      if(ushort.TryParse(context.Request.Query["device"], out var lDevice))
      {
        if(lDevice > 0)
          ConnectionConfig.Devices = new List<ushort> { lDevice };
      }

      // Manager = mConnectionManager.GetOrCreate(DeviceConfig.RemoteHost, DeviceConfig.RemotePort);

      Manager = new ICSPManager();
      Manager.socketId = socketId;

      if(!string.IsNullOrWhiteSpace(ConnectionConfig.BaseDirectory))
        Manager.FileManager.SetBaseDirectory(ConnectionConfig.BaseDirectory);

      // ============================================================================================================================================
      // Setup Converter
      // ============================================================================================================================================

      var lType = string.IsNullOrWhiteSpace(ConnectionConfig.Converter) ? typeof(ModuleWebControlConverter) : Type.GetType(ConnectionConfig.Converter, true);

      Converter = mServiceProvider.GetServices<IMessageConverter>().FirstOrDefault(p => p.GetType() == lType);

      Converter.Client = this;

      Converter.Dest = new AmxDevice(0, 1, 0);

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
          await InitializeDeviceConnectionAsync();
        }
        else
        {
          // Start monitoring auto reconnect
          StartConnectionTimer();

          try
          {
            LogInformation($"Try connect, Host={ConnectionConfig.RemoteHost}, Port={ConnectionConfig.RemotePort}");

            await Manager.ConnectAsync(ConnectionConfig.RemoteHost, ConnectionConfig.RemotePort);
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

    public ushort DevicePortCount { get => mDevicePortCount; set { mDevicePortCount = value; mOverrideDevicePortCount = true; } }

    public string DeviceName { get => mDeviceName; set { mDeviceName = value; mOverrideDeviceName = true; } }

    public string DeviceVersion { get => mDeviceVersion; set { mDeviceVersion = value; mOverrideDeviceVersion = true; } }

    public ushort DeviceId { get => mDeviceId; set { mDeviceId = value; mOverrideDeviceId = true; } }

    private async void OnWebMessage(object sender, MessageEventArgs e)
    {
      if(e.SocketId != SocketId)
        return;

      LogDebug($"Message={e.Message}");

      var lMsg = await Converter.ToDevMessageAsync(e.Message);

      try
      {
        if(lMsg != null)
          await Manager.SendAsync(lMsg);
      }
      catch(Exception ex)
      {
        LogError(ex.Message);
      }
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
        // RequestDevicesOnlineAsync
        // =====================================================================
        Manager.DynamicDeviceCreated += OnDynamicDeviceCreated;
        Manager.DeviceInfo += OnDeviceInfo;
        Manager.RequestDevicesOnlineEOT += OnManagerRequestDevicesOnlineEOT;

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
        // RequestDevicesOnlineAsync
        // =====================================================================
        Manager.DynamicDeviceCreated -= OnDynamicDeviceCreated;
        Manager.DeviceInfo -= OnDeviceInfo;
        Manager.RequestDevicesOnlineEOT -= OnManagerRequestDevicesOnlineEOT;

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

    internal async Task CreateDeviceInfoAsync(bool force = false)
    {
      if(Manager != null && ConnectionConfig.Devices?.Count > 0)
      {
        // Find free Device
        var lDeviceNo = ConnectionConfig.Devices.Except(mDevices).FirstOrDefault();

        // No more connections, Close WebSocket ...
        if(lDeviceNo == 0)
        {
          LogWarn($"DeviceRange={string.Join(", ", ConnectionConfig.Devices)}, No more device connections available on controller");

          Socket?.CloseAsync((WebSocketCloseStatus)4001, "No more device connections available on controller", CancellationToken.None);

          return;
        }

        CurrentDevice = lDeviceNo.ToString();

        // Get custom values from config for device
        if(ConnectionConfig.DeviceConfig.TryGetValue(CurrentDevice, out var deviceConfig))
        {
          // Check has override by IMessageConverter
          if(!mOverrideDevicePortCount) /**/ mDevicePortCount /**/ = deviceConfig.PortCount;
          if(!mOverrideDeviceName)      /**/ mDeviceName      /**/ = deviceConfig.DeviceName;
          if(!mOverrideDeviceVersion)   /**/ mDeviceVersion   /**/ = deviceConfig.DeviceVersion;
          if(!mOverrideDeviceId)        /**/ mDeviceId        /**/ = deviceConfig.DeviceId;
        }

        Converter.Device = lDeviceNo;

        LogWarn($"Device={lDeviceNo}");

        if(Manager.Devices.TryGetValue(lDeviceNo, out var lDeviceInfo))
        {
          Converter.System = lDeviceInfo.System;

          Converter.Dest = new AmxDevice(0, 1, lDeviceInfo.System);

          if(force)
          {
            lDeviceInfo = new DeviceInfoData(lDeviceNo, Manager.CurrentLocalIpAddress.Address);

            if(!string.IsNullOrWhiteSpace(mDeviceVersion))
              lDeviceInfo.Version = mDeviceVersion?.ToLower();

            if(mDeviceId > 0)
              lDeviceInfo.DeviceId = mDeviceId;

            if(!string.IsNullOrWhiteSpace(mDeviceName))
              lDeviceInfo.Name = mDeviceName;

            await Manager?.CreateDeviceInfoAsync(lDeviceInfo, mDevicePortCount);
          }
        }
        else
        {
          lDeviceInfo = new DeviceInfoData(lDeviceNo, Manager.CurrentLocalIpAddress.Address);

          if(!string.IsNullOrWhiteSpace(mDeviceVersion))
            lDeviceInfo.Version = mDeviceVersion?.ToLower();

          if(mDeviceId > 0)
            lDeviceInfo.DeviceId = mDeviceId;

          if(!string.IsNullOrWhiteSpace(mDeviceName))
            lDeviceInfo.Name = mDeviceName;

          Converter.Dest = Manager.SystemDevice;

          await Manager?.CreateDeviceInfoAsync(lDeviceInfo, mDevicePortCount);
        }
      }

      // No more connections, Close WebSocket ...
      if(ConnectionConfig.Devices?.Count == 0)
      {
        LogWarn($"DeviceRange={string.Join(", ", ConnectionConfig.Devices)}, No more device connections available on controller");

        Socket?.CloseAsync((WebSocketCloseStatus)4001, "No more device connections available on controller", CancellationToken.None);

        return;
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

        await InitializeDeviceConnectionAsync();
      }
      else
      {
        Socket?.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Remote connection has closed", CancellationToken.None);
      }
    }

    private async void OnDynamicDeviceCreated(object sender, DynamicDeviceCreatedEventArgs e)
    {
      LogDebug($"System={e.System}, Device={e.Device}");

      mDynamicDevice = new AmxDevice(e.Device, 1, e.System);

      // Not Needed ...
      // await Manager?.CreateDeviceInfoAsync(new DeviceInfoData(e.Device, Manager?.CurrentLocalIpAddress) { System = e.System });

      // Use DynamicDevice for query online-tree
      await Manager?.RequestDevicesOnlineAsync(mDynamicDevice);
    }

    private void OnDeviceInfo(object sender, DeviceInfoEventArgs e)
    {
      // Ignore
      if(mDynamicDevice == AmxDevice.Empty || e.ObjectId > 0)
        return;

      LogDebug($"{e.Device:00000} - {e.Name} ({e.Version})");

      if(!mDevices.Contains(e.Device))
        mDevices.Add(e.Device);
    }

    private async void OnManagerRequestDevicesOnlineEOT(object sender, EventArgs e)
    {
      LogDebug($"End ...");

      // Online-tree has finished, create the device
      await CreateDeviceInfoAsync();
    }

    private async void OnManagerDeviceOnline(object sender, DeviceInfoData e)
    {
      LogInformation($"Device={e.Device}, System={e.System}, Name={e.Name}");

      Converter.System = e.System;

      Converter.Device = e.Device;

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

    internal void LogError(string message, [CallerMemberName] string callerName = "(Caller name not set)")
    {
      mLogger.LogError($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}");
    }

    internal void LogWarn(string message, [CallerMemberName] string callerName = "(Caller name not set)")
    {
      mLogger.LogWarning($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}");
    }

    internal void LogInformation(string message, [CallerMemberName] string callerName = "(Caller name not set)")
    {
      mLogger.LogInformation($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}".Replace("\u0002", "[$02]").Replace("\u0003", "[$03]"));
    }

    internal void LogDebug(string message, [CallerMemberName] string callerName = "(Caller name not set)")
    {
      mLogger.LogDebug($"[{nameof(ProxyClient)}][{SocketId:00}][{callerName}]: {message}".Replace("\u0002", "[$02]").Replace("\u0003", "[$03]"));
    }

    private async Task InitializeDeviceConnectionAsync()
    {
      mDynamicDevice = AmxDevice.Empty;

      mDevices.Clear();

      // Create dynamic device for request OnlineTree
      await Manager?.SendAsync(MsgCmdDynamicDeviceAddressRequest.CreateRequest(Manager?.CurrentLocalIpAddress?.Address));
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
        LogInformation($"Try Reconnect, Host={ConnectionConfig.RemoteHost}, Port={ConnectionConfig.RemotePort}");

        try
        {
          await Manager?.ConnectAsync(ConnectionConfig.RemoteHost, ConnectionConfig.RemotePort);
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
