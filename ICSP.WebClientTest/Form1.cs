using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSP.WebClientTest.Extensions;

namespace ICSP.WebClientTest
{
  public partial class Form1 : Form
  {
    private WebSocketClient _WebSocket1;
    private WebSocketClient _WebSocket2;

    public Form1()
    {
      InitializeComponent();

      _WebSocket1 = new WebSocketClient() { ID = 1 };
      _WebSocket2 = new WebSocketClient() { ID = 2 };

      _WebSocket1.OnMessage += _WebSocket1_OnMessage;
      _WebSocket2.OnMessage += _WebSocket2_OnMessage;

      cmd_Open1.Click += Cmd_Open1_Click;
      cmd_Open2.Click += Cmd_Open2_Click;

      cmd_Close1.Click += Cmd_Close1_Click;
      cmd_Close2.Click += Cmd_Close2_Click;

      cmd_Send1.Click += Cmd_Send1_Click;
      cmd_Send2.Click += Cmd_Send2_Click;
    }

    private void Cmd_Open1_Click(object sender, EventArgs e)
    {
      try
      {
        var lUri = new Uri(txt_Url1.Text);

        _ = _WebSocket1.StartAsync(lUri);
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.InnerException.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void Cmd_Open2_Click(object sender, EventArgs e)
    {
      try
      {
        var lUri = new Uri(txt_Url2.Text);

        _ = _WebSocket2.StartAsync(lUri);
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void Cmd_Close1_Click(object sender, EventArgs e)
    {
      try
      {
        _ = _WebSocket1.StopAsync();

      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void Cmd_Close2_Click(object sender, EventArgs e)
    {
      try
      {
        _ = _WebSocket2.StopAsync();
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void Cmd_Send1_Click(object sender, EventArgs e)
    {
      try
      {
        _ = _WebSocket1.SendAsync(txt_Send1.Text.Trim());
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void Cmd_Send2_Click(object sender, EventArgs e)
    {
      try
      {
        _ = _WebSocket2.SendAsync(txt_Send2.Text.Trim());
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void _WebSocket1_OnMessage(object sender, string e)
    {
      this.InvokeIfRequired(a =>
      {
        txt_Data1.AppendText(e + Environment.NewLine);
      });
    }

    private void _WebSocket2_OnMessage(object sender, string e)
    {
      this.InvokeIfRequired(a =>
      {
        txt_Data2.AppendText(e + Environment.NewLine);
      });
    }
  }
}
