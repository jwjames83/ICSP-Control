using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSP;
using ICSP.Constants;
using ICSP.Manager.ConnectionManager;
using ICSP.Manager.DeviceManager;

namespace ICSPControl.Dialogs
{
  public partial class DlgTrace : Form
  {
    private const int MaxLogEntries = 1001;

    private readonly ICSPManager mManager;

    private bool mTraceEnabled;

    public DlgTrace(ICSPManager manager)
    {
      InitializeComponent();

      mManager = manager ?? throw new ArgumentNullException(nameof(manager));

      mManager.CommandNotImplemented += OnCommandNotImplemented;

      // Default Filter
      ckb_DeviceInfo.Checked = true;
      ckb_PortCount.Checked = true;
      ckb_DynamicDeviceCreated.Checked = true;
      ckb_ChannelEvent.Checked = true;
      ckb_StringEvent.Checked = true;
      ckb_LevelEvent.Checked = true;
      ckb_Others.Checked = true;

      ckb_PingMessage.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_BlinkMessage.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_DeviceInfo.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_PortCount.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_DynamicDeviceCreated.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_ChannelEvent.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_StringEvent.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_CommandEvent.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_LevelEvent.CheckedChanged += (s, e) => ConfigureEventHandler();
      ckb_Others.CheckedChanged += (s, e) => ConfigureEventHandler();

      mTraceEnabled = true;

      cmd_StartStopTrace.Text = "Stop Trace";

      ConfigureEventHandler();

      cmd_StartStopTrace.Click += OnStartStopTraceClick;
      cmd_ClearLog.Click += OnClearLogClick;
    }

    private void OnCommandNotImplemented(object sender, ICSPMsgDataEventArgs e)
    {
      /*
      AppendLog(e.Message.ID, "NotImplemented: Protocol : {0}", e.Message.Protocol);
      AppendLog(e.Message.ID, "NotImplemented: Length   : {0}", e.Message.Length);
      AppendLog(e.Message.ID, "NotImplemented: Flag     : {0}", e.Message.Flag);
      AppendLog(e.Message.ID, "NotImplemented: Dest     : {0}", e.Message.Dest);
      AppendLog(e.Message.ID, "NotImplemented: Source   : {0}", e.Message.Source);
      AppendLog(e.Message.ID, "NotImplemented: Hop      : {0}", e.Message.Hop);
      */
      AppendLog(e.Message.ID, "NotImplemented: MessageId: 0x{0:X4}", e.Message.ID);
      AppendLog(e.Message.ID, "NotImplemented: Command  : 0x{0:X4} ({1})", e.Message.Command, ICSPMsg.GetFrindlyName(e.Message.Command));
      AppendLog(e.Message.ID, "NotImplemented: Checksum : 0x{0:X4}", e.Message.Checksum);
      AppendLog(e.Message.ID, "NotImplemented: Data (0x): {0}", BitConverter.ToString(e.Message.Data).Replace("-", " "));
    }

    private void OnStartStopTraceClick(object sender, EventArgs e)
    {
      mTraceEnabled = !mTraceEnabled;

      ConfigureEventHandler();

      if(mTraceEnabled)
        cmd_StartStopTrace.Text = "Stop Trace";
      else
        cmd_StartStopTrace.Text = "Start Trace";
    }

    private void OnClearLogClick(object sender, EventArgs e)
    {
      txt_Log.Clear();
    }

    public void AppendLog(ushort id, string format)
    {
      AppendLog(id, format, null);
    }

    public void AppendLog(ushort id, string format, params object[] args)
    {
      if(!mTraceEnabled)
        return;

      var lMessage = format;

      if(args != null && args.Length > 0)
        lMessage = string.Format(format, args);

      txt_Log.AppendText(string.Format("{0:yyy-MM-dd (HH:mm.ss)}: ID=0x{1:X4}, {2}\r\n", DateTime.Now, id, lMessage));

      if(txt_Log.Lines.Length > MaxLogEntries)
      {
        var newLines = new string[MaxLogEntries];

        Array.Copy(txt_Log.Lines, 1, newLines, 0, MaxLogEntries);

        txt_Log.Lines = newLines;
      }

      txt_Log.SelectionStart = txt_Log.Text.Length;

      txt_Log.ScrollToCaret();

      Application.DoEvents();
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
      AppendLog(e.Message.ID, "MessageReceived - Command=0x{0:X4}, {1}", e.Message.Command, ICSPMsg.GetFrindlyName(e.Message.Command));
    }

