using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSP.WebClientTest.Extensions;

namespace ICSP.WebClientTest
{
  public partial class TestForm : Form
  {
    private readonly WebSocketClient mWebSocket1;
    private readonly WebSocketClient mWebSocket2;

    public TestForm()
    {
      InitializeComponent();

      mWebSocket1 = new WebSocketClient() { ID = 1 };
      mWebSocket2 = new WebSocketClient() { ID = 2 };

      mWebSocket1.OnMessage += WebSocket1_OnMessage;
      mWebSocket2.OnMessage += WebSocket2_OnMessage;

      cmd_Open1.Click += Cmd_Open1_Click;
      cmd_Open2.Click += Cmd_Open2_Click;

      cmd_Close1.Click += Cmd_Close1_Click;
      cmd_Close2.Click += Cmd_Close2_Click;

      cmd_Send1.Click += Cmd_Send1_Click;
      cmd_Send2.Click += Cmd_Send2_Click;

      cmd_Push1.MouseDown += Cmd_Push1_MouseDown;
      cmd_Push1.MouseUp += Cmd_Push1_MouseUp;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);
    }

    private async void Cmd_Push1_MouseDown(object sender, MouseEventArgs e)
    {
      cmd_Push1.Text = "Release";

      var lStr = string.Format("PUSH:{0}:{1};", num_ChannelPort1.Value, num_ChannelChannel1.Value);

      await mWebSocket1.SendAsync(lStr);
    }

    private async void Cmd_Push1_MouseUp(object sender, MouseEventArgs e)
    {
      cmd_Push1.Text = "Push";

      var lStr = string.Format("RELEASE:{0}:{1};", num_ChannelPort1.Value, num_ChannelChannel1.Value);

      await mWebSocket1.SendAsync(lStr);
    }

    private async void Cmd_Open1_Click(object sender, EventArgs e)
    {
      try
      {
        var lUri = new Uri(txt_Url1.Text);

        await mWebSocket1.StopAsync();
        await mWebSocket1.StartAsync(lUri);
      }
      catch(Exception ex)
      {
        txt_Data1.AppendText($"Error: {ex.Message}" + Environment.NewLine);
      }
    }

    private async void Cmd_Open2_Click(object sender, EventArgs e)
    {
      try
      {
        var lUri = new Uri(txt_Url2.Text);

        await mWebSocket2.StopAsync();
        await mWebSocket2.StartAsync(lUri);
      }
      catch(Exception ex)
      {
        txt_Data2.AppendText($"Error: {ex.Message}" + Environment.NewLine);
      }
    }

    private async void Cmd_Close1_Click(object sender, EventArgs e)
    {
      try
      {
        await mWebSocket1.StopAsync();
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void Cmd_Close2_Click(object sender, EventArgs e)
    {
      try
      {
        await mWebSocket2.StopAsync();
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void Cmd_Send1_Click(object sender, EventArgs e)
    {
      try
      {
        await mWebSocket1.SendAsync(txt_Send1.Text.Trim());
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private async void Cmd_Send2_Click(object sender, EventArgs e)
    {
      try
      {
        await mWebSocket2.SendAsync(txt_Send2.Text.Trim());
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void WebSocket1_OnMessage(object sender, string e)
    {
      this.InvokeIfRequired(a =>
      {
        txt_Data1.AppendText(e + Environment.NewLine);
      });
    }

    private void WebSocket2_OnMessage(object sender, string e)
    {
      this.InvokeIfRequired(a =>
      {
        txt_Data2.AppendText(e + Environment.NewLine);
      });
    }
  }
}
