namespace ICSP
{
  public enum FileType : byte
  {
    /// <summary>
    /// Unused
    /// </summary>
    Unused = 0,

    /// <summary>
    /// IRData
    /// </summary>
    IRData = 1,

    /// <summary>
    /// Firmware
    /// </summary>
    Firmware = 2,

    /// <summary>
    /// TouchPanel File
    /// </summary>
    TouchPanelFile = 3,

    /// <summary>
    /// Axcess2 Tokens
    /// </summary>
    Axcess2Tokens = 4,
  }
}