    private void OnBlinkMessage(object sender, BlinkEventArgs e)
    {
      if(ckb_BlinkMessage.Checked)
      {
        if((e.LED & 0x01) == 1)
          AppendLog(e.Message.ID, "BlinkMessage - DateTime={0:dd.MM.yyyy HH:mm:ss}, LED=On", e.DateTime);
        else
          AppendLog(e.Message.ID, "BlinkMessage - DateTime={0:dd.MM.yyyy HH:mm:ss}, LED=Off", e.DateTime);
      }
    }

    private void OnPingEvent(object sender, PingEventArgs e)
    {
      if(ckb_PingMessage.Checked)
        AppendLog(e.Message.ID, "PingEvent - Device={0}, System={1}", e.Device, e.System);
    }

    private void OnChannelEvent(object sender, ChannelEventArgs e)
    {
      if(e.Enabled)
        AppendLog(e.Message.ID, "[{0}] Output Channel: On  - Channel {1}", e.Device, e.Channel);
      else
        AppendLog(e.Message.ID, "[{0}] Output Channel: Off - Channel {1}", e.Device, e.Channel);
    }

    private void OnStringEvent(object sender, StringEventArgs e)
    {
      AppendLog(e.Message.ID, "[{0}] String={1}", e.Message.Dest, e.Text);
    }

    private void OnCommandEvent(object sender, CommandEventArgs e)
    {
      AppendLog(e.Message.ID, "[{0}] Command={1}", e.Message.Dest, e.Text);
    }

    private void OnLevelEvent(object sender, LevelEventArgs e)
    {
      AppendLog(e.Message.ID, "[{0}] Level: Type={1}, Value={2}", e.Message.Dest, e.ValueType, e.Value);
    }

    private void OnDeviceInfo(object sender, DeviceInfoEventArgs e)
    {
      AppendLog(e.Message.ID, "DeviceInfo - Device={0:00000}, Firmware={1}, Description={2}", e.Device, e.Version, e.Name);
    }

    private void OnPortCount(object sender, PortCountEventArgs e)
    {
      AppendLog(e.Message.ID, "PortCount  - Device={0:00000}, System={1}, PortCount={2}", e.Device, e.System, e.PortCount);
    }

    private void OnDynamicDeviceCreated(object sender, DynamicDeviceCreatedEventArgs e)
    {
      AppendLog(e.Message.ID, "DynamicDeviceCreatedEventArgs - Device={0:00000}, System={1},", e.Device, e.System);
    }

    private void ConfigureEventHandler()
    {
      // Remove all events
      mManager.PingEvent -= OnPingEvent;
      mManager.BlinkMessage -= OnBlinkMessage;
      mManager.DeviceInfo -= OnDeviceInfo;
      mManager.PortCount -= OnPortCount;
      mManager.DynamicDeviceCreated -= OnDynamicDeviceCreated;
      mManager.ChannelEvent -= OnChannelEvent;
      mManager.StringEvent -= OnStringEvent;
      mManager.CommandEvent -= OnCommandEvent;
      mManager.LevelEvent -= OnLevelEvent;
      mManager.MessageReceived -= OnMessageReceived;

      if(mTraceEnabled)
      {
        if(ckb_PingMessage.Checked) mManager.PingEvent += OnPingEvent;
        if(ckb_BlinkMessage.Checked) mManager.BlinkMessage += OnBlinkMessage;
        if(ckb_DeviceInfo.Checked) mManager.DeviceInfo += OnDeviceInfo;
        if(ckb_PortCount.Checked) mManager.PortCount += OnPortCount;
        if(ckb_DynamicDeviceCreated.Checked) mManager.DynamicDeviceCreated += OnDynamicDeviceCreated;
        if(ckb_ChannelEvent.Checked) mManager.ChannelEvent += OnChannelEvent;
        if(ckb_StringEvent.Checked) mManager.StringEvent += OnStringEvent;
        if(ckb_CommandEvent.Checked) mManager.CommandEvent += OnCommandEvent;
        if(ckb_LevelEvent.Checked) mManager.LevelEvent += OnLevelEvent;
        if(ckb_Others.Checked) mManager.MessageReceived += OnMessageReceived;
      }

      if(mTraceEnabled)
        cmd_StartStopTrace.Text = "Stop Trace";
      else
        cmd_StartStopTrace.Text = "Start Trace";
    }
  }
}
