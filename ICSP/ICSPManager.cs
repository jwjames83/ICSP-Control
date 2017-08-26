using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

using ICSP.Client;
using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;
using ICSP.Manager;
using ICSP.Manager.ConnectionManager;
using ICSP.Manager.DeviceManager;
using ICSP.Manager.DiagnosticManager;
using ICSP.Reflection;

namespace ICSP
{
  public class ICSPManager
  {
    private Dictionary<ushort, Type> mMessages;

    private Dictionary<ushort, DeviceInfoData> mDevices;

    public event ClientConnectedEventHandler Connected;
    public event ClientConnectedEventHandler Disconnected;

    public event DynamicDeviceCreatedEventHandler DynamicDeviceCreated;
    public event MessageReceivedEventHandler MessageReceived;
    public event DeviceInfoEventHandler DeviceInfo;
    public event PortCountEventHandler PortCount;
    public event ChannelEventHandler ChannelEvent;

    private SynchronizationContext mSyncContext;

    private ICSPClient mClient;

    public ICSPManager()
    {
      mSyncContext = new SynchronizationContext();

      mMessages = new Dictionary<ushort, Type>();

      mDevices = new Dictionary<ushort, DeviceInfoData>();

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

    public void SetSynchronizationContext(SynchronizationContext syncContext)
    {
      if(syncContext == null)
        throw new ArgumentNullException(nameof(syncContext));

      mSyncContext = syncContext;
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
        mClient.ClientConnected -= OnClientConnected;
        mClient.ClientDisconnected -= OnClientDisconnected;
        mClient.DataReceived -= OnDataReceived;
        mClient.Dispose();
      }

      mClient = new ICSPClient();

      mClient.ClientConnected += OnClientConnected;
      mClient.ClientDisconnected += OnClientDisconnected;
      mClient.DataReceived += OnDataReceived;

      mClient.Connect(Host, Port);
    }

    public void Disconnect()
    {
      if(mClient != null)
        mClient.Disconnect();
    }

    private void OnClientConnected(object sender, ClientConnectedEventArgs e)
    {
      var lRequest = MsgCmdDynamicDeviceAddressRequest.CreateRequest(mClient.LocalIpAddress);

      Send(lRequest);

      if(Connected != null)
        mSyncContext?.Send(x => Connected(this, e), null);
    }

    private void OnClientDisconnected(object sender, ClientConnectedEventArgs e)
    {
      CurrentSystem = 0;

      DynamicDevice = AmxDevice.Empty;

      mDevices.Clear();

      if(Disconnected != null)
        mSyncContext?.Send(x => Disconnected(this, e), null);
    }

