namespace ICSP.Core.IO
{
  /// <summary>
  /// FileTransfer Unused functions
  /// </summary>
  public enum FileTransferUnusedFunction
  {
    /// <summary>
    /// Negative Acknowledgement 
    /// </summary>
    Nak = 0x0001,

    /// <summary>
    /// Get Directory Information
    /// </summary>
    DirGet = 256,

    /// <summary>
    /// Directory Information, Response to GetDirectoryInfo
    /// </summary>
    DirHeader = 257,

    /// <summary>
    /// Directory Item, Response to GetDirectoryInfo
    /// </summary>
    DirEntry = 258,

    /// <summary>
    /// Delete Directory
    /// </summary>
    DirRemove = 259,

    /// <summary>
    /// Delete File
    /// </summary>
    FileRemove = 260,

    /// <summary>
    /// Create Directory
    /// </summary>
    DirCreate = 261,
  }
}
