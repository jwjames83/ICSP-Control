using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using ICSPControl.Properties;

namespace ICSPControl.Dialogs
{
  public struct PanelType
  {
    public ushort DeviceId { get; set; }

    public string DeviceType { get; set; }
  }

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

      // Fill ComboBox: PanelType

      // =================================================================================
      // Add G4 Panels (From PPF.xml)
      // =================================================================================

      var lPanelTypes = new List<PanelType>();

      lPanelTypes.Add(new PanelType() { DeviceId = 333, DeviceType = "G4 MVP-5150" });
      lPanelTypes.Add(new PanelType() { DeviceId = 329, DeviceType = "G4 MVP-5200i" });
      lPanelTypes.Add(new PanelType() { DeviceId = 288, DeviceType = "G4 MVP-750" });
      lPanelTypes.Add(new PanelType() { DeviceId = 323, DeviceType = "G4 MVP-8400i" });
      lPanelTypes.Add(new PanelType() { DeviceId = 289, DeviceType = "G4 MVP-8400" });
      lPanelTypes.Add(new PanelType() { DeviceId = 343, DeviceType = "G4 MVP-9000i" });
      lPanelTypes.Add(new PanelType() { DeviceId = 342, DeviceType = "G4 NXD-430" });
      lPanelTypes.Add(new PanelType() { DeviceId = 331, DeviceType = "G4 NXD-500i" });
      lPanelTypes.Add(new PanelType() { DeviceId = 313, DeviceType = "G4 NXD-CV5" });
      lPanelTypes.Add(new PanelType() { DeviceId = 377, DeviceType = "G4 NXD-700i" });
      lPanelTypes.Add(new PanelType() { DeviceId = 324, DeviceType = "G4 NXD-700Vi" });
      lPanelTypes.Add(new PanelType() { DeviceId = 291, DeviceType = "G4 NXD-CV7" });
      lPanelTypes.Add(new PanelType() { DeviceId = 376, DeviceType = "G4 NXT-CA7" });
      lPanelTypes.Add(new PanelType() { DeviceId = 291, DeviceType = "G4 NXT-CV7" });
      lPanelTypes.Add(new PanelType() { DeviceId = 325, DeviceType = "G4 NXD-1000Vi" });
      lPanelTypes.Add(new PanelType() { DeviceId = 292, DeviceType = "G4 NXD-CV10" });
      lPanelTypes.Add(new PanelType() { DeviceId = 292, DeviceType = "G4 NXT-CV10" });
      lPanelTypes.Add(new PanelType() { DeviceId = 292, DeviceType = "G4 NXT-CV10" });
      lPanelTypes.Add(new PanelType() { DeviceId = 294, DeviceType = "G4 NXD-1200V" });
      lPanelTypes.Add(new PanelType() { DeviceId = 294, DeviceType = "G4 NXT-1200V" });
      lPanelTypes.Add(new PanelType() { DeviceId = 295, DeviceType = "G4 NXD-1200VG" });
      lPanelTypes.Add(new PanelType() { DeviceId = 295, DeviceType = "G4 NXT-1200VG" });
      lPanelTypes.Add(new PanelType() { DeviceId = 277, DeviceType = "G4 NXD-CA12" });
      lPanelTypes.Add(new PanelType() { DeviceId = 277, DeviceType = "G4 NXT-CA12" });
      lPanelTypes.Add(new PanelType() { DeviceId = 278, DeviceType = "G4 NXD-CV12" });
      lPanelTypes.Add(new PanelType() { DeviceId = 278, DeviceType = "G4 NXT-CV12" });
      lPanelTypes.Add(new PanelType() { DeviceId = 296, DeviceType = "G4 NXD-1500VG" });
      lPanelTypes.Add(new PanelType() { DeviceId = 296, DeviceType = "G4 NXT-1500VG" });
      lPanelTypes.Add(new PanelType() { DeviceId = 281, DeviceType = "G4 NXD-CA15" });
      lPanelTypes.Add(new PanelType() { DeviceId = 281, DeviceType = "G4 NXT-CA15" });
      lPanelTypes.Add(new PanelType() { DeviceId = 282, DeviceType = "G4 NXD-CV15" });
      lPanelTypes.Add(new PanelType() { DeviceId = 282, DeviceType = "G4 NXT-CV15" });
      lPanelTypes.Add(new PanelType() { DeviceId = 297, DeviceType = "G4 NXD-1700VG" });
      lPanelTypes.Add(new PanelType() { DeviceId = 297, DeviceType = "G4 NXT-1700VG" });
      lPanelTypes.Add(new PanelType() { DeviceId = 284, DeviceType = "G4 NXD-CV17" });
      lPanelTypes.Add(new PanelType() { DeviceId = 284, DeviceType = "G4 NXT-CV17" });
      lPanelTypes.Add(new PanelType() { DeviceId = 398, DeviceType = "G4 MST-431" });
      lPanelTypes.Add(new PanelType() { DeviceId = 399, DeviceType = "G4 MSD-431" });
      lPanelTypes.Add(new PanelType() { DeviceId = 400, DeviceType = "G4 MST-701" });
      lPanelTypes.Add(new PanelType() { DeviceId = 401, DeviceType = "G4 MSD-701" });
      lPanelTypes.Add(new PanelType() { DeviceId = 402, DeviceType = "G4 MST-1001" });
      lPanelTypes.Add(new PanelType() { DeviceId = 403, DeviceType = "G4 MSD-1001" });
      lPanelTypes.Add(new PanelType() { DeviceId = 468, DeviceType = "G4 HPX-MSP-7" });
      lPanelTypes.Add(new PanelType() { DeviceId = 469, DeviceType = "G4 HPX-MSP-10" });
      lPanelTypes.Add(new PanelType() { DeviceId = 375, DeviceType = "G4 MXD-430" });
      lPanelTypes.Add(new PanelType() { DeviceId = 375, DeviceType = "G4 MXD-430-L" });
      lPanelTypes.Add(new PanelType() { DeviceId = 373, DeviceType = "G4 MXT-700" });
      lPanelTypes.Add(new PanelType() { DeviceId = 374, DeviceType = "G4 MXD-700" });
      lPanelTypes.Add(new PanelType() { DeviceId = 371, DeviceType = "G4 MXT-1000" });
      lPanelTypes.Add(new PanelType() { DeviceId = 372, DeviceType = "G4 MXD-1000" });
      lPanelTypes.Add(new PanelType() { DeviceId = 369, DeviceType = "G4 MXT-1900L-PAN" });
      lPanelTypes.Add(new PanelType() { DeviceId = 370, DeviceType = "G4 MXD-1900L-PAN" });
      lPanelTypes.Add(new PanelType() { DeviceId = 361, DeviceType = "G4 MXT-2000XL-PAN" });
      lPanelTypes.Add(new PanelType() { DeviceId = 368, DeviceType = "G4 MXD-2000XL-PAN" });
      lPanelTypes.Add(new PanelType() { DeviceId = 326, DeviceType = "G4 NXP-PLV" });
      lPanelTypes.Add(new PanelType() { DeviceId = 274, DeviceType = "G4 NXP-TPI4" });
      lPanelTypes.Add(new PanelType() { DeviceId = 327, DeviceType = "G4 TPI-PRO" });
      lPanelTypes.Add(new PanelType() { DeviceId = 334, DeviceType = "G4 TPI-PRO-DVI" });
      lPanelTypes.Add(new PanelType() { DeviceId = 341, DeviceType = "G4 NXV-300" });
      lPanelTypes.Add(new PanelType() { DeviceId = 322, DeviceType = "G4 R-4" });
      lPanelTypes.Add(new PanelType() { DeviceId = 304, DeviceType = "G4 Mio Modero DMS" });
      lPanelTypes.Add(new PanelType() { DeviceId = 305, DeviceType = "G4 Mio Modero DMS Pinnacle" });
      lPanelTypes.Add(new PanelType() { DeviceId = 349, DeviceType = "G4 iPod touch (TPC)" });

