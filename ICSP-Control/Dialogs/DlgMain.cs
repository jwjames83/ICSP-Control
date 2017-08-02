using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ICSP;
using ICSP.Client;
using ICSP.Constants;
using ICSP.Manager.DeviceManager;
using ICSP.Manager.DiagnosticManager;
using ICSPControl.Controls;
using ICSPControl.Properties;

using TpControls;

namespace ICSPControl.Dialogs
{
  public partial class DlgMain : Form
  {
    private System.Windows.Forms.Timer mBlinkTimer;

    private DlgNotifications mDlgNotifications;
    private DlgTest mDlgTest;

    private ICSPManager mICSPManager;

    public DlgMain()
    {
      InitializeComponent();

      mBlinkTimer = new System.Windows.Forms.Timer();
      mBlinkTimer.Interval = 500;
      mBlinkTimer.Tick += (s, e) => { tssl_Blink.BackColor = SystemColors.Control; };

      // Fix
      // The StatusStrip.Padding property is borked, it returns the wrong value for 
      // Padding Right if the sizing grip is disabled.
      MainStatusStrip.Padding =
        new Padding(MainStatusStrip.Padding.Left, MainStatusStrip.Padding.Top, MainStatusStrip.Padding.Left, MainStatusStrip.Padding.Bottom);

      OnlineTree.Nodes.Clear();
      OnlineTree.Nodes.Add("<Empty Device Tree>");

      tsmi_CommunicationSetttings.Click += OnCommunicationSetttingsClick;
      tsmi_Exit.Click += OnExitClick;

      tssl_Host.Text = string.Format("Host: {0}", Settings.Default.AmxHost);
      tssl_Port.Text = string.Format("Port: {0}", Settings.Default.AmxPort);

      // Physical Device
      tssl_Device.Text = string.Format("Device: {0}", Settings.Default.PhysicalDeviceNumber);

      txt_Text.Text = Settings.Default.LastSendText;

      cmd_CreatePhysicalDevice.Click += OnCreateDeviceInfo;

      // ContextMenu
      cmd_RefreshSystemOnlineTree.Click += OnRefreshSystemOnlineTreeClick;
      cmd_ShowDeviceProperties.Click += OnShowDevicePropertiesClick;

      // Add the event handler for handling UI thread exceptions to the event.
      Application.ThreadException += OnThreadException;

      mICSPManager = new ICSPManager();

      mICSPManager.Connected += OnManagerConnected;
      mICSPManager.Disconnected += OnManagerDisconnected;

      mICSPManager.DynamicDeviceCreated += OnDynamicDeviceCreated;
      mICSPManager.MessageReceived += OnDataReceived;
      mICSPManager.DeviceInfo += OnDeviceInfo;
      mICSPManager.PortCount += OnPortCount;

      cmd_Connect.Click += OnConnectClick;
      cmd_Disconnect.Click += OnDisconnectClick;

      cmd_ShowNotifications.Click += OnShowNotificationsClick;
      cmd_ShowFeedbackTest.Click += OnShowFeedbackTestClick;

      foreach(var lButton in GetControlsOfType<TpButton>(this))
        lButton.SetManager(mICSPManager);

      OnlineTree.MouseUp += OnlineTreeOnMouseUp;

      try
      {
        // mICSPManager.Connect(Settings.Default.AmxHost, Settings.Default.AmxPort);
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(this, ex.Message);
      }
    }
    
    private void OnDynamicDeviceCreated(object sender, DynamicDeviceCreatedArgs e)
    {
      tssl_CurrentSystem.Text = string.Format("Current System: {0}", e.System);
      tssl_DynamicDevice.Text = string.Format("Dynamic Device: {0:00000}", e.DynamicDevice);
    }

    private void OnShowNotificationsClick(object sender, EventArgs e)
    {
      if(mDlgNotifications == null || mDlgNotifications.IsDisposed)
        mDlgNotifications = new DlgNotifications(mICSPManager);

      mDlgNotifications.Show();
      mDlgNotifications.BringToFront();
    }

