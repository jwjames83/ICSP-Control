using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Client;
using ICSP.Core.Cryptography;
using ICSP.Core.IO;
using ICSP.Core.Logging;
using ICSP.Core.Manager;
using ICSP.Core.Manager.ConfigurationManager;
using ICSP.Core.Manager.ConnectionManager;
using ICSP.Core.Manager.DeviceManager;
using ICSP.Core.Manager.DiagnosticManager;

using Serilog.Events;

namespace ICSP.Core
{
  public class ICSPManager : IDisposable
  {
    public const string DefaultUsername = "administrator";
    public const string DefaultPassword = "password";

    private const int EncryptionModeMask = (int)(EncryptionMode.RC4_Receive | EncryptionMode.RC4_Send); // 0x06

    private readonly ConcurrentDictionary<ushort, CreateDeviceInfoRequest> mPendingDevicesRequests;

    private readonly ConcurrentDictionary<ushort, DeviceInfoData> mDevices;

    public event EventHandler<ClientOnlineOfflineEventArgs> ClientOnlineStatusChanged;

    public event EventHandler<DynamicDeviceCreatedEventArgs> DynamicDeviceCreated;

    public event EventHandler<ICSPMsgDataEventArgs> DataReceived;
    public event EventHandler<ICSPMsgDataEventArgs> CommandNotImplemented;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public event EventHandler<BlinkEventArgs> BlinkMessage;
    public event EventHandler<PingEventArgs> PingEvent;

    public event EventHandler<DeviceInfoEventArgs> DeviceInfo;
    public event EventHandler<PortCountEventArgs> PortCount;

    public event EventHandler<ChannelEventArgs> ChannelEvent;
    public event EventHandler<StringEventArgs> StringEvent;
    public event EventHandler<CommandEventArgs> CommandEvent;
    public event EventHandler<LevelEventArgs> LevelEvent;

    public event EventHandler<EventArgs> RequestDevicesOnlineEOT;
    public event EventHandler<DiscoveryInfoEventArgs> DiscoveryInfo;

    public event EventHandler<DeviceInfoData> DeviceOnline;
    public event EventHandler<DeviceInfoData> DeviceOffline;

    public event EventHandler<CancelEventArgs> Disconnecting;
    public event EventHandler<CancelEventArgs> Disposing;

    private ICSPClient mClient;

    private int mConnectionTimeout = 1;

    private int mTaskConnectAsyncRunning = 0;

    public ICSPManager()
    {
      mPendingDevicesRequests = new ConcurrentDictionary<ushort, CreateDeviceInfoRequest>();

      mDevices = new ConcurrentDictionary<ushort, DeviceInfoData>();

      Credentials = new NetworkCredential(DefaultUsername, DefaultPassword);

      FileManager = new FileManager(this);
    }

    public int socketId;

    public bool IsDisposed { get; private set; }

    public ushort CurrentSystem { get; private set; }

    public AmxDevice SystemDevice { get; private set; }

    public ConcurrentDictionary<ushort, DeviceInfoData> Devices
    {
      get
      {
        return mDevices;
      }
    }

    public bool IsConnected
    {
      get
      {
        return mClient?.Connected ?? false;
      }
    }

    public IPEndPoint CurrentRemoteIpAddress
    {
      get
      {
        return mClient?.RemoteEndPoint;
      }
    }

    public IPEndPoint CurrentLocalIpAddress
    {
      get
      {
        return mClient?.LocalEndPoint;
      }
    }

    public string Host { get; private set; }

    public int Port { get; private set; }

    public NetworkCredential Credentials { get; set; }

    /// <summary>
    /// Gets the time to wait while trying to establish a connection
    /// before terminating the attempt and generating an error.
    /// </summary>
    /// <remarks>
    /// You can set the amount of time a connection waits to time out by 
    /// using the ConnectTimeout or Connection Timeout keywords in the connection string.
    /// A value of 0 indicates no limit, and should be avoided in a ConnectionString 
    /// because an attempt to connect waits indefinitely.
    /// </remarks>
    public int ConnectionTimeout
    {
      get
      {
        return mConnectionTimeout;
      }
      set
      {
        if(value < 0)
          throw new ArgumentOutOfRangeException(nameof(ConnectionTimeout));

        mConnectionTimeout = value;
      }
    }

