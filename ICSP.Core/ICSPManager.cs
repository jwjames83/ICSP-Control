using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

using ICSP.Core.Client;
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

    // in work ...
    public const int DeviceUninitialized = 0;
    public const int DeviceUnconfigured = 1;
    public const int DeviceUnverified = 2;
    public const int DeviceConnected = 3;

    public ICSPManager()
    {
      mDevices = new ConcurrentDictionary<ushort, DeviceInfoData>();

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
        Logger.LogVerbose("{0} Bytes", e.Message.RawData.Length);
        Logger.LogVerbose("Data 0x: {0:l}", BitConverter.ToString(e.Message.RawData).Replace("-", " "));

        DataReceived?.Invoke(this, e);

        // No action needed
        if(e.Handled)
          return;

        // Speed up
        if(Logger.LogLevel <= LogEventLevel.Verbose)
          e.Message.WriteLogVerbose();

        //      P | Len   | Flag  | Dest (SDP)        | Source (SDP)      | H  | ID    | CMD   | N-Data      | CS
        // ------------------------------------------------------------------------------------------------------------------------------------------------ -
        //    02 | 01 41 | 02 08 | 00 D1 7D 01 00 01 | 00 D1 00 00 00 01 | 0F | 00 02 | 07 31 | FA EF 21 37 | 00 00 01 26 30 82 01 22 30 0D 06 09 2A 86 48 86 F7 0D 01 01 01 05 00 03 82 01 0F 00 30 82 01 0A 02 82 01 01 00 F2 28 3A 0E 6A 79 1B DC 22 86 6D F1 B1 62 11 F2 18 9C 5A 57 F6 57 3E 8A D2 C5 B4 2D 67 C1 20 B7 FD EF 15 57 49 CA EB E4 A3 E2 4F 38 7A 1C 08 40 61 31 DA 11 F3 68 A3 05 78 04 B2 66 53 0F 52 C2 DC 89 D5 21 15 FE 0A 60 A0 BF 62 31 80 46 96 AD 57 FD D4 09 42 E8 BE 1C 4A 2E 73 06 26 B4 B2 C7 23 8B 54 9A 8F 04 5D 0C A9 E4 2D BC DF 1D 0F AD 0E CF B4 54 94 8A 7B A4 D8 45 93 2D C2 EE 6E 02 71 BB D3 C7 00 DD 17 99 58 AD 6F 4B F6 7B BD CC 80 DA 0F 00 54 5D 83 E3 93 03 60 53 EB 73 99 34 37 54 C0 D4 5E 44 28 08 CA 69 FB 09 A4 79 23 65 AD 6B F6 7A A0 3A 2B 79 B0 64 29 49 C4 DF 49 7D F5 DD 21 67 05 19 85 7F 59 D1 25 88 31 E8 42 6F 3D 49 B9 61 B5 D0 F7 97 4E 91 AA FF B3 ED 1C 71 7B 5F 2D 0B D6 3E 3D 7D DC 1F 6E FE 4B 88 91 B4 54 6A 7E D5 5B 7D 30 3F 3C 0D BA 7A 2F 51 F1 D3 02 03 01 00 01 | 92
        // <- 02 | 01 41 | 02 08 | 00 d1 7d 01 00 00 | 00 d1 00 00 00 01 | 0f | 04 d7 | 07 31 | 8f e8 0c 50 | 00 00 01 26 30 82 01 22 30 0d 06 09 2a 86 48 86 f7 0d 01 01 01 05 00 03 82 01 0f 00 30 82 01 0a 02 82 01 01 00 f2 28 3a 0e 6a 79 1b dc 22 86 6d f1 b1 62 11 f2 18 9c 5a 57 f6 57 3e 8a d2 c5 b4 2d 67 c1 20 b7 fd ef 15 57 49 ca eb e4 a3 e2 4f 38 7a 1c 08 40 61 31 da 11 f3 68 a3 05 78 04 b2 66 53 0f 52 c2 dc 89 d5 21 15 fe 0a 60 a0 bf 62 31 80 46 96 ad 57 fd d4 09 42 e8 be 1c 4a 2e 73 06 26 b4 b2 c7 23 8b 54 9a 8f 04 5d 0c a9 e4 2d bc df 1d 0f ad 0e Cf b4 54 94 8a 7b a4 d8 45 93 2d c2 ee 6e 02 71 Bb d3 c7 00 dd 17 99 58 ad 6f 4b f6 7b bd cc 80 da 0f 00 54 5d 83 e3 93 03 60 53 eb 73 99 34 37 54 c0 d4 5e 44 28 08 ca 69 fb 09 a4 79 23 65 ad 6b f6 7a a0 3a 2b 79 b0 64 29 49 c4 df 49 7d f5 dd 21 67 05 19 85 7f 59 d1 25 88 31 e8 42 6f 3d 49 b9 61 b5 d0 f7 97 4e 91 aa ff b3 ed 1c 71 7b 5f 2d 0b d6 3e 3d 7d dc 1f 6e fe 4b 88 91 b4 54 6a 7e d5 5b 7d 30 3f 3c 0d ba 7a 2f 51 f1 d3 02 03 01 00 01 | fc
        // -> 02 | 01 17 | 02 00 | 00 00 00 00 00 00 | 00 d1 7d 01 00 00 | ff | 04 d8 | 07 32 | 00 01 01 00 8d 46 59 36 92 cd a2 0f ca 12 ef ee 8d 44 47 df 47 51 fc 96 c6 25 ac 82 89 4f d8 21 76 35 c3 08 9d 8e 99 e9 c0 08 f2 4e be 76 4e ab f5 21 91 e4 ca cf c0 00 1c c8 3d 38 3b e1 36 66 62 27 94 a1 7b 4e 9a 51 fe 71 37 4c e9 8f 5d 11 18 b8 5f cd 7f d7 5e c6 53 ec ec 9d 22 91 22 5d d4 55 c1 25 8a a3 b1 d0 b0 06 ce f2 5d e3 6c ae 09 87 98 93 03 95 a3 77 ac aa 39 68 eb e9 de ce 23 7e 49 00 e7 0c 8f 1c b0 62 f2 bc 37 da b0 67 1f 52 5e 83 34 27 57 8f aa 7d f5 27 e3 02 42 90 ed be 02 99 be ba f0 66 75 1e be d1 5f a5 10 23 89 35 6f 30 a6 43 20 03 34 87 a5 8c fc d9 1c 5d fe 7a d7 1f 86 3c bf e4 77 51 d2 29 73 b4 27 be b4 be a6 d7 70 95 3a e3 96 32 15 7f cf e6 62 1b 5a a8 03 f0 e0 3d 0e 5b 3c e9 b2 b4 b6 c7 6e 7e 64 01 04 c6 9f 1a 83 d6 6d fa 28 09 20 1b 08 35 a9 7e 9a 31 | 2f

        switch(e.Message)
        {
          case MsgCmdAck m:
          {
            var lKey = CreateDeviceInfoKey(m.ID);

            if(MemoryCache.Default.Get(lKey) is DeviceInfoData deviceInfo)
            {
              MemoryCache.Default.Remove(lKey);

              Logger.LogInfo("MsgCmdAck: ID=0x{0:X4}, Device={1}, System={2}, DeviceId={3}, Name={4:l}", m.ID, deviceInfo.Device, deviceInfo.System, deviceInfo.DeviceId, deviceInfo.Name);

              deviceInfo.System = m.Source.System;

              var lResult = mDevices.TryAdd(deviceInfo.Device, deviceInfo);

              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
              Logger.LogDebug(false, "Device Online: Device={0}, Name={1:l}, IPv4Address={2:l}", deviceInfo.Device, deviceInfo.Name, deviceInfo.IPv4Address);
              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

              DeviceOnline?.Invoke(this, deviceInfo);
            }
            else
            {
              Logger.LogWarn("MsgCmdAck: ID=0x{0:X4} unknown!", m.ID);
            }

            break;
          }
          case MsgCmdChallengeAckMD5 m:
          {
            if(m.Authenticated)
            {
              // TODO: Ugly code -> m.ID - 1 ...
              var lKey = CreateDeviceInfoKey((ushort)(m.ID - 1));

              if(MemoryCache.Default.Get(lKey) is DeviceInfoData deviceInfo)
              {
                MemoryCache.Default.Remove(lKey);

                Logger.LogInfo("MsgCmdAck: ID=0x{0:X4}, Device={1}, System={2}, DeviceId={3}, Name={4:l}", m.ID, deviceInfo.Device, deviceInfo.System, deviceInfo.DeviceId, deviceInfo.Name);

                deviceInfo.System = m.Source.System;

                var lResult = mDevices.TryAdd(deviceInfo.Device, deviceInfo);

                Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
                Logger.LogDebug(false, "Device Online: Device={0}, Name={1:l}, IPv4Address={2:l}", deviceInfo.Device, deviceInfo.Name, deviceInfo.IPv4Address);
                Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

                DeviceOnline?.Invoke(this, deviceInfo);
              }
              else
              {
                Logger.LogWarn("MsgCmdAck: ID=0x{0:X4} unknown!", m.ID);
              }
            }
            else
            {
              Logger.LogError("MsgCmdChallengeAckMD5: Authenticated failed!", m.ID);
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
            if(m.Device == 0 && m.ObjectId == 0)
            {
              CurrentSystem = m.System;

              SystemDevice = new AmxDevice(0, 1, m.System);
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
          case MsgCmdChallengeRequestMD5 m: // NI-700
          {
            // Authentication Challenge

            // TODO: Username, password ...

            // EncryptionType:
            // Netlinx Studio send 1 ...

            // Assumption:
            // 0: None => This value does not work, if the option [Encrypt ICSP connection] is enabled on the controller
            // 1: RC4
            // 2: Future1

            ushort lEncryptionType = 2; // (1: RC4, 2: Future1) ?

            var lResponse = MsgCmdChallengeResponseMD5.CreateRequest(m.Source, m.Dest, m.Challenge, lEncryptionType, "administrator", "password");

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
        if(arguments.CacheItem.Value is DeviceInfoData deviceInfo)
        {
          if(arguments.CacheItem.Key.StartsWith("CreateDeviceInfoAsync:"))
          {
            Logger.LogWarn(false, "No response for {0:l}, Device={1}, Name={2:l}, IPv4Address={3:l}", arguments.CacheItem.Key, deviceInfo.Device, deviceInfo.Name, deviceInfo.IPv4Address);
          }
          else
          {
            Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
            Logger.LogDebug(false, "Device Offline: Device={0}, Name={1:l}, IPv4Address={2:l}", deviceInfo.Device, deviceInfo.Name, deviceInfo.IPv4Address);
            Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

            var lResult = mDevices.TryRemove(deviceInfo.Device, out _);

            DeviceOffline?.Invoke(this, deviceInfo);
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

    public async Task CreateDeviceInfoAsync(DeviceInfoData deviceInfo)
    {
      await CreateDeviceInfoAsync(deviceInfo, 1, 256, 8);
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

      // 30 Sec: If controller is startup, response need a short short time ...
      var lPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(30), RemovedCallback = OnCacheEntryRemovedCallback };

      // Add this request to identify the Ack-Message
      MemoryCache.Default.Set(CreateDeviceInfoKey(lDeviceRequest.ID), deviceInfo, lPolicy);

      Logger.LogInfo("ID=0x{0:X4}, Device={1}, System={2}, DeviceId={3}, Name={4:l}, PortCount={5}", lDeviceRequest.ID, deviceInfo.Device, deviceInfo.System, deviceInfo.DeviceId, deviceInfo.Name, portCount);

      await SendAsync(lDeviceRequest);

      if(portCount > 1)
      {
        // It is sent by a device upon reporting if the device has more than one port.
        var lPortCountRequest = MsgCmdPortCountBy.CreateRequest(lDest, lSource, deviceInfo.Device, deviceInfo.System, portCount);

        await SendAsync(lPortCountRequest);
      }

      if(channelCount > 256)
      {
        // It is sent by a device/port upon reporting if the device has more than 256 channels.
        var lOutputChannelCountRequest = MsgCmdOutputChannelCount.CreateRequest(lDest, lSource, channelCount);

        await SendAsync(lOutputChannelCountRequest);
      }

      if(levelCount > 8)
      {
        // Sent upon reporting by a device/port if it has more than 8 levels.
        var lLevelCountRequest = MsgCmdLevelCount.CreateRequest(lDest, lSource, levelCount);

        await SendAsync(lLevelCountRequest);
      }

      /*
      for(ushort port = 1; port <= portCount; port++)
      {
        lSource = new AmxDevice(deviceInfo.Device, port, deviceInfo.System);

        // MsgCmdStringSize:
        // It is sent by a device/port upon reporting if the device/port supports more than 64 byte strings or more than 8-bit character strings.
        // It returns the maximum number of elements/string the device supports and the types of strings supported.
        var lStringSizeRequest = MsgCmdStringSize.CreateRequest(lDest, lSource, EncodingType.Default, ushort.MaxValue);

        await SendAsync(lStringSizeRequest);

        // MsgCmdCommandSize:
        // It is sent by a device/port upon reporting if the device/port supports more than 64 byte commands or more than 8-bit character commands.
        // It returns the maximum number of elements/command the device supports and the types of strings supported.
        var lCommandSizeRequest = MsgCmdCommandSize.CreateRequest(lDest, lSource, EncodingType.Default, ushort.MaxValue);

        await SendAsync(lCommandSizeRequest);
      }
      */
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

    private string CreateDeviceInfoKey(ushort id)
    {
      return string.Format("CreateDeviceInfoAsync:ID=0x{0:X4}", id);
    }
  }
}