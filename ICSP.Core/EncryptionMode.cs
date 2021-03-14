using System;

namespace ICSP.Core
{
  [Flags]
  public enum EncryptionMode
  {
    None        /**/ = 0,

    RC4_Receive /**/ = 2, // 0010

    RC4_Send    /**/ = 4, // 0100
  }
}
