using System;
using System.ComponentModel;

using ICSP;
using ICSP.Client;
using ICSP.Manager.DeviceManager;
using System.Windows.Forms;

using ICSPControl.Properties;
using System.Linq;
using ICSP.IO;
using System.Diagnostics;

namespace ICSPControl.Dialogs
{
  public partial class DlgFileTransfer : WeifenLuo.WinFormsUI.Docking.DockContent
  {
    private readonly ICSPManager mICSPManager;

    private const int MaxLogEntries = 1001;

    private int mCurrentFile;
    private int mCurrentSize;

    public DlgFileTransfer(ICSPManager manager)
    {
      InitializeComponent();

      mICSPManager = manager ?? throw new ArgumentNullException(nameof(manager));

      cmd_CreatePhysicalDevice.Click += OnCreateDeviceInfo;

      cmd_OutputDirectoryBrowse.Click += OnOutputFolderBrowseClick;
      cmd_OpenFolder.Click += OnOpenFolderClick;

      mICSPManager.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
      mICSPManager.DynamicDeviceCreated += OnDynamicDeviceCreated;

      // =====================================================================
      // FileManager-Events
      // =====================================================================

      mICSPManager.OnTransferFileData += OnTransferFileData;

      //mICSPManager.OnTransferFileDataComplete += OnTransferFileDataComplete;
      //mICSPManager.OnTransferFileDataCompleteAck += OnTransferFileDataCompleteAck;
      mICSPManager.OnTransferFilesInitialize += OnTransferFilesInitialize;
      mICSPManager.OnTransferFilesComplete += OnTransferFilesComplete;

      mICSPManager.OnGetDirectoryInfo += OnGetDirectoryInfo;
      mICSPManager.OnDirectoryInfo += OnDirectoryInfo;
      mICSPManager.OnDirectoryItem += OnDirectoryItem;
      mICSPManager.OnDeleteFile += OnDeleteFile;
      mICSPManager.OnCreateDirectory += OnCreateDirectory;

      // mICSPManager.OnTransferSingleFile += OnTransferSingleFile;
      // mICSPManager.OnTransferSingleFileAck += OnTransferSingleFileAck;
      mICSPManager.OnTransferSingleFileInfo += OnTransferSingleFileInfo;
      // mICSPManager.OnTransferSingleFileInfoAck += OnTransferSingleFileInfoAck;
      mICSPManager.OnTransferGetFileAccessToken += OnTransferGetFileAccessToken;
      mICSPManager.OnTransferGetFileAccessTokenAck += OnTransferGetFileAccessTokenAck;
      mICSPManager.OnTransferGetFile += OnTransferGetFile;

      txt_PanelDirectory.Text = AppDomain.CurrentDomain.BaseDirectory;

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

    // =====================================================================
    // FileManager-Events
    // =====================================================================

    #region FileManager-Events

    private void OnTransferFileData(object sender, TransferFileDataEventArgs e)
    {
      mCurrentSize += e.JunkSize;

      if(mCurrentSize <= prb_2.Maximum)
        prb_2.Value = mCurrentSize;
      else
        prb_2.Value = prb_2.Maximum;
    }

    private void OnTransferFileDataComplete(object sender, EventArgs e) { AppendLog("OnTransferFileDataComplete", null); }
    private void OnTransferFileDataCompleteAck(object sender, EventArgs e) { AppendLog("OnTransferFileDataCompleteAck", null); }

    private void OnTransferFilesInitialize(object sender, TransferFilesInitializeEventArgs e)
    {
      AppendLog("OnTransferFilesInitialize: FileCount={0}", e.FileCount);

      mCurrentFile = 0;
      mCurrentSize = 0;

      prb_1.Value = 0;
      prb_1.Maximum = e.FileCount;

      prb_2.Value = 0;
    }

    private void OnTransferFilesComplete(object sender, EventArgs e)
    {
      mCurrentFile = 0;

      AppendLog("OnTransferFilesComplete", null);
    }

    private void OnGetDirectoryInfo(object sender, GetDirectoryInfoEventArgs e)
    {
      AppendLog("OnGetDirectoryInfo: Path={0}", e.Path);
    }

    private void OnDirectoryInfo(object sender, DirectoryInfoEventArgs e)
    {
      AppendLog("OnDirectoryInfo: FullPath={0}", e.FullPath);
    }

    private void OnDirectoryItem(object sender, DirectoryItemEventArgs e)
    {
      AppendLog("OnDirectoryItem: FileName={0}", e.FileName);
    }

    private void OnDeleteFile(object sender, DeleteFileEventArgs e)
    {
      AppendLog("OnDeleteFile: FileName={0}", e.FileName);
    }

    private void OnCreateDirectory(object sender, CreatDirectoryEventArgs e)
    {
      if(mCurrentFile == 0)
        txt_Info.Clear();

      AppendLog("OnCreateDirectory: Directory={0}", e.Directory);
    }

    private void OnTransferSingleFile(object sender, EventArgs e) { AppendLog("OnTransferSingleFile", null); }
    private void OnTransferSingleFileAck(object sender, EventArgs e) { AppendLog("OnTransferSingleFileAck", null); }

    private void OnTransferSingleFileInfo(object sender, TransferSingleFileInfoEventArgs e)
    {
      if(++mCurrentFile <= prb_1.Maximum)
        prb_1.Value = mCurrentFile;
      else
        prb_1.Value = prb_1.Maximum;

      prb_2.Maximum = e.FileSize;

      mCurrentSize = 0;

      AppendLog("OnTransferFilesInitialize: FileSize={0} bytes, FileName={1}", e.FileSize, e.FileName);
    }

    private void OnTransferSingleFileInfoAck(object sender, EventArgs e) { AppendLog("OnTransferSingleFileInfoAck", null); }
    
    private void OnTransferGetFileAccessToken(object sender, TransferGetFileAccessTokenEventArgs e)
    {
      AppendLog("OnTransferGetFileAccessToken: Size={0} bytes, FileName={1}", e.Size, e.FileName);
    }

    private void OnTransferGetFileAccessTokenAck(object sender, EventArgs e) { AppendLog("OnTransferGetFileAccessTokenAck", null); }
    private void OnTransferGetFile(object sender, EventArgs e) { AppendLog("OnTransferGetFile", null); }

    #endregion

    private void OnDynamicDeviceCreated(object sender, DynamicDeviceCreatedEventArgs e)
    {
      if(Settings.Default.PhysicalDeviceAutoCreate)
        CreatePhysicalDevice();
    }

    private void OnOutputFolderBrowseClick(object sender, EventArgs e)
    {
      FolderDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

      if(FolderDialog.ShowDialog() == DialogResult.OK)
      {
        txt_PanelDirectory.Text = FolderDialog.SelectedPath;

        // Settings.Default.OutputFolder = FolderDialog.SelectedPath;

        Settings.Default.Save();
      }
    }

    private void OnOpenFolderClick(object sender, EventArgs e)
    {
      try
      {
        var lBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        Process.Start(lBaseDirectory);
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex);

        return;
      }
    }