    private void OnShowFeedbackTestClick(object sender, EventArgs e)
    {
      if(mDlgTest == null || mDlgTest.IsDisposed)
        mDlgTest = new DlgTest(mICSPManager);

      mDlgTest.Show();
      mDlgTest.BringToFront();
    }
    
    private void OnCommunicationSetttingsClick(object sender, EventArgs e)
    {
      new DlgSettings().ShowDialog(this);

      tssl_Host.Text = string.Format("Host: {0}", Settings.Default.AmxHost);
      tssl_Port.Text = string.Format("Port: {0}", Settings.Default.AmxPort);
      tssl_Device.Text = string.Format("Device: {0}", Settings.Default.PhysicalDeviceNumber);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Settings.Default.LastSendText = txt_Text.Text;
      Settings.Default.Save();
    }

    private void OnExitClick(object sender, EventArgs e)
    {
      Application.Exit();
    }

    public static IEnumerable<T> GetControlsOfType<T>(Control root) where T : Control
    {
      var t = root as T;

      if(t != null)
        yield return t;

      foreach(Control c in root.Controls)
        foreach(var i in GetControlsOfType<T>(c))
          yield return i;
    }

    private void OnConnectClick(object sender, EventArgs e)
    {
      try
      {
        mICSPManager.Connect(Settings.Default.AmxHost, Settings.Default.AmxPort);
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(this, ex.Message);
      }
    }

    private void OnDisconnectClick(object sender, EventArgs e)
    {
      try
      {
        mICSPManager.Disconnect();
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex.Message);
      }
    }

    private void OnlineTreeOnMouseUp(object sender, MouseEventArgs e)
    {
      if(e.Button == MouseButtons.Right)
      {
        var lNode = OnlineTree.GetNodeAt(e.X, e.Y);

        if(lNode != null)
        {
          // Select the clicked node
          OnlineTree.SelectedNode = lNode;
        }

        cmd_ShowDeviceProperties.Enabled = lNode?.Tag is DeviceInfoEventArgs;

        cm_OnlineTree.Show(OnlineTree, e.Location);
      }
    }

    private void OnRefreshSystemOnlineTreeClick(object sender, EventArgs e)
    {
      OnlineTree.Nodes.Clear();
      OnlineTree.Nodes.Add("<Empty Device Tree>");

      mICSPManager?.RequestDevicesOnline(0, 1);
    }
    
    private void OnShowDevicePropertiesClick(object sender, EventArgs e)
    {
      var lNode = OnlineTree.SelectedNode;

      if(lNode == null)
        return;
      
      var lLocation = lNode.Bounds.Location;
      
      lLocation = OnlineTree.PointToScreen(lLocation);

      lLocation.Offset(10, 12);

      var lTitle = "Device Properties";

      var lInfo = lNode.Tag as DeviceInfoEventArgs;

      if(lInfo != null)
      {
        var lSb = new StringBuilder();
        
        // System
        if(lNode == OnlineTree.Nodes[0])
        {
          lSb.AppendFormat("System: {0:00000}\n", lInfo.System);

          if(lInfo.IPv4Address != null)
            lSb.AppendFormat("IPv4 Address: {0}\n", lInfo.IPv4Address);

          if(lInfo.MacAddress != null)
            lSb.AppendFormat("MAC Address: {0}\n", lInfo.MacAddress);

          if(lInfo.IPv6Address != null)
            lSb.AppendFormat("IPv6 Address: {0}\n", lInfo.IPv6Address);
        }
        else
        {
          if(lInfo.Device == 0 && lInfo.ObjectId == 0)
            lTitle = "NDP Device Properties";

          lSb.AppendFormat("Device: {0:00000}\n", lInfo.Device);
          lSb.AppendFormat("Description: {0}\n", lInfo.Name);
          lSb.AppendFormat("Manufacturer: {0}\n", lInfo.Manufacture);
          lSb.AppendFormat("Firmware ID: 0x{0:X4}\n", lInfo.FirmwareId);
          lSb.AppendFormat("Device ID: 0x{0:X4}\n", lInfo.DeviceId);
          lSb.AppendFormat("Manufacture ID: 0x{0:X4}\n", lInfo.ManufactureId);

          if(!string.IsNullOrWhiteSpace(lInfo.SerialNumber))
            lSb.AppendFormat("Serial Number: {0}\n", lInfo.SerialNumber);

          if(lInfo.IPv4Address != null)
            lSb.AppendFormat("IPv4 Address: {0}\n", lInfo.IPv4Address);

          if(lInfo.MacAddress != null)
            lSb.AppendFormat("MAC Address: {0}\n", lInfo.MacAddress);

          if(lInfo.IPv6Address != null)
            lSb.AppendFormat("IPv6 Address: {0}\n", lInfo.IPv6Address);
        }
        
        new BalloonTip(OnlineTree, lTitle, lSb.ToString(), BalloonTip.Icon.Info, 10000, false, (short)lLocation.X, (short)lLocation.Y);
      }
    }

