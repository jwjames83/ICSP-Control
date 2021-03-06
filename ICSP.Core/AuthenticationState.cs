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
    NotAuthenticated  /**/ = 0, // 0000 0000 0000 0000
    Authenticated     /**/ = 1, // 0000 0000 0000 0001
    DoEncrypt         /**/ = 2, // 0000 0000 0000 0010
    EncryptionModeRC4 /**/ = 4, // 0000 0000 0000 0100
  }
}
