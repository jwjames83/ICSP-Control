using System;

namespace ICSP.Manager
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class MsgCmdAttribute : Attribute
  {
    public MsgCmdAttribute(ushort msgCmd)
    {
      MsgCmd = msgCmd;
    }
    
    public ushort MsgCmd { get; }
  }
}