using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSP;
using ICSP.Constants;
using ICSP.Manager.ConnectionManager;
using ICSP.Manager.DeviceManager;

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

      mICSPManager.MessageReceived += OnMessageReceived;
      mICSPManager.BlinkMessage += OnBlinkMessage;
      mICSPManager.PingEvent += OnPingEvent; ;
      mICSPManager.ChannelEvent += OnChannelEvent;
      mICSPManager.DeviceInfo += OnDeviceInfo;
      mICSPManager.PortCount += OnPortCount;
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

    public void AppendText(ushort id, string format)
    {
      AppendText(id, format, null);
    }

    public void AppendText(ushort id, string format, params object[] args)
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

      mLogQueue.Enqueue(string.Format("{0:yyy-MM-dd (HH:mm.ss)}: ID=0x{1:X4}, {2}", DateTime.Now, id, lMessage));

      txt_Text.Text = string.Join(System.Environment.NewLine, mLogQueue.ToArray());

      txt_Text.SelectionStart = txt_Text.Text.Length;
      txt_Text.ScrollToCaret();
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
      var lAppend = true;

      switch(e.Message.Command)
      {
        case ConnectionManagerCmd.BlinkMessage: lAppend = false; break;
        case ConnectionManagerCmd.PingRequest: lAppend = false; break;

        case DeviceManagerCmd.OutputChannelOn: lAppend = false; break;
        case DeviceManagerCmd.OutputChannelOff: lAppend = false; break;

        case DeviceManagerCmd.DeviceInfo: lAppend = false; break;
        case DeviceManagerCmd.PortCountBy: lAppend = false; break;
      }

      if(lAppend)
      {
        switch(e.Message)
        {
          case MsgCmdDynamicDeviceAddressResponse m:
          {
            AppendText(e.Message.ID, "DataReceived - Command=0x{0:X4}, {1}, Device={2}, System={3}", e.Message.Command, ICSPMsg.GetFrindlyName(e.Message.Command), m.Device, m.System);
            break;
          }
          case MsgCmdStringMasterDev m:
          {
            AppendText(e.Message.ID, "[{0}] String={1}", e.Message.Dest, m.Text);
            break;
          }
          case MsgCmdCommandMasterDev m:
          {
            AppendText(e.Message.ID, "[{0}] Command={1}", e.Message.Dest, m.Text);
            break;
          }
          case MsgCmdLevelValueMasterDev m:
          {
            AppendText(e.Message.ID, "[{0}] Level=Type={1}, Value={2}", e.Message.Dest, m.ValueType, m.Value);
            break;
          }
          default:
          {
            AppendText(e.Message.ID, "DataReceived - Command=0x{0:X4}, {1}", e.Message.Command, ICSPMsg.GetFrindlyName(e.Message.Command));
            break;
          }
        }
      }
    }

    private void OnBlinkMessage(object sender, BlinkEventArgs e)
    {
      if(ckb_ShowBlink.Checked)
      {
        if((e.LED & 0x01) == 1)
          AppendText(e.Message.ID, "BlinkMessage - DateTime={0:dd.MM.yyyy HH:mm:ss}, LED=On", e.DateTime);
        else
          AppendText(e.Message.ID, "BlinkMessage - DateTime={0:dd.MM.yyyy HH:mm:ss}, LED=Off", e.DateTime);
      }
    }

    private void OnPingEvent(object sender, PingEventArgs e)
    {
      if(ckb_ShowPing.Checked)
        AppendText(e.Message.ID, "PingEvent - Device={0}, System={1}", e.Device, e.System);
    }

    private void OnChannelEvent(object sender, ChannelEventArgs e)
    {
      if(e.Enabled)
        AppendText(e.Message.ID, "[{0}] Output Channel: On  - Channel {1}", e.Device, e.Channel);
      else
        AppendText(e.Message.ID, "[{0}] Output Channel: Off - Channel {1}", e.Device, e.Channel);
    }

    private void OnDeviceInfo(object sender, DeviceInfoEventArgs e)
    {
      AppendText(e.Message.ID, "DeviceInfo - Device={0:00000}, Firmware={1}, Description={2}", e.Device, e.Version, e.Name);
    }

    private void OnPortCount(object sender, PortCountEventArgs e)
    {
      AppendText(e.Message.ID, "PortCount  - Device={0:00000}, System={1}, PortCount={2}", e.Device, e.System, e.PortCount);
    }
  }
}