      // =================================================================================
      // Add G5 Panels
      // =================================================================================

      lPanelTypes.Add(new PanelType() { DeviceId = 0x0205, DeviceType = "G5 MT-1002" });

      /*
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 333, DeviceType = "G4 MVP-5150" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 329, DeviceType = "G4 MVP-5200i" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 288, DeviceType = "G4 MVP-750" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 323, DeviceType = "G4 MVP-8400i" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 289, DeviceType = "G4 MVP-8400" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 343, DeviceType = "G4 MVP-9000i" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 342, DeviceType = "G4 NXD-430" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 331, DeviceType = "G4 NXD-500i" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 313, DeviceType = "G4 NXD-CV5" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 377, DeviceType = "G4 NXD-700i" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 324, DeviceType = "G4 NXD-700Vi" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 291, DeviceType = "G4 NXD-CV7" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 376, DeviceType = "G4 NXT-CA7" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 291, DeviceType = "G4 NXT-CV7" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 325, DeviceType = "G4 NXD-1000Vi" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 292, DeviceType = "G4 NXD-CV10" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 292, DeviceType = "G4 NXT-CV10" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 292, DeviceType = "G4 NXT-CV10" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 294, DeviceType = "G4 NXD-1200V" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 294, DeviceType = "G4 NXT-1200V" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 295, DeviceType = "G4 NXD-1200VG" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 295, DeviceType = "G4 NXT-1200VG" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 277, DeviceType = "G4 NXD-CA12" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 277, DeviceType = "G4 NXT-CA12" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 278, DeviceType = "G4 NXD-CV12" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 278, DeviceType = "G4 NXT-CV12" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 296, DeviceType = "G4 NXD-1500VG" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 296, DeviceType = "G4 NXT-1500VG" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 281, DeviceType = "G4 NXD-CA15" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 281, DeviceType = "G4 NXT-CA15" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 282, DeviceType = "G4 NXD-CV15" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 282, DeviceType = "G4 NXT-CV15" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 297, DeviceType = "G4 NXD-1700VG" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 297, DeviceType = "G4 NXT-1700VG" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 284, DeviceType = "G4 NXD-CV17" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 284, DeviceType = "G4 NXT-CV17" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 398, DeviceType = "G4 MST-431" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 399, DeviceType = "G4 MSD-431" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 400, DeviceType = "G4 MST-701" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 401, DeviceType = "G4 MSD-701" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 402, DeviceType = "G4 MST-1001" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 403, DeviceType = "G4 MSD-1001" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 468, DeviceType = "G4 HPX-MSP-7" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 469, DeviceType = "G4 HPX-MSP-10" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 375, DeviceType = "G4 MXD-430" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 375, DeviceType = "G4 MXD-430-L" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 373, DeviceType = "G4 MXT-700" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 374, DeviceType = "G4 MXD-700" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 371, DeviceType = "G4 MXT-1000" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 372, DeviceType = "G4 MXD-1000" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 369, DeviceType = "G4 MXT-1900L-PAN" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 370, DeviceType = "G4 MXD-1900L-PAN" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 361, DeviceType = "G4 MXT-2000XL-PAN" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 368, DeviceType = "G4 MXD-2000XL-PAN" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 326, DeviceType = "G4 NXP-PLV" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 274, DeviceType = "G4 NXP-TPI4" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 327, DeviceType = "G4 TPI-PRO" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 334, DeviceType = "G4 TPI-PRO-DVI" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 341, DeviceType = "G4 NXV-300" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 322, DeviceType = "G4 R-4" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 304, DeviceType = "G4 Mio Modero DMS" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 305, DeviceType = "G4 Mio Modero DMS Pinnacle" });
      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 349, DeviceType = "G4 iPod touch (TPC)" });

      // =================================================================================
      // Add G5 Panels
      // =================================================================================

      cbo_PanelType.Items.Add(new PanelType() { DeviceId = 0x0205, DeviceType = "G5 MT-1002" });
      */

