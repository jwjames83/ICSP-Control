using System;
using System.Linq;
using System.Reflection;

using ICSP.Core.Extensions;
using ICSP.Core.Logging;

namespace ICSP.Core.Manager.DeviceManager
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

    // AmxDevice dest, AmxDevice source

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, ushort channel)
    {
      var lRequest = CreateType();
      
      lRequest.Device = source;
      lRequest.Channel = channel;

      var lData = source.GetBytesDPS().Concat(ArrayExtensions.Int16ToBigEndian(channel)).ToArray();

      return lRequest.Serialize(dest, source, lRequest.MsgCmd, lData);
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
