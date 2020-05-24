using System;

namespace ICSP.WebProxy
{
  public sealed class MessageEventArgs : EventArgs
  {
    public MessageEventArgs(string message, int socketId)
    {
      Message = message;

      SocketId = socketId;
    }

    public string Message { get; }

    public int SocketId { get; }
  }
}