    private void SaveSettings()
    {
      //Settings.Default.FileNameTp4 = txt_FileNameTp4.Text.Trim();

      //Settings.Default.OutputFolder = txt_OutputDirectory.Text.Trim();

      //Settings.Default.ConnHost = txt_ConnHost.Text.Trim();
      //Settings.Default.ConnPort = (int)num_ConnPort.Value;

      //Settings.Default.JsFileName = txt_JsFileName.Text.Trim();
      //Settings.Default.JsVariableName = txt_JsVariableName.Text.Trim();

      //Settings.Default.CreateJson = ckb_CreateJson.Checked;
      //Settings.Default.CreateXml = ckb_CreateXml.Checked;
      //Settings.Default.CreateSegmentInfo = ckb_CreateSegmentInfo.Checked;

      Settings.Default.Save();
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

    public void AppendLog(string format, params object[] args)
    {
      var lMessage = format;

      if(args != null && args.Length > 0)
        lMessage = string.Format(format, args);

      txt_Info.AppendText(string.Format("{0:yyy-MM-dd (HH:mm.ss)}: {1}\r\n", DateTime.Now, lMessage));

      if(txt_Info.Lines.Length > MaxLogEntries)
      {
        var newLines = new string[MaxLogEntries];

        Array.Copy(txt_Info.Lines, 1, newLines, 0, MaxLogEntries);

        txt_Info.Lines = newLines;
      }

      txt_Info.SelectionStart = txt_Info.Text.Length;

      txt_Info.ScrollToCaret();

      Application.DoEvents();
    }
  }
}