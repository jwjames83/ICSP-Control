using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using ICSP.Control.Properties;
using ICSP.Core;
using ICSP.Core.Client;
using ICSP.Core.Manager.DeviceManager;
using ICSP.Core.Manager.DiagnosticManager;

using ICSPControl.Extensions;

using Microsoft.Win32;

using WeifenLuo.WinFormsUI.Docking;

namespace ICSPControl.Dialogs
{
  public partial class DlgMain : Form
  {
    private readonly System.Windows.Forms.Timer mBlinkTimer;

    private readonly ICSPManager mICSPManager;

    private DlgFileTransfer mDlgFileTransfer;
    private DlgDiagnostic mDlgDiagnostic;
    private DlgTrace mDlgTrace;

    private bool mDynamicDeviceOnline;
    private bool mPhysicalDeviceOnline;

    public DlgMain()
    {
      InitializeComponent();

      mBlinkTimer = new System.Windows.Forms.Timer { Interval = 500 };
      mBlinkTimer.Tick += (s, e) => { tssl_Blink.BackColor = SystemColors.Control; };

      // Fix
      // The StatusStrip.Padding property is borked, it returns the wrong value for 
      // Padding Right if the sizing grip is disabled.
      MainStatusStrip.Padding =
        new Padding(MainStatusStrip.Padding.Left, MainStatusStrip.Padding.Top, MainStatusStrip.Padding.Left, MainStatusStrip.Padding.Bottom);

      tsmi_File_Connect.Click += OnConnectClick;
      tsmi_File_Disconnect.Click += OnDisconnectClick;

      tsb_Connect.Click += OnConnectClick;
      tsb_Disconnect.Click += OnDisconnectClick;

      tsmi_File_CommunicationSetttings.Click += OnCommunicationSetttingsClick;
      tsmi_File_Exit.Click += OnExitClick;

      tsmi_Tools_InfoFileTransfer.Click += (s, e) => { new DlgInfoFileTransfer().ShowDialog(); };
      tsmi_Tools_OpenTmpFolder.Click += (s, e) => { OpenTmpFolder(); };

      tsmi_Tools_FileTransfer.Click += OnFileTransferClick;
      tsmi_Tools_ControlDevice.Click += OnControlDeviceClick;
      tsmi_Tools_DeviceNotifications.Click += OnDeviceNotificationsClick;

      tsb_FileTransfer.Click += OnFileTransferClick;
      tsb_ControlDevice.Click += OnControlDeviceClick;
      tsb_DeviceNotifications.Click += OnDeviceNotificationsClick;

      tssl_Host.Text = string.Format("Host: {0}", Settings.Default.AmxHost);
      tssl_Port.Text = string.Format("Port: {0}", Settings.Default.AmxPort);

      // Physical Device
      tssl_Device.Text = string.Format("Device: {0}", Settings.Default.PhysicalDeviceNumber);

      // Add the event handler for handling UI thread exceptions to the event.
      Application.ThreadException += OnThreadException;

      mICSPManager = new ICSPManager();

      mICSPManager.FileManager.SetBaseDirectory(Settings.Default.FileTransferPanelDirectory);

      mICSPManager.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
      mICSPManager.DynamicDeviceCreated += OnDynamicDeviceCreated;

      mICSPManager.DeviceOnline += OnDeviceOnline;
      mICSPManager.DeviceOffline += OnDeviceOffline;

      mICSPManager.DiscoveryInfo += OnDiscoveryInfo;
      mICSPManager.BlinkMessage += OnBlinkMessage;

      DockPanel.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();

      if(Settings.Default.AutoConnect)
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

    private void OnCommunicationSetttingsClick(object sender, EventArgs e)
    {
      new DlgSettings().ShowDialog(this);

      tssl_Host.Text = string.Format("Host: {0}", Settings.Default.AmxHost);
      tssl_Port.Text = string.Format("Port: {0}", Settings.Default.AmxPort);
      tssl_Device.Text = string.Format("Device: {0}", Settings.Default.PhysicalDeviceNumber);
    }

    private void OnExitClick(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void OnDeviceNotificationsClick(object sender, EventArgs e)
    {
      if(mDlgTrace == null || mDlgTrace.Disposing || mDlgTrace.IsDisposed)
      {
        mDlgTrace = new DlgTrace(mICSPManager);
      }

      mDlgTrace.Show(DockPanel, DockState.Document);
      mDlgTrace.BringToFront();
    }

    private void OnFileTransferClick(object sender, EventArgs e)
    {
      if(mDlgFileTransfer == null || mDlgFileTransfer.Disposing || mDlgFileTransfer.IsDisposed)
      {
        mDlgFileTransfer = new DlgFileTransfer(mICSPManager);
      }

      mDlgFileTransfer.Show(DockPanel, DockState.Document);
      mDlgFileTransfer.BringToFront();
    }

    private void OnControlDeviceClick(object sender, EventArgs e)
    {
      if(mDlgDiagnostic == null || mDlgDiagnostic.Disposing || mDlgDiagnostic.IsDisposed)
      {
        mDlgDiagnostic = new DlgDiagnostic(mICSPManager);
      }

      mDlgDiagnostic.Show(DockPanel, DockState.Document);
      mDlgDiagnostic.BringToFront();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Settings.Default.Save();
    }

    private void OnClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      tsmi_File_Connect.Enabled = !e.ClientOnline;
      tsmi_File_Disconnect.Enabled = e.ClientOnline;

      tsb_Connect.Enabled = !e.ClientOnline;
      tsb_Disconnect.Enabled = e.ClientOnline;

      if(e.ClientOnline)
      {
        tssl_ClientState.Text = "Connected";
        tssl_ClientState.BackColor = Color.Green;
      }
      else
      {
        tssl_ClientState.Text = "Not Connected";
        tssl_ClientState.BackColor = Color.Red;

        tssl_CurrentSystem.Text = "Current System: 0";
        tssl_DynamicDevice.Text = "Dynamic Device: 00000";

        tssl_ProgramName.Text = "Program Name: ";
        tssl_MainFile.Text = "Main File: ";

        mDynamicDeviceOnline = false;
        mPhysicalDeviceOnline = false;

        tssl_DynamicDevice.BackColor = SystemColors.Control;
        tssl_Device.BackColor = SystemColors.Control;
      }
    }

    private void OnDynamicDeviceCreated(object sender, DynamicDeviceCreatedEventArgs e)
    {
      this.InvokeIfRequired(a =>
      {
        tssl_CurrentSystem.Text = string.Format("Current System: {0}", e.System);
        tssl_DynamicDevice.Text = string.Format("Dynamic Device: {0:00000}", e.Device);
      });

      if(Settings.Default.PhysicalDeviceAutoCreate)
        CreatePhysicalDevice();

      // Request ProgramInfo ...
      mICSPManager.Send(MsgCmdRequestDiscoveryInfo.CreateRequest(mICSPManager.DynamicDevice, 0x1F));
    }

    private void OnDeviceOnline(object sender, DeviceInfoData e)
    {
      this.InvokeIfRequired(a =>
      {
        if(e.Device == mICSPManager.DynamicDevice.Device)
      {
        mDynamicDeviceOnline = true;
        tssl_DynamicDevice.BackColor = Color.Green;
      }

      if(e.Device == Settings.Default.PhysicalDeviceNumber)
      {
        mPhysicalDeviceOnline = true;
        tssl_Device.BackColor = Color.Green;
        }
      });
    }

    private void OnDeviceOffline(object sender, DeviceInfoData e)
    {
      this.InvokeIfRequired(a =>
      {
        if(e.Device == mICSPManager.DynamicDevice.Device)
        {
          mDynamicDeviceOnline = false;
          tssl_DynamicDevice.BackColor = SystemColors.Control;
        }

        if(e.Device == Settings.Default.PhysicalDeviceNumber)
        {
          mPhysicalDeviceOnline = false;
          tssl_Device.BackColor = SystemColors.Control;
        }
      });
    }

    private void OnDiscoveryInfo(object sender, DiscoveryInfoEventArgs e)
    {
      if(e.IPv4Address.Equals(mICSPManager.CurrentRemoteIpAddress))
      {
        this.InvokeIfRequired(a =>
        {
          tssl_ProgramName.Text = string.Format("Program Name: {0}", e.ProgramName);
          tssl_MainFile.Text = string.Format("Main File: {0}", e.MainFile);
        });
      }
    }

    private void OnBlinkMessage(object sender, BlinkEventArgs e)
    {
      mBlinkTimer.Stop();

      this.InvokeIfRequired(a =>
      {
        tssl_Blink.BackColor = Color.Green;
      });

      mBlinkTimer.Start();
    }

    private void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
      ErrorMessageBox.Show(this, e.Exception.Message);
    }

