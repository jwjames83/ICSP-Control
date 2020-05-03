namespace ICSP
{
  public enum FileTransferFunction
  {
    /// <summary>
    /// Ack
    /// </summary>
    Ack = 0x0001,

    /// <summary>
    /// Get Panel Manifest
    /// </summary>
    GetPanelHierarchy = 0x0100,

    /// <summary>
    /// Get Panel Manifest
    /// </summary>
    GetPanelManifest = 0x0104,

    /// <summary>
    /// Create Remote Panel Directories
    /// </summary>
    CreateRemotePanelDirectories = 0x0105,

    /// <summary>
    /// GetPanelManifestUnknown
    /// </summary>
    GetPanelManifestUnknown = 0x0106,
    
  }
}
