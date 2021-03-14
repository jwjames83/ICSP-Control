using System;

namespace ICSP.Core
{
  public enum EncryptionType
  {
    /// <summary>
    /// This value does not work, if the option [Encrypt ICSP connection] is enabled on the controller.
    /// </summary>
    None = 0,

    /// <summary>
    /// Arcfour stream cipher (Master -> Device)
    /// </summary>
    RC4_ReceiveOnly = 1,

    /// <summary>
    /// Arcfour stream cipher (Master -> Device, Device -> Master)
    /// </summary>
    RC4_Both = 2,
  }
}
