using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using ICSP;
using ICSP.Client;
using ICSP.IO;
using ICSP.Logging;
using ICSP.Manager.DeviceManager;

using ICSPControl.Extensions;
using ICSPControl.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSPControl.Dialogs
{
  public partial class DlgFileTransfer : WeifenLuo.WinFormsUI.Docking.DockContent
  {
    private const int MaxLogEntries = 1001;

    private readonly ICSPManager mICSPManager;

    private Dictionary<string, string> mJsonList;

    private int mCurrentFileCount;
    private int mCurrentFileSize;

    private bool mCreateJson;
    private bool mCreateJs;

    public DlgFileTransfer(ICSPManager manager)
    {
      InitializeComponent();

      mICSPManager = manager ?? throw new ArgumentNullException(nameof(manager));

      cmd_CreatePhysicalDevice.Click += OnCreateDeviceInfo;

      txt_PanelDirectory.TextChanged += OnPanelDirectoryTextChanged;
      cmd_OutputDirectoryBrowse.Click += OnOutputFolderBrowseClick;
      cmd_OpenFolder.Click += OnOpenFolderClick;

      cmd_CreateJson.Click += OnCreateJsonClick;
      cmd_CreateJs.Click += OnCreateJsClick;

      mICSPManager.ClientOnlineStatusChanged += OnClientOnlineStatusChanged;
      mICSPManager.DynamicDeviceCreated += OnDynamicDeviceCreated;

      // =====================================================================
      // FileManager-Events
      // =====================================================================

      mICSPManager.FileManager.OnTransferFileData += OnTransferFileData;

      //mICSPManager.FileManager.OnTransferFileDataComplete += OnTransferFileDataComplete;
      //mICSPManager.FileManager.OnTransferFileDataCompleteAck += OnTransferFileDataCompleteAck;
      mICSPManager.FileManager.OnTransferFilesInitialize += OnTransferFilesInitialize;
      mICSPManager.FileManager.OnTransferFilesComplete += OnTransferFilesComplete;

      mICSPManager.FileManager.OnGetDirectoryInfo += OnGetDirectoryInfo;
      mICSPManager.FileManager.OnDirectoryInfo += OnDirectoryInfo;
      mICSPManager.FileManager.OnDirectoryItem += OnDirectoryItem;
      mICSPManager.FileManager.OnDeleteFile += OnDeleteFile;
      mICSPManager.FileManager.OnCreateDirectory += OnCreateDirectory;

      // mICSPManager.FileManager.OnTransferSingleFile += OnTransferSingleFile;
      // mICSPManager.FileManager.OnTransferSingleFileAck += OnTransferSingleFileAck;
      mICSPManager.FileManager.OnTransferSingleFileInfo += OnTransferSingleFileInfo;
      // mICSPManager.FileManager.OnTransferSingleFileInfoAck += OnTransferSingleFileInfoAck;
      mICSPManager.FileManager.OnTransferGetFileAccessToken += OnTransferGetFileAccessToken;
      mICSPManager.FileManager.OnTransferGetFileAccessTokenAck += OnTransferGetFileAccessTokenAck;
      mICSPManager.FileManager.OnTransferGetFile += OnTransferGetFile;

      txt_PanelDirectory.Text = mICSPManager.FileManager.BaseDirectory;

      ckb_CreateJson.Checked = Settings.Default.FileTransferCreateJson;
      txt_FileNameJson.Text = Settings.Default.FileTransferFileNameJson;

      ckb_CreateJs.Checked = Settings.Default.FileTransferCreateJs;
      txt_FileNameJs.Text = Settings.Default.FileTransferFileNameJs;
      txt_VariableNameJs.Text = Settings.Default.FileTransferVariableNameJs;

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

    #region FileManager-Events

    private void OnTransferFileData(object sender, TransferFileDataEventArgs e)
    {
      mCurrentFileSize += e.JunkSize;

      if(mCurrentFileSize <= prb_2.Maximum)
        prb_2.Value = mCurrentFileSize;
      else
        prb_2.Value = prb_2.Maximum;
    }

    private void OnTransferFileDataComplete(object sender, EventArgs e)
    {
      AppendLog("OnTransferFileDataComplete", null);
    }

    private void OnTransferFileDataCompleteAck(object sender, EventArgs e)
    {
      AppendLog("OnTransferFileDataCompleteAck", null);
    }

    private void OnTransferFilesInitialize(object sender, TransferFilesInitializeEventArgs e)
    {
      AppendLog("OnTransferFilesInitialize: FileCount={0}", e.FileCount);

      mCurrentFileCount = 0;
      mCurrentFileSize = 0;

      prb_1.Value = 0;
      prb_1.Maximum = e.FileCount;

      prb_2.Value = 0;
    }

    private void OnTransferFilesComplete(object sender, EventArgs e)
    {
      mCurrentFileCount = 0;
      mCurrentFileSize = 0;

      prb_1.Value = 0;
      prb_2.Value = 0;

      AppendLog("OnTransferFilesComplete", null);

      try
      {
        var lTxt = string.Format(Properties.Resources.Js_Config, mICSPManager.Host, "8000");

        var lPath = Path.Combine(mICSPManager.FileManager.BaseDirectory, "js");

        Directory.CreateDirectory(Path.Combine(mICSPManager.FileManager.BaseDirectory, "js"));

        File.WriteAllText(Path.Combine(lPath, "config.js"), lTxt);
      }
      catch(Exception ex)
      {
        AppendLog("OnTransferFilesComplete: Error on create js\\config.js, Message={0:l}", ex.Message);
      }

      mCreateJson = ckb_CreateJson.Checked;
      mCreateJs = ckb_CreateJs.Checked;

      if(mCreateJson || mCreateJs)
      {
        CreateJsonList();
        ProccessJsonList();
      }
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
      if(mCurrentFileCount == 0)
        txt_Info.Clear();

      AppendLog("OnCreateDirectory: Directory={0}", e.Directory);
    }

    private void OnTransferSingleFile(object sender, EventArgs e)
    {
      AppendLog("OnTransferSingleFile", null);
    }

    private void OnTransferSingleFileAck(object sender, EventArgs e)
    {
      AppendLog("OnTransferSingleFileAck", null);
    }

    private void OnTransferSingleFileInfo(object sender, TransferSingleFileInfoEventArgs e)
    {
      if(++mCurrentFileCount <= prb_1.Maximum)
        prb_1.Value = mCurrentFileCount;
      else
        prb_1.Value = prb_1.Maximum;

      prb_2.Maximum = e.FileSize;

      mCurrentFileSize = 0;

      AppendLog("OnTransferSingleFileInfo: FileSize={0} bytes, FileName={1}", e.FileSize, e.FileName);
    }

    private void OnTransferSingleFileInfoAck(object sender, EventArgs e)
    {
      AppendLog("OnTransferSingleFileInfoAck", null);
    }

    private void OnTransferGetFileAccessToken(object sender, TransferGetFileAccessTokenEventArgs e)
    {
      AppendLog("OnTransferGetFileAccessToken: Size={0} bytes, FileName={1}", e.Size, e.FileName);
    }

    private void OnTransferGetFileAccessTokenAck(object sender, EventArgs e)
    {
      AppendLog("OnTransferGetFileAccessTokenAck", null);
    }

    private void OnTransferGetFile(object sender, EventArgs e)
    {
      AppendLog("OnTransferGetFile", null);
    }

    #endregion

    protected override void OnClosing(CancelEventArgs e)
    {
      SaveSettings();
    }

    private void OnDynamicDeviceCreated(object sender, DynamicDeviceCreatedEventArgs e)
    {
      if(Settings.Default.PhysicalDeviceAutoCreate)
        CreatePhysicalDevice();
    }

    private void OnPanelDirectoryTextChanged(object sender, EventArgs e)
    {
      try
      {
        mICSPManager.FileManager.SetBaseDirectory(txt_PanelDirectory.Text);
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex.Message);
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

    private void OnCreateJsonClick(object sender, EventArgs e)
    {
      txt_Info.Clear();

      mCreateJson = true;

      mCreateJs = ckb_CreateJs.Checked;

      CreateJsonList();

      ProccessJsonList();
    }

    private void OnCreateJsClick(object sender, EventArgs e)
    {
      txt_Info.Clear();

      mCreateJson = ckb_CreateJson.Checked;

      mCreateJs = true;

      CreateJsonList();

      ProccessJsonList();
    }

    private void OnOutputFolderBrowseClick(object sender, EventArgs e)
    {
      try
      {
        var lPanelDirectory = txt_PanelDirectory.Text.Trim();

        if(string.IsNullOrWhiteSpace(lPanelDirectory))
          lPanelDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var lDir = new DirectoryInfo(lPanelDirectory);

        FolderDialog.SelectedPath = lDir.FullName;

        if(FolderDialog.ShowDialog() == DialogResult.OK)
        {
          txt_PanelDirectory.Text = FolderDialog.SelectedPath;

          Settings.Default.FileTransferPanelDirectory = FolderDialog.SelectedPath;

          Settings.Default.Save();
        }
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex.Message);
      }
    }

    private void OnOpenFolderClick(object sender, EventArgs e)
    {
      try
      {
        var lPanelDirectory = txt_PanelDirectory.Text.Trim();

        if(string.IsNullOrWhiteSpace(lPanelDirectory))
          lPanelDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var lDir = new DirectoryInfo(lPanelDirectory);

        Process.Start(lDir.FullName);
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex.Message);
      }
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

    private void SaveSettings()
    {
      Settings.Default.FileTransferPanelDirectory = txt_PanelDirectory.Text.Trim();

      Settings.Default.FileTransferCreateJson = ckb_CreateJson.Checked;
      Settings.Default.FileTransferFileNameJson = txt_FileNameJson.Text.Trim();

      Settings.Default.FileTransferCreateJs = ckb_CreateJs.Checked;
      Settings.Default.FileTransferFileNameJs = txt_FileNameJs.Text.Trim();
      Settings.Default.FileTransferVariableNameJs = txt_VariableNameJs.Text.Trim();

      Settings.Default.Save();
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

    private void AppendLog(string format, params object[] args)
    {
      this.InvokeIfRequired(a =>
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
      });

      Application.DoEvents();
    }

    private void CreateJsonList()
    {
      try
      {
        var lDir = new DirectoryInfo(Path.Combine(mICSPManager.FileManager.BaseDirectory, "AMXPanel"));

        AppendLog("CreateJsonList: Directory={0}", lDir.FullName);

        mJsonList = new Dictionary<string, string>();

        var lXmlDoc = new XmlDocument();

        var lFiles = lDir
          .EnumerateFiles()
          .Where(file => file.Extension.Equals(".xml", StringComparison.OrdinalIgnoreCase) || file.Extension.Equals(".xma", StringComparison.OrdinalIgnoreCase))
          .ToList();

        foreach(var fileInfo in lFiles)
        {
          try
          {
            // Ignore ...
            switch(fileInfo.Name.ToLower())
            {
              case "g5apps.xma":
              case "logs.xma":
              case "manifest.xma": continue;
            }

            lXmlDoc.Load(fileInfo.FullName);

            // Import System Fonts
            // G4: %CommonProgramFiles(x86)%\AMXShare\G4SupportFiles\__system\graphics\fnt.xma
            if(fileInfo.Name.Equals("fonts.xma", StringComparison.OrdinalIgnoreCase))
            {
              var lXmlDocSysFonts = new XmlDocument();

              lXmlDocSysFonts.LoadXml(Resources.SysFonts);

              var lTargetNode = lXmlDoc.DocumentElement.SelectSingleNode("/root/fontList");

              if(lTargetNode.ChildNodes.Count == 0)
              {
                foreach(XmlNode node in lXmlDocSysFonts.DocumentElement.SelectNodes("/root/fontList/font"))
                {
                  var lNode = lXmlDoc.ImportNode(node, true);

                  lTargetNode.AppendChild(lNode);
                }
              }
              else
              {
                XmlNode lRefChild = null;

                foreach(XmlNode node in lXmlDocSysFonts.DocumentElement.SelectNodes("/root/fontList/font"))
                {
                  var lNode = lXmlDoc.ImportNode(node, true);

                  // Order by Font-Index
                  if(lRefChild == null)
                    lRefChild = lTargetNode.InsertBefore(lNode, lTargetNode.FirstChild);
                  else
                    lRefChild = lTargetNode.InsertAfter(lNode, lRefChild);
                }
              }
            }

            // Remove XmlDeclaration
            foreach(XmlNode node in lXmlDoc)
            {
              if(node.NodeType == XmlNodeType.XmlDeclaration)
                lXmlDoc.RemoveChild(node);
            }

            JsonArrrayHelper(lXmlDoc);

            var lJson = JsonConvert.SerializeXmlNode(lXmlDoc, Newtonsoft.Json.Formatting.Indented, true);

            // <page type="page">
            // JSON.NET and Replacing @ Sign in XML to JSON converstion
            lJson = (Regex.Replace(lJson, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase));

            mJsonList.Add(fileInfo.Name, lJson);
          }
          catch(Exception ex)
          {
            Logger.LogError(ex);

            AppendLog("CreateJsonList: Error, Message={0:l}", ex.Message);
          }
        }
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex.Message);
      }
    }

    private void ProccessJsonList()
    {
      try
      {
        var lDir = new DirectoryInfo(Path.Combine(mICSPManager.FileManager.BaseDirectory, "AMXPanel"));

        AppendLog("ProccessJsonList: Directory={0}", lDir.FullName);

        if((mJsonList?.Count ?? 0) == 0)
          return;

        try
        {
          var lJsonProject = new JObject();

          // Main-Properties order:
          // -------------------------------------------------------
          lJsonProject.Add(new JProperty("project"));
          lJsonProject.Add(new JProperty("pages", new JArray()));
          lJsonProject.Add(new JProperty("map"));
          lJsonProject.Add(new JProperty("palettes", new JArray()));
          lJsonProject.Add(new JProperty("fontList", new JArray()));

          foreach(var keyValue in mJsonList)
          {
            var lJsonObj = JObject.Parse(keyValue.Value).SelectToken("", false);

            // Omit root object
            if((lJsonObj.First as JProperty)?.Name == "root")
              lJsonObj = lJsonObj.SelectToken("root", false);

            var lProperty = lJsonObj.First as JProperty;

            // Resources (cm/bm/sm/..)
            switch(keyValue.Key.ToLower())
            {
              case "prj.xma"     /**/: lJsonProject["project"] = lJsonObj; break;
              case "map.xma"     /**/: lJsonProject["map"] = lJsonObj; break;
              case "fonts.xma"   /**/: ((JArray)lJsonProject["fontList"]).Add(lJsonObj["fontList"]["font"]); break;
              case "pal_001.xma" /**/: ((JArray)lJsonProject["palettes"]).Add(lJsonObj); break;
              default:
              {
                switch(lProperty?.Name)
                {
                  case "versionInfo" /**/: lJsonProject["project"] = lJsonObj; break;
                  case "page"        /**/: ((JArray)lJsonProject["pages"]).Add(lJsonObj); break;
                  case "cm"          /**/:
                  case "bm"          /**/:
                  case "sm"          /**/: lJsonProject["map"] = lJsonObj; break;
                  case "paletteData" /**/: ((JArray)lJsonProject["palettes"]).Add(lJsonObj); break;
                  default            /**/: lJsonProject.Merge(lJsonObj); break;
                }

                break;
              }
            }
          }

          var lJSonProjStr = lJsonProject.ToString();

          // Project -> JSON
          if(mCreateJson)
          {
            var lFileName = string.Format(@"{0}\{1}", lDir.FullName, txt_FileNameJson.Text).ToLower();

            File.WriteAllText(lFileName, lJSonProjStr);
          }

          // Project -> JavaScript
          if(mCreateJs)
          {
            lJSonProjStr = string.Format("var {0} = {1}", txt_VariableNameJs.Text, lJSonProjStr);

            var lFileNameJsProject = string.Format(@"{0}\{1}", lDir.FullName, txt_FileNameJs.Text);

            File.WriteAllText(lFileNameJsProject, lJSonProjStr);
          }
        }
        catch(Exception ex)
        {
          AppendLog("CreateJson: Error, Message={0:l}", ex.Message);
        }
      }
      catch(Exception ex)
      {
        ErrorMessageBox.Show(ex.Message);
      }
    }

    private void JsonArrrayHelper(XmlDocument xmlDoc)
    {
      var lNamespace = "http://james.newtonking.com/projects/json";

      /*
      From Json.NET documentation: http://james.newtonking.com/projects/json/help/?topic=html/ConvertingJSONandXML.htm
      You can force a node to be rendered as an Array by adding the attribute json:Array='true' to the XML node you are converting to JSON.

      Also, you need to declare the json prefix namespace at the XML header xmlns:json='http://james.newtonking.com/projects/json' or 
      else you will get an XML error stating that the json prefix is not declared.

      The next example is provided by the documentation:
      xml = @"<person xmlns:json='http://james.newtonking.com/projects/json' id='1'>
            <name>Alan</name>
            <url>http://www.google.com</url>
            <role json:Array='true'>Admin</role>
          </person>";

      */

      xmlDoc.DocumentElement.SetAttribute("xmlns:json", lNamespace);

      // Pages    : /root/pageList/pageEntry[]
      // PageFlips: /root/page/button/pf
      var lElements = xmlDoc.SelectNodes("/root/pageList/pageEntry|/rootresourceList/resource|/root/page/button|/root/page/button/pf|/root/tableList/tableEntry|/root/tableList/tableEntry/row");

      foreach(XmlElement element in lElements)
      {
        var lAttr = xmlDoc.CreateAttribute("Array", lNamespace);

        lAttr.Value = "true";

        element.Attributes.Append(lAttr);
      }
    }
  }
}