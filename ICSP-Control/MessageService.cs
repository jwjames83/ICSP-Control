using System;

namespace ICSPControl
{
  public delegate void MessageEventHandler(object sender, MessageEventArgs e);

  public sealed class MessageEventArgs : EventArgs
  {
    public MessageEventArgs(string text)
    {
      Text = text;
    }

    public string Text
    {
      get;
      private set;
    }
  }

  public class MessageService
  {
    public static event MessageEventHandler OnMessage;
    
    public static void CreateMsg(object sender, string text)
    {
      OnMessage?.Invoke(sender, new MessageEventArgs(text));
    }
  }
}