    public FileManager FileManager { get; }

    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if(!IsDisposed)
      {
        if(disposing)
        {
          var lEventArgs = new CancelEventArgs();

          Disposing?.Invoke(this, lEventArgs);

          if(lEventArgs.Cancel)
            return;

          // Verwalteten Zustand (verwaltete Objekte) entsorgen
          if(mClient != null)
          {
            mClient.ClientOnlineStatusChanged -= OnClientOnlineStatusChanged;
            mClient.DataReceived -= OnDataReceived;

            mClient.Dispose();
            mClient = null;
          }
        }

        IsDisposed = true;
      }
    }

    public async Task ConnectAsync(string host)
    {
      await ConnectAsync(host, ICSPClient.DefaultPort);
    }

    public async Task ConnectAsync(string host, int port)
    {
      if(IsDisposed)
        throw new ObjectDisposedException("The current instance has been disposed!");

      // Ensure that method would be called only once
      if(Interlocked.Exchange(ref mTaskConnectAsyncRunning, 1) != 0)
        return;

      try
      {
        Host = host ?? throw new ArgumentNullException(nameof(host));

        Port = port;

        if(mClient == null)
        {
          mClient = new ICSPClient() { ConnectionTimeout = mConnectionTimeout };

          mClient.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
          mClient.DataReceived += OnDataReceived;
        }

        await mClient.ConnectAsync(Host, Port);
      }
      finally
      {
        Interlocked.Exchange(ref mTaskConnectAsyncRunning, 0);
      }
    }

    public void Disconnect(bool force = false)
    {
      if(IsDisposed)
        return;

      var lEventArgs = new CancelEventArgs();

      if(!force)
        Disconnecting?.Invoke(this, lEventArgs);

      if(lEventArgs.Cancel)
        return;

      if(mClient != null)
        mClient.Disconnect();
    }

    private void OnClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      if(e.ClientOnline)
      {
      }
      else
      {
        CurrentSystem = 0;

        SystemDevice = AmxDevice.Empty;

        // Do not remove devices!
        // After restart Master controller, master does ping ...
        mDevices.Clear();

        var lKeys = MemoryCache.Default.Select(s => s.Key).ToList();

        foreach(var key in lKeys)
          MemoryCache.Default.Remove(key);
      }

