using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP
{
  public struct ICSPMsgData
  {
    public const int DefaultFlag = 0x0200;

    public static readonly ICSPMsgData Empty;
    
    public static ICSPMsgData FromMessage(byte[] msg)
    {
      var lMsg = new ICSPMsgData();

      lMsg.RawData = msg;

      if(msg.Length >= 3)
      {
        lMsg.Protocol = msg[0];

        lMsg.Length = msg.GetBigEndianInt16(1);

        // Protocol (1), Length (2), Checksum (1)
        if(msg.Length < lMsg.Length + 4)
        {
          Logger.LogWarn("{0} => Data to Short: {1} Bytes", nameof(ICSPMsgData),  msg.Length);

          return ICSPMsgData.Empty;
        }

        lMsg.Flag = msg.GetBigEndianInt16(3);

        lMsg.Dest = AmxDevice.FromSDP(msg.Range(5, 6));
        lMsg.Source = AmxDevice.FromSDP(msg.Range(11, 6));

        lMsg.Hop = msg[17];

        lMsg.ID = msg.GetBigEndianInt16(18);

        lMsg.Command = msg.GetBigEndianInt16(20);

        // Data
        lMsg.Data = msg.Range(22, msg.Length - 22 - 1);

        lMsg.Checksum = msg[lMsg.Length + 3];
      }

      return lMsg;
    }

    public byte[] RawData { get; set; }

    /// <summary>
    /// Protocol
    /// </summary>
    public byte Protocol { get; set; }

    /// <summary>
    /// Length
    /// </summary>
    public ushort Length { get; set; }

    /// <summary>
    /// Flag (Version, Type)
    /// </summary>
    public ushort Flag { get; set; }

    /// <summary>
    /// Destination
    /// </summary>
    public AmxDevice Dest { get; set; }

    /// <summary>
    /// Source
    /// </summary>
    public AmxDevice Source { get; set; }

    /// <summary>
    /// Allowed Hop count
    /// </summary>
    public byte Hop { get; set; }

    /// <summary>
    /// Message ID
    /// </summary>
    public ushort ID { get; set; }

    /// <summary>
    /// Message Command
    /// </summary>
    public ushort Command { get; set; }
    
    /// <summary>
    /// Message Data
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Checksum (Sum of Bytes % 256)
    /// </summary>
    public byte Checksum { get; set; }
  }
}
