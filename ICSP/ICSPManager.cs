using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ICSP
{
  public class ICSPManager
  {
    private readonly Dictionary<ushort, Type> mMessages;

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
    public event EventHandler<ProgramInfoEventArgs> ProgramInfo;

    private readonly FileManager mFileManager;
    private ICSPClient mClient;

    private readonly StateManager mStateManager;

    public ICSPManager()
    {
      mMessages = new Dictionary<ushort, Type>();

      mDevices = new Dictionary<ushort, DeviceInfoData>();

      mStateManager = new StateManager(this);
      
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

    private void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
      try
      {
        Logger.LogDebug("{0} Bytes", e.Bytes.Length);
        Logger.LogDebug("Data 0x: {0}", BitConverter.ToString(e.Bytes).Replace("-", " "));

        var lMsgData = GetMsgData(e.Bytes);

        var lLastId = lMsgData.LastOrDefault().ID;

        foreach(var lData in lMsgData)
        {
          var lDataEventArgs = new ICSPMsgDataEventArgs(lData);

          DataReceived?.Invoke(this, lDataEventArgs);

          // No action needed
          if(lDataEventArgs.Handled)
            return;

          Type lMsgType = null;

          if(mMessages.ContainsKey(lData.Command))
            lMsgType = mMessages[lData.Command];

          if(lMsgType == null)
          {
            Logger.LogDebug(false, "-----------------------------------------------------------");

            Logger.LogWarn("Command: 0x{0:X4} ({1}) => Command not implemented", lData.Command, ICSPMsg.GetFrindlyName(lData.Command));

            CommandNotImplemented?.Invoke(this, lDataEventArgs);

            continue;
          }

          if(!(TypeHelper.CreateInstance(lMsgType, lData) is ICSPMsg lMsg))
            return;

          // Speed up
          if(Logger.LogLevel <= Serilog.Events.LogEventLevel.Debug)
            lMsg.WriteLog(lData.ID == lLastId);

          switch(lMsg)
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
              var lResponse = MsgCmdStatus.CreateRequest(lMsg.Source, lMsg.Dest, lMsg.Dest, StatusType.Normal, 1, "Normal");

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
            case MsgCmdProbablyProgramInfo m:
            {
              ProgramInfo?.Invoke(this, new ProgramInfoEventArgs(m));

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
            default:
            {
              MessageReceived?.Invoke(this, new MessageReceivedEventArgs(lMsg));

              break;
            }
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
        mClient?.Send(request);
      else
      {
        Logger.LogDebug(false, "ICSPManager.Send[1]: MessageId=0x{0:X4}, Type={1}", request.ID, request.GetType().Name);
        Logger.LogDebug(false, "ICSPManager.Send[2]: Source={0}, Dest={1}", request.Source, request.Dest);
        Logger.LogError(false, "ICSPManager.Send[3]: Client is offline");
      }
    }

    private List<ICSPMsgData> GetMsgData(byte[] msg)
    {
      var lMsgBytes = msg;
      var lMessages = new List<ICSPMsgData>();
      var lOffset = 0;

      while(lMsgBytes.Length >= 3)
      {
        // +4 => Protocol (1), Length (2), Checksum (1)
        var lSize = ((lMsgBytes[1] << 8) | lMsgBytes[2]) + 4;

        lMsgBytes = new byte[lSize];
        Array.Copy(msg, lOffset, lMsgBytes, 0, lSize);

        var lMsg = ICSPMsgData.FromMessage(lMsgBytes);

        if(lMsg.Command != ICSPMsgData.Empty.Command)
          lMessages.Add(lMsg);

        lOffset += lSize;

        lMsgBytes = new byte[msg.Length - lOffset];
        Array.Copy(msg, lOffset, lMsgBytes, 0, lMsgBytes.Length);
      }

      return lMessages;
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