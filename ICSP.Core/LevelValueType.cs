namespace ICSP.Core
{
  public enum LevelValueType : byte
  {
    /// <summary>
    /// 0:255 (System.Byte)
    /// </summary>
    Byte     /**/ = 0x10,

    /// <summary>
    ///  -128:127 (System.SByte)
    /// </summary>
    Char     /**/ = 0x11,

    /// <summary>
    /// 0:65535 (System.UInt16)
    /// </summary>
    Integer  /**/ = 0x20, // + WideChar

    /// <summary>
    /// 
    /// </summary>
    SInteger /**/ = 0x21,

    /// <summary>
    /// 
    /// </summary>
    ULong    /**/ = 0x40,

    /// <summary>
    /// -999999999:999999999 (System.Int32)
    /// </summary>
    Long     /**/ = 0x41,

    /// <summary>
    /// 
    /// </summary>
    Float    /**/ = 0x4F,

    /// <summary>
    /// 
    /// </summary>
    Double   /**/ = 0x8F
  }
}
