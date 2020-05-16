using System;
using System.Linq;
using System.Reflection;

using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  public abstract class MsgBaseCmdChannel<T> : ICSPMsg
  {
    protected MsgBaseCmdChannel()
    {
    }

    public MsgBaseCmdChannel(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(Data.Range(0, 6));

        Channel = Data.GetBigEndianInt16(6);
      }
    }

    private static MsgBaseCmdChannel<T> CreateType()
    {
      var lConstructor =
        typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
          .Where(constructor => constructor.GetParameters().Length == 0)
          .FirstOrDefault();

      if(lConstructor != null)
        return (MsgBaseCmdChannel<T>)lConstructor.Invoke(null);

      throw new ApplicationException("Command need an empty constructor");
    }

    public static ICSPMsg CreateRequest(AmxDevice source, AmxDevice device, ushort channel)
    {
      var lRequest = CreateType();
      
      lRequest.Device = device;
      lRequest.Channel = channel;

      var lData = device.GetBytesDPS().Concat(ArrayExtensions.Int16ToBigEndian(channel)).ToArray();

      return lRequest.Serialize(device, source, lRequest.MsgCmd, lData);
    }

    protected abstract ushort MsgCmd { get; }

    public AmxDevice Device { get; set; }

    public ushort Channel { get; set; }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device : {1:l}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} Channel: {1}", GetType().Name, Channel);
    }
  }
}
