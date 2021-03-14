using System;

namespace ICSP.Core
{
  /*
  AuthenticatedBlinkOff = 3;
  AuthenticatedBlinkOn  = 4;  
  EncryptedBlinkOff     = 5;  
  EncryptedBlinkOn      = 6;  
  AuthenticationFailed  = 7;  
  AccessNotAllowed      = 8;
  */

  [Flags]
  public enum AuthenticationState
  {
    NotAuthenticated      /**/ = 0, // 0000 0000 0000 0000
    Authenticated         /**/ = 1, // 0000 0000 0000 0001

    EncryptionRC4_Receive /**/ = 2, // 0000 0000 0000 0010
    EncryptionRC4_Send    /**/ = 4, // 0000 0000 0000 0100

    NotAllowed = 0x8000,
  }
}