      cbo_PanelType.DataSource = lPanelTypes;
      cbo_PanelType.ValueMember = nameof(PanelType.DeviceId);
      cbo_PanelType.DisplayMember = nameof(PanelType.DeviceType);

      var lPanelType = lPanelTypes.FirstOrDefault(p => p.DeviceType == Settings.Default.PhysicalDeviceDeviceType);

      if(lPanelType.DeviceId == 0)
        lPanelType = lPanelTypes.FirstOrDefault(p => p.DeviceId == Settings.Default.PhysicalDeviceDeviceId);

      cbo_PanelType.SelectedItem = lPanelType;

      if(cbo_PanelType.SelectedIndex < 0)
        cbo_PanelType.SelectedIndex = 0;

      ckb_PhysicalDeviceUseCustomDeviceId.Checked = Settings.Default.PhysicalDeviceUseCustomDeviceId;
      num_PhysicalDeviceDeviceId.Value = Settings.Default.PhysicalDeviceCustomDeviceId;

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

        Settings.Default.PhysicalDeviceDeviceId = ((ushort?)cbo_PanelType.SelectedValue) ?? 0;
        Settings.Default.PhysicalDeviceDeviceType = cbo_PanelType.Text;
        Settings.Default.PhysicalDeviceUseCustomDeviceId = ckb_PhysicalDeviceUseCustomDeviceId.Checked;
        Settings.Default.PhysicalDeviceCustomDeviceId = (ushort)num_PhysicalDeviceDeviceId.Value;

        Settings.Default.PhysicalDeviceFirmwareId = (ushort)num_PhysicalDeviceFirmwareId.Value;

        Settings.Default.Save();
      }

      base.OnClosing(e);
    }
  }
}
