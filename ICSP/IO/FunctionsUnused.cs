namespace ICSP.IO
{
  /// <summary>
  ///  FileTransfer Unused functions
  /// </summary>
  public enum FunctionsUnused
  {
    /// <summary>
    /// Get Directory Information
    /// </summary>
    GetDirectoryInfo = 0x0100,

    /// <summary>
    /// Directory Information, Response to GetDirectoryInfo
    /// </summary>
    DirectoryInfo = 0x0101,

    /// <summary>
    /// Directory Item, Response to GetDirectoryInfo
    /// </summary>
    DirectoryItem = 0x0102,

    /// <summary>
    /// Delete File
    /// </summary>
    DeleteFile = 0x0104,

    /// <summary>
    /// Create Directory
    /// </summary>
    CreateDirectory = 0x0105,
  }
}