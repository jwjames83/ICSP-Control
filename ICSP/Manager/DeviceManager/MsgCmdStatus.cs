using System.IO;
using System.Text;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.DeviceManager
{
  /// <summary>
  /// This message is the response from a master to the Request Status Code message above.
  /// It is sent by a device/port if the device/port needs to update the master of a status change.
  /// </summary>
  [MsgCmd(DeviceManagerCmd.Status)]
  public class MsgCmdStatus : ICSPMsg
  {
    public const int MsgCmd = DeviceManagerCmd.Status;

    private MsgCmdStatus()
    {
    }

    public MsgCmdStatus(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        Device = AmxDevice.FromDPS(Data.Range(0, 6));

        // StatusCode
        StatusCode = (StatusType)Data.GetBigEndianInt16(6);

        // ValueType
        ValueType = Data[8];

        // Length
        Length = Data.GetBigEndianInt16(9);

        // SerialNumber
        StatusString = AmxUtils.GetString(Data, 11, Length);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdStatus(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, AmxDevice device, StatusType statusCode, byte valueType, string statusString)
    {
      var lStatusString = statusString ?? string.Empty;

      var lBytes = Encoding.Default.GetBytes(lStatusString);

      var lRequest = new MsgCmdStatus
      {
        Device = device,
        StatusCode = statusCode,
        ValueType = valueType,
        Length = (ushort)(lBytes.Length),
        StatusString = statusString
      };

      byte[] lData;

      using(var lStream = new MemoryStream())
      {
        // Device
        lStream.Write(lRequest.Device.GetBytesDPS(), 0, 6);

        // StatusCode
        lStream.Write(AmxUtils.Int16ToBigEndian((ushort)lRequest.StatusCode), 0, 2);

        // ValueType
        lStream.Write(AmxUtils.Int16To8Bit(lRequest.ValueType), 0, 1);

        // Length
        lStream.Write(AmxUtils.Int16ToBigEndian(lRequest.Length), 0, 2);

        // StatusString
        lStream.Write(lBytes, 0, lBytes.Length);

        lData = lStream.ToArray();
      }

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} Device      : {1:00000}", GetType().Name, Device);
      Logger.LogDebug(false, "{0:l} StatusCode  : {1} ({2})", GetType().Name, (ushort)StatusCode, StatusCode);
      Logger.LogDebug(false, "{0:l} ValueType   : {1}", GetType().Name, ValueType);
      Logger.LogDebug(false, "{0:l} Length      : {1}", GetType().Name, Length);
      Logger.LogDebug(false, "{0:l} StatusString: {1:l}", GetType().Name, StatusString);
    }

    #region Properties

    public AmxDevice Device { get; set; }

    /// <summary>
    /// 16-bit bit field. 
    /// If port = 0 then status of device instead of port.
    /// </summary>
    public StatusType StatusCode { get; private set; }

    /// <summary>
    /// Unsigned 8-bit value.
    /// Type of Specifier StatusString.
    /// </summary>
    public byte ValueType { get; private set; }

    /// <summary>
    /// Unsigned 16-bit value.
    /// Number of characters in string (i.e.number of elements, this is not the number of bytes)
    /// </summary>
    public ushort Length { get; private set; }

    /// <summary>
    /// Length characters. (n or n* 2 bytes)
    /// </summary>
    public string StatusString { get; private set; }

    #endregion
  }
}