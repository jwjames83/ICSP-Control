using System;
using System.ComponentModel;

using ICSP;
using ICSP.Client;
using ICSP.Manager.DeviceManager;

using ICSPControl.Properties;

namespace ICSPControl.Dialogs
{
  public partial class DlgFileTransfer : WeifenLuo.WinFormsUI.Docking.DockContent
  {
    private readonly ICSPManager mICSPManager;

    public DlgFileTransfer(ICSPManager manager)
    {
      InitializeComponent();

      mICSPManager = manager ?? throw new ArgumentNullException(nameof(manager));

      cmd_CreatePhysicalDevice.Click += OnCreateDeviceInfo;

      mICSPManager.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
      mICSPManager.DynamicDeviceCreated += OnDynamicDeviceCreated;

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

    private void OnDynamicDeviceCreated(object sender, DynamicDeviceCreatedEventArgs e)
    {
      if(Settings.Default.PhysicalDeviceAutoCreate)
        CreatePhysicalDevice();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      Settings.Default.Save();
    }
    
    private void OnClientOnlineStatusChanged(object sender, ClientOnlineOfflineEventArgs e)
    {
      if(e.ClientOnline)
      {
      }
      else
      {
      }
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
  }
}