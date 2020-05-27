using System;
using System.Collections.Generic;
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
    private readonly Dictionary<ushort, DeviceInfoData> mDevices;

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

    private ICSPClient mClient;

    private int mConnectionTimeout = 1;

    private int mTaskConnectAsyncRunning = 0;

    private bool mIsDisposed;

    public ICSPManager()
    {
      mDevices = new Dictionary<ushort, DeviceInfoData>();

      FileManager = new FileManager(this);
    }

    public ushort CurrentSystem { get; private set; }

    public AmxDevice DynamicDevice { get; private set; }

    public bool IsConnected
    {
      get
      {
        return mClient?.Connected ?? false;
      }
    }

    public IPAddress CurrentRemoteIpAddress
    {
      get
      {
        return mClient?.RemoteIpAddress;
      }
    }

    public IPAddress CurrentLocalIpAddress
    {
      get
      {
        return mClient?.LocalIpAddress;
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

      // GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if(!mIsDisposed)
      {
        if(disposing)
        {
          // Verwalteten Zustand (verwaltete Objekte) entsorgen
          if(mClient != null)
          {
            mClient.Dispose();
            mClient = null;
          }
        }

        mIsDisposed = true;
      }
    }

    public async Task ConnectAsync(string host)
    {
      await ConnectAsync(host, ICSPClient.DefaultPort);
    }

    public async Task ConnectAsync(string host, int port)
    {
      // Ensure that method would be called only once
      if(Interlocked.Exchange(ref mTaskConnectAsyncRunning, 1) != 0)
        return;

      try
      {
        Host = host ?? throw new ArgumentNullException(nameof(host));

        Port = port;

        if(mClient != null)
        {
          mClient.ClientOnlineStatusChanged -= OnClientOnlineStatusChanged;
          mClient.DataReceived -= OnDataReceived;
          mClient.Dispose();
        }

        mClient = new ICSPClient() { ConnectionTimeout = mConnectionTimeout };

        mClient.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
        mClient.DataReceived += OnDataReceived;

        await mClient.ConnectAsync(Host, Port);
      }
      finally
      {
        Interlocked.Exchange(ref mTaskConnectAsyncRunning, 0);
      }
    }

    public void Disconnect()
    {
      if(mClient != null)
        mClient.Disconnect();
    }

    private void OnClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      if(e.ClientOnline)
      {
        // Send(MsgCmdDynamicDeviceAddressRequest.CreateRequest(mClient.LocalIpAddress));
      }
      else
      {
        CurrentSystem = 0;

        DynamicDevice = AmxDevice.Empty;

        mDevices.Clear();

        var lKeys = MemoryCache.Default.Select(s => s.Key).ToList();

        foreach(var key in lKeys)
          MemoryCache.Default.Remove(key);
      }

      ClientOnlineStatusChanged?.Invoke(this, e);
    }

    private void OnDataReceived(object sender, ICSPMsgDataEventArgs e)
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

        switch(e.Message)
        {
          case MsgCmdAck m:
          {
            if(MemoryCache.Default.Get(m.ID.ToString()) is DeviceInfoData deviceInfo)
            {
              MemoryCache.Default.Remove(m.ID.ToString());

              deviceInfo.System = m.Source.System;

              if(!mDevices.ContainsKey(deviceInfo.Device))
                mDevices.Add(deviceInfo.Device, deviceInfo);

              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
              Logger.LogDebug(false, "Device Online: Device={0}, Name={1:l}, IPv4Address={2:l}", deviceInfo.Device, deviceInfo.Name, deviceInfo.IPv4Address);
              Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

              DeviceOnline?.Invoke(this, deviceInfo);
            }

            break;
          }
          case MsgCmdFileTransfer m:
          {
            FileManager.ProcessMessage(m);

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

              var lPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(6), RemovedCallback = OnCacheEntryRemovedCallback };

              MemoryCache.Default.AddOrGetExisting(lKey, lDeviceInfo, lPolicy);

              var lResponse = MsgCmdPingResponse.CreateRequest(
                lDeviceInfo.Device, lDeviceInfo.System, lDeviceInfo.ManufactureId, lDeviceInfo.DeviceId, mClient.LocalIpAddress);

              Send(lResponse);

              PingEvent?.Invoke(this, new PingEventArgs(m));
            }

            break;
          }
          case MsgCmdDynamicDeviceAddressResponse m:
          {
            CurrentSystem = m.System;

            DynamicDevice = new AmxDevice(m.Device, 1, m.System);

            CreateDeviceInfo(new DeviceInfoData(m.Device, mClient.LocalIpAddress) { System = m.System });

            DynamicDeviceCreated?.Invoke(this, new DynamicDeviceCreatedEventArgs(m));

            break;
          }
          case MsgCmdRequestEthernetIp m:
          {
            if(mDevices.ContainsKey(m.Dest.Device))
            {
              var lRequest = MsgCmdGetEthernetIpAddress.CreateRequest(m.Source, m.Dest, mClient.LocalIpAddress);

              Send(lRequest);
            }

            break;
          }
          case MsgCmdRequestDeviceInfo m:
          {
            if(m.Device == DynamicDevice.Device && m.System == DynamicDevice.System)
            {
              var lDeviceInfo = new DeviceInfoData(m.Device, mClient.LocalIpAddress) { System = m.System };

              var lDest = m.Source;

              var lSource = DynamicDevice;

              var lResponse = MsgCmdDeviceInfo.CreateRequest(lDest, lSource, lDeviceInfo);

              Send(lResponse);
            }

            break;
          }
          case MsgCmdRequestStatus m:
          {
            var lResponse = MsgCmdStatus.CreateRequest(e.Message.Source, e.Message.Dest, e.Message.Dest, StatusType.Normal, 1, "Normal");

            Send(lResponse);

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
              CurrentSystem = m.System;

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
          Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");
          Logger.LogDebug(false, "Device Offline: Device={0}, Name={1:l}, IPv4Address={2:l}", deviceInfo.Device, deviceInfo.Name, deviceInfo.IPv4Address);
          Logger.LogDebug(false, "-----------------------------------------------------------------------------------------------------");

          mDevices.Remove(deviceInfo.Device);

          DeviceOffline?.Invoke(this, deviceInfo);
        }
      }
    }

    public void SendString(AmxDevice device, string text)
    {
      var lRequest = MsgCmdStringMasterDev.CreateRequest(DynamicDevice, device, text);

      Send(lRequest);
    }

    public void SendCommand(AmxDevice device, string text)
    {
      var lRequest = MsgCmdCommandMasterDev.CreateRequest(DynamicDevice, device, text);

      Send(lRequest);
    }

    public void SetChannel(AmxDevice device, ushort channel, bool enabled)
    {
      if(enabled)
      {
        var lRequest = MsgCmdOutputChannelOn.CreateRequest(device, DynamicDevice, channel);

        Send(lRequest);
      }
      else
      {
        var lRequest = MsgCmdOutputChannelOff.CreateRequest(device, DynamicDevice, channel);

        Send(lRequest);
      }
    }

    public void SendLevel(AmxDevice device, ushort level, ushort value)
    {
      var lRequest = MsgCmdLevelValueMasterDev.CreateRequest(DynamicDevice, device, level, value);

      Send(lRequest);
    }

    public void CreateDeviceInfo(DeviceInfoData deviceInfo)
    {
      CreateDeviceInfo(deviceInfo, 1);
    }

    public void CreateDeviceInfo(DeviceInfoData deviceInfo, ushort portCount)
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

      var lPolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddSeconds(2), RemovedCallback = OnCacheEntryRemovedCallback };

      MemoryCache.Default.Set(lDeviceRequest.ID.ToString(), deviceInfo, lPolicy);

      Send(lDeviceRequest);

      if(portCount > 1)
      {
        // It is sent by a device upon reporting if the device has more than one port.
        var lPortCountRequest = MsgCmdPortCountBy.CreateRequest(lSource, deviceInfo.Device, deviceInfo.System, portCount);

        Send(lPortCountRequest);
      }
    }

    public void RequestDevicesOnline()
    {
      var lRequest = MsgCmdRequestDevicesOnline.CreateRequest(DynamicDevice);

      Send(lRequest);
    }

    public void RequestDeviceStatus(AmxDevice device)
    {
      // System 0 does not works!
      if(device.System == 0)
        device = new AmxDevice(device.Device, device.Port, DynamicDevice.System);

      var lRequest = MsgCmdRequestDeviceStatus.CreateRequest(DynamicDevice, device);

      Send(lRequest);
    }

    public void Send(ICSPMsg request)
    {
      if(mClient?.Connected ?? false)
        mClient?.SendAsync(request);
      else
      {
        Logger.LogDebug(false, "ICSPManager.Send[1]: MessageId=0x{0:X4}, Type={1:l}", request.ID, request.GetType().Name);
        Logger.LogDebug(false, "ICSPManager.Send[2]: Source={0:l}, Dest={1:l}", request.Source, request.Dest);
        Logger.LogError(false, "ICSPManager.Send[3]: Client is offline");
      }
    }
  }
}