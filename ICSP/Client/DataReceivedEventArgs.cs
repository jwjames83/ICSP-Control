using System;

namespace ICSP.Client
{
  public sealed class DataReceivedEventArgs : EventArgs
  {
    public DataReceivedEventArgs(byte[] bytes)
    {
      Bytes = bytes;
    }

    public byte[] Bytes { get; private set; }
  }
}