    private void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
      try
      {
        Logger.LogDebug("{0} Bytes", e.Bytes.Length);

        var lMsgData = GetMsgData(e.Bytes);

        var lLastId = lMsgData.LastOrDefault().ID;

        foreach(var lData in lMsgData)
        {
          Type lMsgType = null;

          if(mMessages.ContainsKey(lData.Command))
            lMsgType = mMessages[lData.Command];
          
          if(lMsgType == null)
          {
            Logger.LogDebug(false, "-----------------------------------------------------------");

            Logger.LogWarn("Command: 0x{0:X4} ({1}) => Command not implemented", lData.Command, ICSPMsg.GetFrindlyName(lData.Command));

            continue;
          }

          var lMsg = TypeHelper.CreateInstance(lMsgType, lData) as ICSPMsg;

          if(lMsg == null)
            return;

          lMsg.WriteLog(lData.ID == lLastId);

          var lMessageArgs = new MessageReceivedEventArgs(lMsg);

          if(MessageReceived != null)
            mSyncContext?.Send(d => MessageReceived(this, lMessageArgs), null);

          // Keine weitere Aktionen erforderlich
          if(lMessageArgs.Handled)
            return;

          switch(lMsg.Command)
          {
            case ConnectionManagerCmd.PingRequest:
              {
                var lRequest = lMsg as MsgCmdPingRequest;

                if(lRequest != null && mDevices.ContainsKey(lRequest.Device))
                {
                  var lDeviceInfo = mDevices[lRequest.Device];

                  if(lDeviceInfo != null)
                  {
                    var lResponse = MsgCmdPingResponse.CreateRequest(
                      lDeviceInfo.Device, lDeviceInfo.System, lDeviceInfo.ManufactureId, lDeviceInfo.DeviceId, mClient.LocalIpAddress);

                    Send(lResponse);
                  }
                }

                break;
              }
            case ConnectionManagerCmd.DynamicDeviceAddressResponse:
              {
                var lResponse = lMsg as MsgCmdDynamicDeviceAddressResponse;

                if(lResponse != null)
                {
                  CurrentSystem = lResponse.System;

                  DynamicDevice = new AmxDevice(lResponse.Device, 1, lResponse.System);
                  
                  var lDeviceInfo = new DeviceInfoData(lResponse.Device, lResponse.System, mClient.LocalIpAddress);

                  if(!mDevices.ContainsKey(lDeviceInfo.Device))
                    mDevices.Add(lDeviceInfo.Device, lDeviceInfo);

                  var lRequest = MsgCmdDeviceInfo.CreateRequest(lDeviceInfo);

                  Send(lRequest);

                  if(DynamicDeviceCreated != null)
                    mSyncContext?.Send(d => DynamicDeviceCreated(this, new DynamicDeviceCreatedArgs(lResponse.System, lResponse.Device)), null);
                }

                break;
              }
            case DeviceManagerCmd.RequestDeviceInfo:
              {
                var lRequest = lMsg as MsgCmdRequestDeviceInfo;

                if(lRequest != null)
                {
                  if(lRequest.Device == DynamicDevice.Device && lRequest.System == DynamicDevice.System)
                  {
                    var lDeviceInfo = new DeviceInfoData(lRequest.Device, lRequest.System, mClient.LocalIpAddress);

                    var lResponse = MsgCmdDeviceInfo.CreateRequest(lDeviceInfo);

                    Send(lResponse);
                  }
                }

                break;
              }
            case DeviceManagerCmd.RequestStatus:
              {
                var lResponse = MsgCmdStatus.CreateRequest(lMsg.Source, lMsg.Dest, lMsg.Dest, StatusType.Normal, 1, "Normal");

                Send(lResponse);

                break;
              }
            case DeviceManagerCmd.OutputChannelOn:
              {
                var lResponse = lMsg as MsgCmdOutputChannelOn;

                if(ChannelEvent != null)
                  mSyncContext?.Send(d => ChannelEvent(this, new ChannelEventArgs(lResponse.Device.Port, lResponse.Channel, true)), null);

                break;
              }
            case DeviceManagerCmd.OutputChannelOff:
              {
                var lResponse = lMsg as MsgCmdOutputChannelOff;

                if(ChannelEvent != null)
                  mSyncContext?.Send(d => ChannelEvent(this, new ChannelEventArgs(lResponse.Device.Port, lResponse.Channel, false)), null);

                break;
              }
            case DeviceManagerCmd.DeviceInfo:
              {
                var lResponse = lMsg as MsgCmdDeviceInfo;

                if(CurrentSystem > 0 && DeviceInfo != null)
                {
                  var lArgs = new DeviceInfoEventArgs(lResponse.Device, lResponse.System);

                  lArgs.DataFlag = lResponse.DataFlag;
                  lArgs.ObjectId = lResponse.ObjectId;
                  lArgs.ParentId = lResponse.ParentId;
                  lArgs.ManufactureId = lResponse.ManufactureId;
                  lArgs.DeviceId = lResponse.DeviceId;
                  lArgs.SerialNumber = lResponse.SerialNumber;
                  lArgs.FirmwareId = lResponse.FirmwareId;
                  lArgs.Version = lResponse.Version;
                  lArgs.Name = lResponse.Name;
                  lArgs.Manufacture = lResponse.Manufacture;

                  lArgs.ExtAddressType = lResponse.ExtAddressType;
                  lArgs.ExtAddressLength = lResponse.ExtAddressLength;
                  lArgs.ExtAddress = lResponse.ExtAddress;
                  lArgs.ExtAddress = lResponse.ExtAddress;

                  lArgs.IPv4Address = lResponse.IPv4Address;
                  lArgs.IpPort = lResponse.IpPort;
                  lArgs.MacAddress = lResponse.MacAddress;
                  lArgs.IPv6Address = lResponse.IPv6Address;

                  mSyncContext?.Send(d => DeviceInfo(this, lArgs), null);
                }

                break;
              }
            case DeviceManagerCmd.PortCountBy:
              {
                var lResponse = lMsg as MsgCmdPortCountBy;

                if(PortCount != null)
                  mSyncContext?.Send(d => PortCount(this, new PortCountEventArgs(lResponse.Device, lResponse.System, lResponse.PortCount)), null);

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
      var lRequest = MsgCmdStringDevMaster.CreateRequest(DynamicDevice, device, text);

      Send(lRequest);
    }

    public void SendCommand(AmxDevice device, string text)
    {
      var lRequest = MsgCmdCommandDevMaster.CreateRequest(DynamicDevice, device, text);

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
      var lRequest = MsgCmdLevelValueDevMaster.CreateRequest(DynamicDevice, device, level, value);

      Send(lRequest);
    }

    public void CreateDeviceInfo(DeviceInfoData deviceInfo)
    {
      if(!mDevices.ContainsKey(deviceInfo.Device))
        mDevices.Add(deviceInfo.Device, deviceInfo);

      var lRequest = MsgCmdDeviceInfo.CreateRequest(deviceInfo);

      Send(lRequest);
    }

    public void RequestDevicesOnline(ushort device, ushort system)
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
      var lSize = 0;

      while(lMsgBytes.Length >= 3)
      {
        // +4 => Protocol (1), Length (2), Checksum (1)
        lSize = lMsgBytes.GetBigEndianInt16(1) + 4;

        var lMsg = ICSPMsgData.FromMessage(msg.Skip(lOffset).Take(lSize).ToArray());

        if(lMsg.Command != ICSPMsgData.Empty.Command)
          lMessages.Add(lMsg);

        lOffset += lSize;

        lMsgBytes = msg.Skip(lOffset).Take(msg.Length).ToArray();
      }

      return lMessages;
    }

    public ushort CurrentSystem { get; private set; }

    public AmxDevice DynamicDevice { get; private set; }

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