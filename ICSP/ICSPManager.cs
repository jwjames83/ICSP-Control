using System;
using System.Collections.Generic;
using System.Net;

using ICSP.Client;
using ICSP.IO;
using ICSP.Logging;
using ICSP.Manager;
using ICSP.Manager.ConfigurationManager;
using ICSP.Manager.ConnectionManager;
using ICSP.Manager.DeviceManager;
using ICSP.Manager.DiagnosticManager;
using ICSP.Reflection;
using Serilog.Events;

namespace ICSP
{
  public class ICSPManager
  {
    private readonly Dictionary<ushort, Type> mMessages;
    private readonly Dictionary<ushort, ICSPMsg> mTypes;

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

    private readonly FileManager mFileManager;
    private ICSPClient mClient;

    private readonly StateManager mStateManager;

    public ICSPManager()
    {
      mMessages = new Dictionary<ushort, Type>();
      mTypes = new Dictionary<ushort, ICSPMsg>();

      mDevices = new Dictionary<ushort, DeviceInfoData>();

      mStateManager = new StateManager(this);

      // Suppress Warnings IDE0052, under development ...
      Console.WriteLine(mStateManager);

      mFileManager = new FileManager(this);

      var lTypes = TypeHelper.GetSublassesOfType(typeof(ICSPMsg));

      foreach(var type in lTypes)
      {
        if(type.IsAssignableFrom(typeof(ICSPMsg)))
          throw new ArgumentException("MessageType is not assignable from ICSPMsg", nameof(type));

        var lAttributes = AttributeHelper.GetList<MsgCmdAttribute>(type);

        foreach(var attribute in lAttributes)
        {
          if(!mMessages.ContainsKey(attribute.MsgCmd))
            mMessages.Add(attribute.MsgCmd, type);

          try
          {
            // Type type = typeof(Foo);

            var lType = (ICSPMsg)Activator.CreateInstance(type, true);

            // var lType = (ICSPMsg)TypeHelper.CreateInstance(type, ICSPMsgData.Empty);

            mTypes.Add(attribute.MsgCmd, lType);
          }
          catch(Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
      }
    }

    public void Connect(string host)
    {
      Connect(host, ICSPClient.DefaultPort);
    }

    public void Connect(string host, int port)
    {
      if(string.IsNullOrWhiteSpace(host))
        throw new ArgumentNullException(nameof(host));

      Host = host;

      Port = port;

      if(mClient != null)
      {
        mClient.ClientOnlineStatusChanged -= OnClientOnlineStatusChanged;
        mClient.DataReceived -= OnDataReceived;
        mClient.Dispose();
      }

      mClient = new ICSPClient();

      mClient.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
      mClient.DataReceived += OnDataReceived;

      mClient.Connect(Host, Port);
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
        var lRequest = MsgCmdDynamicDeviceAddressRequest.CreateRequest(mClient.LocalIpAddress);

        Send(lRequest);
      }
      else
      {
        CurrentSystem = 0;

        DynamicDevice = AmxDevice.Empty;

        mDevices.Clear();
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
          case MsgCmdFileTransfer m:
          {
            mFileManager.ProcessMessage(m);

            break;
          }
          case MsgCmdBlinkMessage m:
          {
            BlinkMessage?.Invoke(this, new BlinkEventArgs(m));

            break;
          }
          case MsgCmdPingRequest m:
          {
            if(mDevices.ContainsKey(m.Device))
            {
              var lDeviceInfo = mDevices[m.Device];

              if(lDeviceInfo != null)
              {
                var lResponse = MsgCmdPingResponse.CreateRequest(
                  lDeviceInfo.Device, lDeviceInfo.System, lDeviceInfo.ManufactureId, lDeviceInfo.DeviceId, mClient.LocalIpAddress);

                Send(lResponse);
              }

              PingEvent?.Invoke(this, new PingEventArgs(m));
            }

            break;
          }
          case MsgCmdDynamicDeviceAddressResponse m:
          {
            CurrentSystem = m.System;

            DynamicDevice = new AmxDevice(m.Device, 1, m.System);

            var lDeviceInfo = new DeviceInfoData(m.Device, m.System, mClient.LocalIpAddress);

            if(!mDevices.ContainsKey(lDeviceInfo.Device))
              mDevices.Add(lDeviceInfo.Device, lDeviceInfo);

            var lRequest = MsgCmdDeviceInfo.CreateRequest(lDeviceInfo);

            Send(lRequest);

            DynamicDeviceCreated?.Invoke(this, new DynamicDeviceCreatedEventArgs(m));

            // =======================================================================================
            // Test-Stuff ...
            // =======================================================================================

            /*
            var lDest = new AmxDevice(10001, 0, 1);

            var lDirectory = "/"; // Get Root-Directory

            Logger.LogDebug(false, "----------------------------------------------------------------");
            Logger.LogDebug(false, "GetDirectoryInfo: Directory={0:l}", lDirectory);
            Logger.LogDebug(false, "----------------------------------------------------------------");

            var lBytes = new byte[] { 0x00, 0x00, }.Concat(Encoding.Default.GetBytes(lDirectory + "\0")).ToArray();

            lRequest = MsgCmdFileTransfer.CreateRequest(lDest, DynamicDevice, FileType.Unused, FunctionsUnused.GetDirectoryInfo, lBytes);

            Send(lRequest);

            // doc:/user
            // .
            // ..
            // images

            lDirectory = "AMXPanel/"; // Get doc:/user ...

            Logger.LogDebug(false, "----------------------------------------------------------------");
            Logger.LogDebug(false, "GetDirectoryInfo: Directory={0:l}", lDirectory);
            Logger.LogDebug(false, "----------------------------------------------------------------");

            var lBytes = new byte[] { 0x00, 0x00, }.Concat(Encoding.Default.GetBytes(lDirectory + "\0")).ToArray();

            lRequest = MsgCmdFileTransfer.CreateRequest(lDest, DynamicDevice, FileType.Unused, FunctionsUnused.GetDirectoryInfo, lBytes);

            Send(lRequest);

            // doc:/user
            // .
            // ..
            // images
            
            Logger.LogDebug(false, "----------------------------------------------------------------");
            Logger.LogDebug(false, "TransferGetFileAccessToken: FileName=AMXPanel/Page.xml");
            Logger.LogDebug(false, "----------------------------------------------------------------");

            //  d0 07 | AMXPanel/Page.xml
            var lBytes = new byte[] { 0xd0, 0x07, 0x41, 0x4d, 0x58, 0x50, 0x61, 0x6e, 0x65, 0x6c, 0x2f, 0x50, 0x61, 0x67, 0x65, 0x2e, 0x78, 0x6d, 0x6c, 0x00, };

            // Read File (AMXPanel/Page.xml)
            lRequest = MsgCmdFileTransfer.CreateRequest(lDest, DynamicDevice, FileType.Axcess2Tokens, FunctionsAxcess2Tokens.TransferGetFileAccessToken, lBytes);

            Send(lRequest);
            
            */

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
              var lDeviceInfo = new DeviceInfoData(m.Device, m.System, mClient.LocalIpAddress);

              var lResponse = MsgCmdDeviceInfo.CreateRequest(lDeviceInfo);

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
        var lRequest = MsgCmdOutputChannelOn.CreateRequest(DynamicDevice, device, channel);

        Send(lRequest);
      }
      else
      {
        var lRequest = MsgCmdOutputChannelOff.CreateRequest(DynamicDevice, device, channel);

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
      if(!mDevices.ContainsKey(deviceInfo.Device))
        mDevices.Add(deviceInfo.Device, deviceInfo);

      var lDeviceRequest = MsgCmdDeviceInfo.CreateRequest(deviceInfo);

      Send(lDeviceRequest);

      if(portCount > 1)
      {
        var lSource = DynamicDevice;

        // lSource = new AmxDevice(deviceInfo.Device, 0, deviceInfo.System);

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
  }
}