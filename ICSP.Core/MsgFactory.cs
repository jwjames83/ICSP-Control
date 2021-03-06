using System;
using System.Collections.Generic;

using ICSP.Core.Manager;
using ICSP.Core.Reflection;

namespace ICSP.Core
{
  public class MsgFactory
  {
    private readonly Dictionary<ushort, ICSPMsg> mTypes;

    public MsgFactory()
    {
      mTypes = new Dictionary<ushort, ICSPMsg>();

      var lTypes = TypeHelper.GetSublassesOfType(typeof(ICSPMsg));

      foreach(var type in lTypes)
      {
        if(type.IsAssignableFrom(typeof(ICSPMsg)))
          throw new ArgumentException("MessageType is not assignable from ICSPMsg", nameof(type));

        var lAttributes = AttributeHelper.GetList<MsgCmdAttribute>(type);

        foreach(var attribute in lAttributes)
        {
          try
          {
            var lType = (ICSPMsg)Activator.CreateInstance(type, true);

            mTypes.Add(attribute.MsgCmd, lType);
          }
          catch(Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
      }
    }

    public object FromData(byte[] bytes)
    {
      if(bytes == null || bytes.Length == 0)
        throw new ArgumentNullException(nameof(bytes));

      var lProtocol = bytes[0];

      switch(lProtocol)
      {
        case ICSPMsg.ProtocolValue:
        {
          if(bytes.Length < ICSPMsg.PacketLengthMin)
            throw new ArgumentException(nameof(bytes));

          var lCmd = (ushort)((bytes[20] << 8) | bytes[21]);

          if(mTypes.ContainsKey(lCmd))
            return mTypes[lCmd].FromData(bytes);

          return new MsgCmdUnknown(bytes);
        }

        case ICSPEncryptedMsg.ProtocolValue:
        {
          if(bytes.Length < ICSPEncryptedMsg.PacketLengthMin)
            throw new ArgumentException(nameof(bytes));

          return ICSPEncryptedMsg.FromData(bytes);
        }

      }
      throw new Exception($"Unknonw Protocol: Value=0x{lProtocol:X2}");

    }
  }
}
