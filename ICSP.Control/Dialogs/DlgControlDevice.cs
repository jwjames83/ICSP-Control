﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSP.Control.Properties;
using ICSP.Core;
using ICSP.Core.Manager.DeviceManager;

using ICSPControl.Controls;
using ICSPControl.Extensions;

namespace ICSPControl.Dialogs
{
  public partial class DlgControlDevice : WeifenLuo.WinFormsUI.Docking.DockContent
  {
    private readonly ICSPManager mManager;

    public DlgControlDevice(ICSPManager manager)
    {
      InitializeComponent();

      mManager = manager ?? throw new ArgumentNullException(nameof(manager));

      OnlineTree.Nodes.Clear();
      OnlineTree.Nodes.Add("<Empty Device Tree>");

      txt_Text.Text = Settings.Default.LastSendText;

      cmd_CreatePhysicalDevice.Click += OnCreateDeviceInfo;

      // ContextMenu
      cmd_RefreshSystemOnlineTree.Click += OnRefreshSystemOnlineTreeClick;
      cmd_ShowDeviceProperties.Click += OnShowDevicePropertiesClick;

      mManager.RequestDevicesOnlineEOT += OnManagerRequestDevicesOnlineEOT;

      mManager.DeviceInfo += OnDeviceInfo;
      mManager.PortCount += OnPortCount;

      OnlineTree.MouseUp += OnlineTreeOnMouseUp;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Settings.Default.LastSendText = txt_Text.Text;
      Settings.Default.Save();
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

      mManager?.RequestDevicesOnlineAsync(DlgMain.DynamicDevice);
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

      if(lNode.Tag is DeviceInfoEventArgs lInfo)
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

    private void OnManagerRequestDevicesOnlineEOT(object sender, EventArgs e)
    {
      this.InvokeIfRequired(a =>
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
      });
    }

    private void OnDeviceInfo(object sender, DeviceInfoEventArgs e)
    {
      var lImageKey = "AMXDeviceDefault";
      var lSelectedImageKey = "AMXDeviceSelected";

      TreeNode lNode;

      this.InvokeIfRequired(a =>
      {
        // System-Device
        if(e.Device == 0 && e.ObjectId == 0 && e.System == mManager.CurrentSystem)
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
      });
    }

    private void OnPortCount(object sender, PortCountEventArgs e)
    {
      var lImageKey = "IODeviceDefault";
      var lSelectedImageKey = "IODeviceSelected";

      this.InvokeIfRequired(a =>
      {
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
      });
    }

    private void OnCmdChannelOn(object sender, EventArgs e)
    {
      mManager?.SetOutputChannelAsync(GetDevice(), DlgMain.DynamicDevice,(ushort)num_Channel.Value, true);
    }

    private void OnCmdChannelOff(object sender, EventArgs e)
    {
      mManager?.SetOutputChannelAsync(GetDevice(), DlgMain.DynamicDevice, (ushort)num_Channel.Value, false);
    }

    private void OnCmdSendLevel(object sender, EventArgs e)
    {
      mManager?.SendLevelAsync(GetDevice(), DlgMain.DynamicDevice, (ushort)num_LevelInput.Value, (ushort)num_LevelValue.Value);
    }

    private void OnCmdSendString(object sender, EventArgs e)
    {
      if(txt_Text.SelectedText.Length > 0)
        mManager?.SendStringAsync(GetDevice(), DlgMain.DynamicDevice, txt_Text.SelectedText);
      else
        mManager?.SendStringAsync(GetDevice(), DlgMain.DynamicDevice, txt_Text.Text);
    }

    private void OnCmdSendCmd(object sender, EventArgs e)
    {
      if(txt_Text.SelectedText.Length > 0)
        mManager?.SendCommandAsync(GetDevice(), DlgMain.DynamicDevice, txt_Text.SelectedText);
      else
        mManager?.SendCommandAsync(GetDevice(), DlgMain.DynamicDevice, txt_Text.Text);
    }

    private void OnCreateDeviceInfo(object sender, EventArgs e)
    {
      if(!mManager.IsConnected)
      {
        InfoMessageBox.Show(this, "Not connected");
        return;
      }

      DlgMain.CreatePhysicalDevice();
    }

    private void OnCmdRequestDeviceStatus(object sender, EventArgs e)
    {
      mManager?.RequestDeviceStatusAsync(GetDevice(), DlgMain.DynamicDevice);
    }

    private AmxDevice GetDevice()
    {
      return new AmxDevice((ushort)num_Device.Value, (ushort)num_DevPort.Value, (ushort)num_System.Value);
    }
  }
}