    private void OnManagerConnected(object sender, ClientConnectedEventArgs e)
    {
      tssl_ClientState.Text = "Connected";
      tssl_ClientState.BackColor = Color.Green;
    }

    private void OnManagerDisconnected(object sender, ClientConnectedEventArgs e)
    {
      tssl_ClientState.Text = "Not Connected";
      tssl_ClientState.BackColor = Color.Red;

      tssl_CurrentSystem.Text = "Current System: 0";
      tssl_DynamicDevice.Text = "Dynamic Device: 00000";

      tssl_ProgramName.Text = "Program Name: ";
      tssl_MainFile.Text = "Main File: ";
    }

    private void OnDataReceived(object sender, MessageReceivedEventArgs e)
    {
      if(e.Message.Command == DiagnosticManagerCmd.RequestDevicesOnlineEOT)
      {
        if(OnlineTree.Nodes.Count > 0)
        {
          OnlineTree.Nodes[0].Expand();

          var lNode = OnlineTree.Nodes[0].Nodes["Virtual"];

          if(lNode != null)
          {
            OnlineTree.Nodes[0].Nodes.RemoveByKey("Virtual");

            OnlineTree.Nodes[0].Nodes.Add(lNode);

            lNode.Expand();
          }
        }

        // Request ProgramInfo ...

        var lRequest = MsgCmdProbablyRequestProgramInfo.CreateRequest(mICSPManager.DynamicDevice, 0x1F);

        mICSPManager.Send(lRequest);
      }

      if(e.Message.Command == DiagnosticManagerCmd.ProbablyProgramInfo)
      {
        var lMsg = e.Message as MsgCmdProbablyProgramInfo;

        tssl_ProgramName.Text = string.Format("Program Name: {0}", lMsg.ProgramName);
        tssl_MainFile.Text = string.Format("Main File: {0}", lMsg.MainFile);
      }

      if(e.Message.Command == ConnectionManagerCmd.BlinkMessage)
      {
        mBlinkTimer.Stop();

        tssl_Blink.BackColor = Color.Green;

        mBlinkTimer.Start();
      }
    }

    private void OnDeviceInfo(object sender, DeviceInfoEventArgs e)
    {
      TreeNode lNode = null;

      var lImageKey = "AMXDeviceDefault";
      var lSelectedImageKey = "AMXDeviceSelected";

      // System-Device
      if(e.Device == 0 && e.ObjectId == 0)
      {
        OnlineTree.Nodes.Clear();

        lNode = OnlineTree.Nodes.Add("System", string.Format("System {0} [{1}]", e.System, e.IPv4Address), lImageKey, lSelectedImageKey);
        lNode.Tag = e;

        // Add Dynamic/Virtual Devices
        lNode.Nodes.Add("Virtual", "Dynamic/Virtual Devices", "VirtualDeviceDefault", "VirtualDeviceSelected");
      }

      var lKeyCurrent = string.Format("{0}-{1}", e.Device, e.ObjectId);
      var lKeyParent = string.Format("{0}-{1}", e.Device, e.ParentId);

      if(e.Device > 32000)
      {
        lImageKey = "CloudDefault";
        lSelectedImageKey = "CloudSelected";

        lKeyParent = "Virtual";
      }

      var lTxt = string.Format("{0:00000} - {1} ({2})", e.Device, e.Name, e.Version);

      if(e.ObjectId > 0)
      {
        lTxt = string.Format("[OID={0}] - {1} ({2})", e.ObjectId, e.Name, e.Version);

        lImageKey = "OIDDeviceDefault";
        lSelectedImageKey = "OIDDeviceSelected";
      }

      var lNodes = OnlineTree.Nodes[0].Nodes.Find(lKeyParent, true);

      if(lNodes.Length == 0)
        lNode = OnlineTree.Nodes[0].Nodes.Add(lKeyCurrent, lTxt, lImageKey, lSelectedImageKey);
      else
        lNode = lNodes[0].Nodes.Add(lKeyCurrent, lTxt, lImageKey, lSelectedImageKey);

      lNode.Tag = e;
    }


