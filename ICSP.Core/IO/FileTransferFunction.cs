namespace ICSP.Core.IO
{
  /// <summary>
  /// The function to execute, such as receive, send, etc.
  /// Values 0 - 255 are predefined.
  /// All other values are based upon the FileType.
  /// </summary>
  public enum FileTransferFunction
  {
    /// <summary>
    /// Negative Acknowledgement 
    /// </summary>
    Nak = 0x0001,

    /// <summary>
    /// Acknowledgement
    /// </summary>
    Ack = 0x0002,

    /// <summary>
    /// Data transmission
    /// </summary>
    TransferFileData = 0x0003,

    /// <summary>
    /// Data transmission complete
    /// </summary>
    TransferFileDataComplete = 0x0004,

    /// <summary>
    /// Data transmission complete -> Ack
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
  }
}