    private void OnCreateDeviceInfo(object sender, EventArgs e)
    {
      if(!mICSPManager.IsConnected)
      {
        InfoMessageBox.Show(this, "Not connected");
        return;
      }

      CreatePhysicalDevice();
    }

    private void CreatePhysicalDevice()
    {
      if(!mICSPManager.IsConnected)
        return;

      var lDeviceId = Settings.Default.PhysicalDeviceDeviceId;

      if(Settings.Default.PhysicalDeviceUseCustomDeviceId)
        lDeviceId = Settings.Default.PhysicalDeviceCustomDeviceId;

      var lDeviceInfo = new DeviceInfoData(
        Settings.Default.PhysicalDeviceNumber,
        mICSPManager.CurrentSystem,
        mICSPManager.CurrentLocalIpAddress)
      {
        Version = Settings.Default.PhysicalDeviceVersion,
        Name = Settings.Default.PhysicalDeviceName,
        Manufacture = Settings.Default.PhysicalDeviceManufacturer,
        SerialNumber = Settings.Default.PhysicalDeviceSerialNumber,
        ManufactureId = Settings.Default.PhysicalDeviceManufactureId,
        DeviceId = lDeviceId,
        FirmwareId = Settings.Default.PhysicalDeviceFirmwareId
      };

      mICSPManager?.CreateDeviceInfo(lDeviceInfo, Settings.Default.PhysicalDevicePortCount);
    }

    private void OpenTmpFolder()
    {
      try
      {
        using var lKey = Registry.CurrentUser.OpenSubKey(@"Software\AMX Corp.\TPDesign5\Settings\User Preferences");

        if(lKey != null)
        {
          if(lKey.GetValue("User selected temp directory") is string lPath)
          {
            Process.Start(lPath);
            return;
          }
        }
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex);

        return;
      }

      InfoMessageBox.Show("G5 Designer not installed.");
    }
  }
}