      ClientOnlineStatusChanged?.Invoke(this, e);
    }

    private async void OnDataReceived(object sender, ICSPMsgDataEventArgs e)
    {
      try
      {
        if(!(e.Message is MsgCmdBlinkMessage))
        {
          Logger.LogVerbose("Data {0} bytes", e.Message.RawData.Length);
          Logger.LogVerbose("Data 0x: {0:l}", BitConverter.ToString(e.Message.RawData).Replace("-", " "));
        }

        DataReceived?.Invoke(this, e);

        // No action needed
        if(e.Handled)
          return;

        // Speed up
        if(Logger.LogLevel <= LogEventLevel.Verbose && !(e.Message is MsgCmdBlinkMessage))
          e.Message.WriteLogVerbose();

        switch(e.Message)
        {
          case MsgCmdAck m:
          {
            if(CurrentSystem == 0)
            {
              CurrentSystem = m.Source.System;

              SystemDevice = new AmxDevice(0, 1, m.Source.System);
            }

            if(mPendingDevicesRequests.TryRemove(m.ID, out var lRequestInfo))
            {
              if(MemoryCache.Default.Remove(lRequestInfo.ID) == null)
                Logger.LogWarn("MsgCmdAck: RequestID=0x{0:X4} not found in MemoryCache!", lRequestInfo.ID);

              lRequestInfo.State = DeviceConnectionState.Connected;

              var lDeviceInfo = lRequestInfo.DeviceInfo;

              Logger.LogInfo("MsgCmdAck: ID=0x{0:X4}, Device={1}, System={2}, DeviceId={3}, Name={4:l}", m.ID, lDeviceInfo.Device, lDeviceInfo.System, lDeviceInfo.DeviceId, lDeviceInfo.Name);

              lDeviceInfo.System = m.Source.System;

              var lResult = mDevices.TryAdd(lDeviceInfo.Device, lDeviceInfo);

              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
              Logger.LogDebug(false, "Device Online: Device={0}, Name={1:l}, IPv4Address={2:l}", lDeviceInfo.Device, lDeviceInfo.Name, lDeviceInfo.IPv4Address);
              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

              await UpdateDeviceInfoAsync(m.Source, m.Dest, lRequestInfo);

              DeviceOnline?.Invoke(this, lDeviceInfo);
            }
            else
            {
              Logger.LogWarn("MsgCmdAck: ID=0x{0:X4} unknown!", m.ID);
            }

            break;
          }
          case MsgCmdChallengeAckMD5 m:
          {
            if(mPendingDevicesRequests.TryRemove(m.ID, out var lRequestInfo))
            {
              if(MemoryCache.Default.Remove(lRequestInfo.ID) == null)
                Logger.LogWarn("MsgCmdChallengeAckMD5: RequestID=0x{0:X4} not found in MemoryCache!", lRequestInfo.ID);

              lRequestInfo.State = DeviceConnectionState.Connected;

              if(m.Authenticated)
              {
                mClient.EncryptionMode = (EncryptionMode)((int)m.Status & EncryptionModeMask);

                var lDeviceInfo = lRequestInfo.DeviceInfo;

                Logger.LogInfo("MsgCmdChallengeAckMD5: ID=0x{0:X4}, Device={1}, System={2}, DeviceId={3}, Name={4:l}", m.ID, lDeviceInfo.Device, lDeviceInfo.System, lDeviceInfo.DeviceId, lDeviceInfo.Name);

                lDeviceInfo.System = m.Source.System;

                // Create device request again
                await CreateDeviceInfoAsync(lRequestInfo.DeviceInfo, lRequestInfo.PortCount, lRequestInfo.ChannelCount, lRequestInfo.LevelCount);
              }
              else
              {
                Logger.LogError("MsgCmdChallengeAckMD5: Authenticated failed!", m.ID);
              }
            }
            else
            {
              Logger.LogWarn("MsgCmdChallengeAckMD5: ID=0x{0:X4} unknown!", m.ID);
            }

            break;
          }
          case MsgCmdFileTransfer m:
          {
            await FileManager.ProcessMessageAsync(m);

            break;
          }
          case MsgCmdBlinkMessage m:
          {
            BlinkMessage?.Invoke(this, new BlinkEventArgs(m));

            break;
          }
          case MsgCmdPingRequest m:
          {
            if(mDevices.TryGetValue(m.Device, out var lDeviceInfo))
            {
              var lKey = "Device:" + lDeviceInfo.Device;

              var lPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(25), RemovedCallback = OnCacheEntryRemovedCallback };

              MemoryCache.Default.AddOrGetExisting(lKey, lDeviceInfo, lPolicy);

              /*
                           P  | Len   | Flag  | Dest (SDP)        | Source (SDP)      | H  | ID    | CMD   | N-Data      | CS
              -------------------------------------------------------------------------------------------------------------------------------------------------
              PingRequest  02 | 00 17 | 02 00 | 00 00 00 00 00 00 | 00 00 00 00 00 00 | ff | 09 e9 | 05 01 | 3a 99 | 00 01 | e6 
              PingResponse 02 | 00 21 | 02 00 | 00 01 00 00 00 01 | 00 01 3a 99 00 01 | 0f | 54 ba | 05 81 | 3a 99 | 00 01 | 00 01 | 01 9c | 02 04 ac 10 7e a7 | f8
              */

              // Dest => 0:1:1, Src => 15001:1:1
              var lSource = new AmxDevice(lDeviceInfo.Device, 1, lDeviceInfo.System);

              var lResponse = MsgCmdPingResponse.CreateRequest(m.Source, lSource,
                lDeviceInfo.Device, lDeviceInfo.System, lDeviceInfo.ManufactureId, lDeviceInfo.DeviceId, mClient?.LocalEndPoint?.Address);

              await SendAsync(lResponse);

              PingEvent?.Invoke(this, new PingEventArgs(m));
            }
            else
            {
              Logger.LogWarn("MsgCmdPingRequest: Device={0} not in Devices!", m.Device);
            }

            break;
          }
          case MsgCmdDynamicDeviceAddressResponse m:
          {
            DynamicDeviceCreated?.Invoke(this, new DynamicDeviceCreatedEventArgs(m));

            break;
          }
          case MsgCmdRequestEthernetIp m:
          {
            if(mDevices.ContainsKey(m.Dest.Device))
            {
              var lRequest = MsgCmdGetEthernetIpAddress.CreateRequest(m.Source, m.Dest, mClient.LocalEndPoint.Address);

              await SendAsync(lRequest);
            }
            else
            {
              var lRequest = MsgCmdGetEthernetIpAddress.CreateRequest(m.Source, m.Dest, mClient.LocalEndPoint.Address);

              await SendAsync(lRequest);

              /*
              lRequest = MsgCmdStatus.CreateRequest(m.Source, m.Dest, m.Dest, StatusType.Reset, 0, "Sorry, was offline ...");

              await SendAsync(lRequest);
              */
            }

            break;
          }
          case MsgCmdRequestDeviceInfo m:
          {
            var lDeviceInfo = new DeviceInfoData(m.Device, mClient.LocalEndPoint.Address) { System = m.System };

            var lResponse = MsgCmdDeviceInfo.CreateRequest(m.Source, m.Dest, lDeviceInfo);

            await SendAsync(lResponse);

            break;
          }
          case MsgCmdRequestStatus m:
          {
            var lResponse = MsgCmdStatus.CreateRequest(e.Message.Source, e.Message.Dest, e.Message.Dest, StatusType.Normal, 1, "Normal");

            await SendAsync(lResponse);

            break;
          }
          case MsgCmdOutputChannelOn m:
          {
            ChannelEvent?.Invoke(this, new ChannelEventArgs(m));

            break;
          }
          case MsgCmdOutputChannelOff m:
          {
            ChannelEvent?.Invoke(this, new ChannelEventArgs(m));

            break;
          }
          case MsgCmdDeviceInfo m:
          {
            if(CurrentSystem == 0)
            {
              // Don't read CurrentSystem from MsgCmdDeviceInfo
              // MsgCmdRequestDevicesOnline returns also M2M devices (multiple systems answers)

              CurrentSystem = m.Source.System;

              SystemDevice = new AmxDevice(0, 1, m.Source.System);
            }

            if(CurrentSystem > 0)
              DeviceInfo?.Invoke(this, new DeviceInfoEventArgs(m));

            break;
          }
          case MsgCmdPortCountBy m:
          {
            PortCount?.Invoke(this, new PortCountEventArgs(m));

            break;
          }
          case MsgCmdRequestDevicesOnlineEOT m:
          {
            RequestDevicesOnlineEOT?.Invoke(this, EventArgs.Empty);

            break;
          }
          case MsgCmdDiscoveryInfo m:
          {
            DiscoveryInfo?.Invoke(this, new DiscoveryInfoEventArgs(m));

            break;
          }
          case MsgCmdStringMasterDev m:
          {
            StringEvent?.Invoke(this, new StringEventArgs(m));

            break;
          }
          case MsgCmdCommandMasterDev m:
          {
            CommandEvent?.Invoke(this, new CommandEventArgs(m));

            break;
          }
          case MsgCmdLevelValueMasterDev m:
          {
            LevelEvent?.Invoke(this, new LevelEventArgs(m));

            break;
          }
          case MsgCmdChallengeRequestMD5 m:
          {
            // =========================================
            // Challenge Handshake Authentication (CHAP)
            // =========================================

            using var lHashAlgorithm = HashAlgorithm.Create("MD5");

            if(lHashAlgorithm == null)
              throw new Exception("ICSP: Failed to build encryption key!");

            var lHash = lHashAlgorithm.ComputeHash(
              m.Challenge
              .Concat(Encoding.UTF8.GetBytes(Credentials?.UserName ?? DefaultUsername))
              .Concat(Encoding.UTF8.GetBytes(Credentials?.Password ?? DefaultPassword)).ToArray());

            // Initialize CryptoProvider
            mClient.CryptoProvider = RC4.Create(lHash);

            var lResponse = MsgCmdChallengeResponseMD5.CreateRequest(m.Source, m.Dest, m.Challenge, EncryptionType.RC4_Both, Credentials);

            if(mPendingDevicesRequests.TryRemove(m.ID, out var lRequestInfo))
            {
              lRequestInfo.State = DeviceConnectionState.Unverified;

              lRequestInfo.MsgID = lResponse.ID;

              mPendingDevicesRequests.TryAdd(lResponse.ID, lRequestInfo);
            }

            await SendAsync(lResponse);

            break;
          }
          case MsgCmdUnknown m:
          {
            Logger.LogDebug(false, "----------------------------------------------------------------");

            // Logger.LogWarn("Command: 0x{0:X4} ({1:l}) => Command not implemented", e.Command, ICSPMsg.GetFrindlyName(e.Command));

            CommandNotImplemented?.Invoke(this, new ICSPMsgDataEventArgs(m));

            break;
          }
          default:
          {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(e.Message));

            break;
          }
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }
    }

    private void OnCacheEntryRemovedCallback(CacheEntryRemovedArguments arguments)
    {
      if(arguments.RemovedReason != CacheEntryRemovedReason.Removed)
      {
        if(arguments.CacheItem.Value is CreateDeviceInfoRequest lRequestInfo)
        {
          var lDeviceInfo = lRequestInfo.DeviceInfo;

          switch(lRequestInfo.State)
          {
            case DeviceConnectionState.Uninitialized:
            {
              Logger.LogWarn(false, "No response for {0:l}, Device={1}, Name={2:l}, IPv4Address={3:l}, State={4}", arguments.CacheItem.Key, lDeviceInfo.Device, lDeviceInfo.Name, lDeviceInfo.IPv4Address, lRequestInfo.State);
              break;
            }
            case DeviceConnectionState.Unverified:
            {
              Logger.LogWarn(false, "No response for {0:l}, Device={1}, Name={2:l}, IPv4Address={3:l}, State={4}", arguments.CacheItem.Key, lDeviceInfo.Device, lDeviceInfo.Name, lDeviceInfo.IPv4Address, lRequestInfo.State);
              break;
            }
            case DeviceConnectionState.Connected:
            {
              Logger.LogWarn(false, "No response for {0:l}, Device={1}, Name={2:l}, IPv4Address={3:l}, State={4}", arguments.CacheItem.Key, lDeviceInfo.Device, lDeviceInfo.Name, lDeviceInfo.IPv4Address, lRequestInfo.State);
              break;
            }
            case DeviceConnectionState.Disconnected:
            {
              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
              Logger.LogDebug(false, "Device Offline: Device={0}, Name={1:l}, IPv4Address={2:l}", lDeviceInfo.Device, lDeviceInfo.Name, lDeviceInfo.IPv4Address);
              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

              var lResult = mDevices.TryRemove(lDeviceInfo.Device, out _);

              DeviceOffline?.Invoke(this, lDeviceInfo);

              break;
            }
            default:
            {
              Logger.LogWarn(false, "No response for {0:l}, Device={1}, Name={2:l}, IPv4Address={3:l}, State={4}", arguments.CacheItem.Key, lDeviceInfo.Device, lDeviceInfo.Name, lDeviceInfo.IPv4Address, lRequestInfo.State);
              break;
            }
          }
        }
      }
    }

    public async Task SendStringAsync(AmxDevice dest, AmxDevice source, string text)
    {
      var lRequest = MsgCmdStringDevMaster.CreateRequest(dest, source, text);

      await SendAsync(lRequest);
    }

    public async Task SendCommandAsync(AmxDevice dest, AmxDevice source, string text)
    {
      var lRequest = MsgCmdCommandDevMaster.CreateRequest(dest, source, text);

      await SendAsync(lRequest);
    }

    public async Task SetOutputChannelAsync(AmxDevice dest, AmxDevice source, ushort channel, bool enabled)
    {
      if(enabled)
      {
        var lRequest = MsgCmdOutputChannelOn.CreateRequest(dest, source, channel);

        await SendAsync(lRequest);
      }
      else
      {
        var lRequest = MsgCmdOutputChannelOff.CreateRequest(dest, source, channel);

        await SendAsync(lRequest);
      }
    }

    public async Task SetInputChannelAsync(AmxDevice dest, AmxDevice source, ushort channel, bool enabled)
    {
      if(enabled)
      {
        var lRequest = MsgCmdInputChannelOnStatus.CreateRequest(dest, source, channel);

        await SendAsync(lRequest);
      }
      else
      {
        var lRequest = MsgCmdInputChannelOffStatus.CreateRequest(dest, source, channel);

        await SendAsync(lRequest);
      }
    }

    public async Task SendLevelAsync(AmxDevice dest, AmxDevice source, ushort level, ushort value)
    {
      var lRequest = MsgCmdLevelValueDevMaster.CreateRequest(dest, source, level, value);

      await SendAsync(lRequest);
    }

    public async Task CreateDeviceInfoAsync(DeviceInfoData deviceInfo, ushort portCount = 1, ushort channelCount = 256, ushort levelCount = 8)
    {
      /*
      P  | Len   | Flag  | Dest              | Source            | H  | ID    | CMD   | N-Data      | CS
      -------------------------------------------------------------------------------------------------------------------------------------------------
      Response to RequestDeviceInfo.
      02 | 00 59 | 02 12 | 00 00 00 00 00 01 | 00 00 27 12 00 01 | 0f | 00 48 | 00 97 | 27 12 00 00 00 00 00 00 00 01 01 71 35 39 36 38 30 32 70 31 30 63 30 31 30 35 00 00 03 8a 76 32 2e 31 30 34 2e 31 33 34 00 4d 58 54 2d 31 39 30 30 4c 2d 50 41 4e 69 00 41 4d 58 20 4c 4c 43 00 02 04 ac 10 7e a8 b4
                           SDP: 0:0:1        | SDP 0:10002:1
      02 | 00 13 | 02 08 | 00 00 00 00 00 00 | 00 01 00 00 00 01 | ff | 00 48 | 00 01 | 69

      02 | 00 13 | 02 08 | 00 01 27 12 00 01 | 00 01 00 00 00 01 | 0F | 00 48 | 00 01 | B4
                           SDP: 1:10002:1    | SDP 1:0:1
      */

      var lDest = new AmxDevice(0, 1, 0);

      var lSource = new AmxDevice(deviceInfo.Device, 1, deviceInfo.System);

      var lDeviceRequest = MsgCmdDeviceInfo.CreateRequest(lDest, lSource, deviceInfo);

      var lRequestInfo = new CreateDeviceInfoRequest(lDeviceRequest.ID, deviceInfo, portCount, channelCount, levelCount);

      mPendingDevicesRequests.TryAdd(lRequestInfo.MsgID, lRequestInfo);

      // 30 Sec: If controller is startup, response need a short short time ...
      var lPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(30), RemovedCallback = OnCacheEntryRemovedCallback };

      // Add this request to identify the Ack-Message
      MemoryCache.Default.Set(lRequestInfo.ID, lRequestInfo, lPolicy);

      Logger.LogInfo("ID=0x{0:X4}, Device={1}, System={2}, DeviceId={3}, Name={4:l}, PortCount={5}", lDeviceRequest.ID, deviceInfo.Device, deviceInfo.System, deviceInfo.DeviceId, deviceInfo.Name, portCount);

      await SendAsync(lDeviceRequest);
    }

    private async Task UpdateDeviceInfoAsync(AmxDevice dest, AmxDevice source, CreateDeviceInfoRequest request)
    {
      if(request.PortCount > 1)
      {
        // It is sent by a device upon reporting if the device has more than one port.
        var lPortCountRequest = MsgCmdPortCountBy.CreateRequest(dest, source, source.Device, source.System, request.PortCount);

        await SendAsync(lPortCountRequest);
      }

      if(request.ChannelCount > 256)
      {
        // It is sent by a device/port upon reporting if the device has more than 256 channels.
        var lOutputChannelCountRequest = MsgCmdOutputChannelCount.CreateRequest(dest, source, request.ChannelCount);

        await SendAsync(lOutputChannelCountRequest);
      }

      if(request.LevelCount > 8)
      {
        // Sent upon reporting by a device/port if it has more than 8 levels.
        var lLevelCountRequest = MsgCmdLevelCount.CreateRequest(dest, source, request.LevelCount);

        await SendAsync(lLevelCountRequest);
      }

      for(ushort port = 1; port <= request.PortCount; port++)
      {
        source = new AmxDevice(source.Device, port, source.System);

        // MsgCmdStringSize:
        // It is sent by a device/port upon reporting if the device/port supports more than 64 byte strings or more than 8-bit character strings.
        // It returns the maximum number of elements/string the device supports and the types of strings supported.
        var lStringSizeRequest = MsgCmdStringSize.CreateRequest(dest, source, EncodingType.Default, ushort.MaxValue);

        await SendAsync(lStringSizeRequest);

        // MsgCmdCommandSize:
        // It is sent by a device/port upon reporting if the device/port supports more than 64 byte commands or more than 8-bit character commands.
        // It returns the maximum number of elements/command the device supports and the types of strings supported.
        var lCommandSizeRequest = MsgCmdCommandSize.CreateRequest(dest, source, EncodingType.Default, ushort.MaxValue);

        await SendAsync(lCommandSizeRequest);
      }
    }

    public async Task RequestDevicesOnlineAsync(AmxDevice source)
    {
      var lDest = new AmxDevice(0, 0, CurrentSystem);

      var lRequest = MsgCmdRequestDevicesOnline.CreateRequest(lDest, source);

      await SendAsync(lRequest);
    }

    public async Task RequestDeviceStatusAsync(AmxDevice dest, AmxDevice source)
    {
      // System 0 does not works!
      if(dest.System == 0)
        dest = new AmxDevice(dest.Device, dest.Port, CurrentSystem);

      var lRequest = MsgCmdRequestDeviceStatus.CreateRequest(dest, source);

      await SendAsync(lRequest);
    }

    public async Task SendAsync(ICSPMsg request)
    {
      if(IsDisposed)
        throw new ObjectDisposedException("The current instance has been disposed!");

      if(mClient?.Connected ?? false)
        await mClient?.SendAsync(request);
      else
      {
        Logger.LogDebug(false, "ICSPManager.Send[1]: MessageId=0x{0:X4}, Type={1:l}", request.ID, request.GetType().Name);
        Logger.LogDebug(false, "ICSPManager.Send[2]: Source={0:l}, Dest={1:l}", request.Source, request.Dest);
        Logger.LogError(false, "ICSPManager.Send[3]: Client is offline");
      }
    }
  }
}