    private void OnPortCount(object sender, PortCountEventArgs e)
    {
      var lImageKey = "IODeviceDefault";
      var lSelectedImageKey = "IODeviceSelected";

      var lDevice = OnlineTree.Nodes.Find(string.Format("{0}-0", e.Device), true).FirstOrDefault();

      if(lDevice != null)
      {
        // PadLeft
        var lTotalWidth = e.PortCount.ToString().Length;

        for(var i = 1; i <= e.PortCount; i++)
        {
          var lTxt = string.Format("Port - {1}", e.Device, i.ToString().PadLeft(lTotalWidth));

          var lNode = lDevice.Nodes.Add(string.Format("{0}-IO-{1}", e.Device, i), lTxt, lImageKey, lSelectedImageKey);

          lNode.Tag = e;
        }
      }

    }

    private void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
      ErrorMessageBox.Show(this, e.Exception.Message);
    }

    private void OnCmdChannelOn(object sender, EventArgs e)
    {
      mICSPManager?.SetChannel(GetDevice(), (ushort)num_Channel.Value, true);
    }

    private void OnCmdChannelOff(object sender, EventArgs e)
    {
      mICSPManager?.SetChannel(GetDevice(), (ushort)num_Channel.Value, false);
    }

    private void OnCmdSendLevel(object sender, EventArgs e)
    {
      mICSPManager?.SendLevel(GetDevice(), (ushort)num_LevelInput.Value, (ushort)num_LevelValue.Value);
    }

    private void OnCmdSendString(object sender, EventArgs e)
    {
      if(txt_Text.SelectedText.Length > 0)
        mICSPManager?.SendString(GetDevice(), txt_Text.SelectedText);
      else
        mICSPManager?.SendString(GetDevice(), txt_Text.Text);
    }

    private void OnCmdSendCmd(object sender, EventArgs e)
    {
      if(txt_Text.SelectedText.Length > 0)
        mICSPManager?.SendCommand(GetDevice(), txt_Text.SelectedText);
      else
        mICSPManager?.SendCommand(GetDevice(), txt_Text.Text);
    }

    private void OnCreateDeviceInfo(object sender, EventArgs e)
    {
      var lDeviceInfo = new DeviceInfoData(
        Settings.Default.PhysicalDeviceNumber,
        mICSPManager.CurrentSystem,
        mICSPManager.CurrentLocalIpAddress);

      lDeviceInfo.Version = Settings.Default.PhysicalDeviceVersion;
      lDeviceInfo.Name = Settings.Default.PhysicalDeviceName;
      lDeviceInfo.Manufacture = Settings.Default.PhysicalDeviceManufacturer;
      lDeviceInfo.SerialNumber = Settings.Default.PhysicalDeviceSerialNumber;
      lDeviceInfo.ManufactureId = Settings.Default.PhysicalDeviceManufactureId;
      lDeviceInfo.DeviceId = Settings.Default.PhysicalDeviceDeviceId;
      lDeviceInfo.FirmwareId = Settings.Default.PhysicalDeviceFirmwareId;
      
      mICSPManager?.CreateDeviceInfo(lDeviceInfo);
    }

    private void OnCmdRequestDeviceStatus(object sender, EventArgs e)
    {
      mICSPManager?.RequestDeviceStatus(GetDevice());
    }
    
    private AmxDevice GetDevice()
    {
      return new AmxDevice((ushort)num_Device.Value, (ushort)num_DevPort.Value, (ushort)num_System.Value);
    }
  }
}