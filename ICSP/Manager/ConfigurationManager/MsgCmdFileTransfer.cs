using System;
using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.Logging;

namespace ICSP.Manager.ConfigurationManager
{
  /// <summary>
  /// This command initiates the transfer of data for a device or master.
  /// It is intended that this transfer mechanism becommon to all types 
  /// of file transfers including firmware upgrades, IR data, touch panel design files.
  /// </summary>
  [MsgCmd(ConfigurationManagerCmd.FileTransfer)]
  public class MsgCmdFileTransfer : ICSPMsg
  {
    public const int MsgCmd = ConfigurationManagerCmd.FileTransfer;

    public const int FuncCreateRemotePanelDirectories = 0x0105;

    private MsgCmdFileTransfer()
    {
    }

    public MsgCmdFileTransfer(ICSPMsgData msg) : base(msg)
    {
      if(msg.Data.Length > 0)
      {
        // FileType
        FileType = (FileType)msg.Data.GetBigEndianInt16(0);

        // Function
        Function = (FileTransferFunction)msg.Data.GetBigEndianInt16(2);

        FileData = msg.Data.Range(4, msg.Data.Length - 4);
      }
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, FileType fileType, ushort function, byte[] fileData)
    {
      var lRequest = new MsgCmdFileTransfer();

      var lData =
        ArrayExtensions.Int16ToBigEndian((ushort)fileType)
        .Concat(ArrayExtensions.Int16ToBigEndian(function))
        .Concat(fileData ?? new byte[] { }).ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }    

    protected override void WriteLogExtended()
    {
      var lOffset = 0;

      Logger.LogDebug(false, "{0} FileType     : 0x{1:X4} ({2})", GetType().Name, (byte)FileType, FileType);
      Logger.LogDebug(false, "{0} Function     : 0x{1:X4} ({2})", GetType().Name, (ushort)Function, Function);

      if(FileType == FileType.Unused)
        Logger.LogDebug(false, "{0} FileData     : {1}", GetType().Name, AmxUtils.GetNullStr(FileData, ref lOffset));

      Logger.LogDebug(false, "{0} FileData (0x): {1}", GetType().Name, BitConverter.ToString(FileData).Replace("-", " "));
    }

    #region Properties

    /// <summary>
    /// FileType:
    /// O = Unused
    /// 1 = IRData
    /// 2 = Firmware
    /// 3 = TouchPanel File
    /// 4 = Axcess2 Tokens
    /// </summary>
    public FileType FileType { get; private set; }

    /// <summary>
    /// The function to execute, such as receive, send, etc.
    /// Values 0 - 255 are predefined.
    /// All other values are based upon the FileType.
    /// </summary>
    public FileTransferFunction Function { get; private set; }

    /// <summary>
    /// It any, contains Function specific data.
    /// </summary>
    public byte[] FileData { get; private set; }

    #endregion
  }
}
