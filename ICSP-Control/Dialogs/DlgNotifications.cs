using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSP;
using ICSP.Constants;
using ICSP.Manager.ConnectionManager;

namespace ICSPControl.Dialogs
{
  public partial class DlgNotifications : Form
  {
    private const int MaxLogEntries = 1000;

    private ICSPManager mICSPManager;

    private Queue<string> mLogQueue = new Queue<string>();

    private bool mLogEnabled;

    public DlgNotifications(ICSPManager manager)
    {
      InitializeComponent();

      if(manager == null)
        throw new ArgumentNullException(nameof(manager));

      mICSPManager = manager;

      mLogEnabled = true;

      cmd_StartStopLog.Click += Cmd_StartStopLog_Click;
      cmd_ClearLog.Click += OnClearLogClick;

      mICSPManager.ChannelEvent += OnChannelEvent; ;
      mICSPManager.MessageReceived += OnDataReceived;
      mICSPManager.DeviceInfo += OnDeviceInfo; ;
    }

    private void Cmd_StartStopLog_Click(object sender, EventArgs e)
    {
      mLogEnabled = !mLogEnabled;

      if(mLogEnabled)
        cmd_StartStopLog.Text = "Stop Log";
      else
        cmd_StartStopLog.Text = "Start Log";
    }

    private void OnClearLogClick(object sender, EventArgs e)
    {
      mLogQueue.Clear();
      txt_Text.Clear();
    }
    
    public void AppendText(string format)
    {
      AppendText(format, null);
    }

    public void AppendText(string format, params object[] args)
    {
      // This should only ever run for 1 loop as you should never go over logMax
      // but if you accidentally manually added to the logQueue - then this would
      // re-adjust you back down to the desired number of log items.
      while(mLogQueue.Count > MaxLogEntries - 1)
        mLogQueue.Dequeue();

      if(!mLogEnabled)
        return;

      /*
      2017-07-25 (20:23:32):: String To[5001:1:1] -[$A9$01$02$01$00$00$03$9A]
      2017-07-25 (20:23:33):: Feedback:On[5001:1:1] - Channel 4
      2017-07-25 (20:23:33):: Output Channel: On - From[5001:1:1] - Channel 4
      2017-07-25 (20:23:33):: String To[5001:1:1] -[$A9$00$01$01$00$00$01$9A]
      2017-07-25 (20:23:33):: Feedback:Off[5001:1:1] - Channel 4
      2017-07-25 (20:23:33):: Output Channel: Off - From[5001:1:1] - Channel 4
      2017-07-25 (20:23:33):: String To[5001:1:1] -[$A9$000$01$00$001$9A]
      */

      var lMessage = format;

      if(args != null && args.Length > 0)
        lMessage = string.Format(format, args);
      
      mLogQueue.Enqueue(string.Format("{0:yyy-MM-dd (HH:mm.ss)}:: {1}", DateTime.Now, lMessage));

      txt_Text.Text = string.Join(System.Environment.NewLine, mLogQueue.ToArray());

      txt_Text.SelectionStart = txt_Text.Text.Length;
      txt_Text.ScrollToCaret();
    }

    private void OnChannelEvent(object sender, ChannelEventArgs e)
    {
      if(e.Enabled)
        AppendText("Output Channel: On  - Channel {0}", e.ChannelCode);
      else
        AppendText("Output Channel: Off - Channel {0}", e.ChannelCode);
    }

    private void OnDataReceived(object sender, MessageReceivedEventArgs e)
    {
      var lAppend = true;

      switch(e.Message.Command)
      {
        case ConnectionManagerCmd.BlinkMessage: lAppend = ckb_ShowBlink.Checked; break;
        case ConnectionManagerCmd.PingRequest: lAppend = ckb_ShowPing.Checked; break;
      }

      if(lAppend)
      {
        if(e.Message.Command == ConnectionManagerCmd.BlinkMessage)
        {
          var lMsg = e.Message as MsgCmdBlinkMessage;

          if(lMsg != null)
          {
            if((lMsg.LED & 0x01) == 1)
              AppendText("BlinkMessage - ID=0x{0:X4}, DateTime={1:dd.MM.yyyy HH:mm:ss}, LED=On", e.Message.ID, lMsg.DateTime);
            else
              AppendText("BlinkMessage - ID=0x{0:X4}, DateTime={1:dd.MM.yyyy HH:mm:ss}, LED=Off", e.Message.ID, lMsg.DateTime);
          }
          else
            AppendText("BlinkMessage - ID=0x{0:X4}, Command=0x{1:X4}, {2}", e.Message.ID, e.Message.Command, ICSPMsg.GetFrindlyName(e.Message.Command));
        }
        else
          AppendText("DataReceived - ID=0x{0:X4}, Command=0x{1:X4}, {2}", e.Message.ID, e.Message.Command, ICSPMsg.GetFrindlyName(e.Message.Command));
      }
    }

    private void OnDeviceInfo(object sender, DeviceInfoEventArgs e)
    {
      AppendText("OnDeviceInfo - Device={0:00000}, Firmware={1}, Description={2}", e.Device, e.Version, e.Name);
    }
  }
}
