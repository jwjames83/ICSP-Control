namespace ICSP.IO
{
  /// <summary>
  /// FileTransfer Axcess 2 Tokens Functions
  /// </summary>
  public enum FunctionsAxcess2Tokens
  {
    /// <summary>
    /// Starting the transfer of a file
    /// </summary>
    TransferSingleFile = 0x0100,

    /// <summary>
    /// Starting the transfer of a file -> Ack
    /// </summary>
    TransferSingleFileAck = 0x0101,

    /// <summary>
    /// File Information 
    /// </summary>
    TransferSingleFileInfo = 0x0102,

    /// <summary>
    /// File Information -> Ack
    /// </summary>
    TransferSingleFileInfoAck = 0x0103,

    /// <summary>
    /// Get Access Token for Get File
    /// </summary>
    TransferGetFileAccessToken = 0x0104,

    /// <summary>
    /// Get Access Token for Get File -> Ack
    /// </summary>
    TransferGetFileAccessTokenAck = 0x0105,

    /// <summary>
    /// Get File Data
    /// </summary>
    TransferGetFile = 0x0106,
  }
}