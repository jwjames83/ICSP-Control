using System.ComponentModel;
using System.Windows.Forms;

using ICSPControl.Properties;

namespace ICSPControl.Dialogs
{
  public partial class DlgSettings : Form
  {
    public DlgSettings()
    {
      InitializeComponent();

      txt_Host.Text = Settings.Default.AmxHost;
      num_Port.Value = Settings.Default.AmxPort;
      ckb_AutoConnect.Checked = Settings.Default.AutoConnect;

      num_PhysicalDeviceNumber.Value = Settings.Default.PhysicalDeviceNumber;
      num_PhysicalDevicePortCount.Value = Settings.Default.PhysicalDevicePortCount;

      ckb_PhysicalDeviceAutoCreate.Checked = Settings.Default.PhysicalDeviceAutoCreate;
      txt_PhysicalDeviceVersion.Text = Settings.Default.PhysicalDeviceVersion;
      txt_PhysicalDeviceName.Text = Settings.Default.PhysicalDeviceName;
      txt_PhysicalDeviceManufacturer.Text = Settings.Default.PhysicalDeviceManufacturer;
      txt_PhysicalDeviceSerialNumber.Text = Settings.Default.PhysicalDeviceSerialNumber;

      num_PhysicalDeviceManufactureId.Value = Settings.Default.PhysicalDeviceManufactureId;
      num_PhysicalDeviceDeviceId.Value = Settings.Default.PhysicalDeviceDeviceId;
      num_PhysicalDeviceFirmwareId.Value = Settings.Default.PhysicalDeviceFirmwareId;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if(DialogResult == DialogResult.OK)
      {
        Settings.Default.AmxHost = txt_Host.Text;
        Settings.Default.AmxPort = (int)num_Port.Value;
        Settings.Default.AutoConnect = ckb_AutoConnect.Checked;

        Settings.Default.PhysicalDeviceNumber = (ushort)num_PhysicalDeviceNumber.Value;
        Settings.Default.PhysicalDevicePortCount = (ushort)num_PhysicalDevicePortCount.Value;

        Settings.Default.PhysicalDeviceAutoCreate = ckb_PhysicalDeviceAutoCreate.Checked;
        Settings.Default.PhysicalDeviceVersion = txt_PhysicalDeviceVersion.Text;
        Settings.Default.PhysicalDeviceName = txt_PhysicalDeviceName.Text;
        Settings.Default.PhysicalDeviceManufacturer = txt_PhysicalDeviceManufacturer.Text;
        Settings.Default.PhysicalDeviceSerialNumber = txt_PhysicalDeviceSerialNumber.Text;

        Settings.Default.PhysicalDeviceManufactureId = (ushort)num_PhysicalDeviceManufactureId.Value;
        Settings.Default.PhysicalDeviceDeviceId = (ushort)num_PhysicalDeviceDeviceId.Value;
        Settings.Default.PhysicalDeviceFirmwareId = (ushort)num_PhysicalDeviceFirmwareId.Value;

        Settings.Default.Save();
      }

      base.OnClosing(e);
    }
  }
}
