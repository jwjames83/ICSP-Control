using System;
using System.Linq;

using ICSP.Constants;
using ICSP.Extensions;
using ICSP.IO;
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

    public MsgCmdFileTransfer(byte[] buffer) : base(buffer)
    {
      if(Data.Length > 0)
      {
        // FileType
        FileType = (FileType)Data.GetBigEndianInt16(0);

        // Function
        Function = Data.GetBigEndianInt16(2);

        FileData = Data.Range(4, Data.Length - 4);
      }
    }

    public override ICSPMsg FromData(byte[] bytes)
    {
      return new MsgCmdFileTransfer(bytes);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, FileType fileType, ushort function, byte[] data = null)
    {
      var lRequest = new MsgCmdFileTransfer
      {
        FileType = fileType,
        Function = function,
        FileData = data
      };

      var lData =
        ArrayExtensions.Int16ToBigEndian((ushort)fileType)
        .Concat(ArrayExtensions.Int16ToBigEndian(function))
        .Concat(data ?? Array.Empty<byte>()).ToArray();

      return lRequest.Serialize(dest, source, MsgCmd, lData);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, FileType fileType, FileTransferFunction function, byte[] data = null)
    {
      return CreateRequest(dest, source, fileType, (ushort)function, data);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, FileType fileType, FunctionsUnused function, byte[] data = null)
    {
      return CreateRequest(dest, source, fileType, (ushort)function, data);
    }

    public static ICSPMsg CreateRequest(AmxDevice dest, AmxDevice source, FileType fileType, FunctionsAxcess2Tokens function, byte[] data = null)
    {
      return CreateRequest(dest, source, fileType, (ushort)function, data);
    }

    internal static ICSPMsg CreateErrorRequest(AmxDevice dest, AmxDevice source, FileType fileType, FileTransferStatusCode statusCode)
    {
      return CreateRequest(dest, source, fileType, (ushort)FileTransferFunction.Nak, ArrayExtensions.Int16ToBigEndian((ushort)statusCode));
    }

    protected override void WriteLogExtended()
    {
      Logger.LogDebug(false, "{0:l} FileType     : 0x{1:X4} ({2})", GetType().Name, (byte)FileType, FileType);

      var lFunction = "Unknown";

      if(Function <= 255)
      {
        lFunction = ((FileTransferFunction)Function).ToString();
      }
      else
      {
        switch(FileType)
        {
          case FileType.Unused: lFunction = ((FunctionsUnused)Function).ToString(); break;
          case FileType.IRData: break;
          case FileType.Firmware: break;
          case FileType.TouchPanelFile: break;
          case FileType.Axcess2Tokens: lFunction = ((FunctionsAxcess2Tokens)Function).ToString(); break;
        }
      }

      Logger.LogDebug(false, "{0:l} Function     : 0x{1:X4} ({2:l})", GetType().Name, Function, lFunction);
      Logger.LogDebug(false, "{0:l} FileData (0x): {1:l}", GetType().Name, BitConverter.ToString(FileData).Replace("-", " "));
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
    public ushort Function { get; private set; }

    /// <summary>
    /// It any, contains Function specific data.
    /// </summary>
    public byte[] FileData { get; private set; }

    #endregion
  }
}
