namespace ICSP.IO
{
  public enum FileTransferFunction
  {
    /// <summary>
    /// Negative acknowledgment 
    /// </summary>
    Nak = 0x0001,

    /// <summary>
    /// Acknowledgment
    /// </summary>
    Ack = 0x0002,

    // ================================================================
    // FileType: Unused
    // ================================================================

    /// <summary>
    /// Get Directory Information
    /// </summary>
    UnusedGetDirectoryInfo = 0x0100,

    /// <summary>
    /// Directory Information
    /// </summary>
    UnusedDirectoryInfo = 0x0101,

    /// <summary>
    /// Directory Item
    /// </summary>
    UnusedDirectoryItem = 0x0102,

    /// <summary>
    /// Delete File
    /// </summary>
    UnusedDeleteFile = 0x0104,

    /// <summary>
    /// Create Directory
    /// </summary>
    UnusedCreateDirectory = 0x0105,

    // ================================================================
    // FileType: Axcess 2 Tokens
    // ================================================================

    /// <summary>
    /// Request for further data
    /// </summary>
    TransferFileDataGetNext = 0x0002,

    /// <summary>
    /// Data transmission
    /// </summary>
    TransferFileData = 0x0003,

    /// <summary>
    /// Data transmission complete
    /// </summary>
    TransferFileDataComplete = 0x0004,

    /// <summary>
    /// Data transmission complete -> Acknowledgment
    /// </summary>
    TransferFileDataCompleteAck = 0x0005,

    /// <summary>
    /// Starting to transfer multiple files
    /// </summary>
    TransferFilesInitialize = 0x0006,

    /// <summary>
    /// End of transfer multiple files
    /// </summary>
    TransferFilesComplete = 0x0007,

    /// <summary>
    /// Starting the transfer of a file
    /// </summary>
    TransferSingleFile = 0x0100,

    /// <summary>
    /// Starting the transfer of a file -> Acknowledgment
    /// </summary>
    TransferSingleFileAck = 0x0101,

    /// <summary>
    /// File Information 
    /// </summary>
    TransferSingleFileInfo = 0x0102,

    /// <summary>
    /// File Information -> Acknowledgment
    /// </summary>
    TransferSingleFileInfoAck = 0x0103,

    /// <summary>
    /// Get Access Token for Get File
    /// </summary>
    TransferGetFileAccessToken = 0x0104,

    /// <summary>
    /// Get File Data
    /// </summary>
    TransferGetFile = 0x0106,
  }
}
