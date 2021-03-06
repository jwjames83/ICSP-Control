using System.Security.Cryptography;

namespace ICSP.Core.Cryptography
{
  public sealed class RC4CryptoServiceProvider : RC4
  {
    /// <summary>
    /// Initializes a new instance of the RC4CryptoServiceProvider class.
    /// </summary>
    public RC4CryptoServiceProvider()
    {
      // Change the limits for the current implementation (5 - 256 bytes).
      LegalKeySizesValue = new KeySizes[] {
        new KeySizes(40, 2048, 0),
        new KeySizes(2048, 2048, 0),
      };
    }

    /// <summary>
    /// Initializes a new instance of the RC4CryptoServiceProvider class.
    /// </summary>
    public RC4CryptoServiceProvider(byte[] key)
    {
      // Change the limits for the current implementation (5 - 256 bytes).
      LegalKeySizesValue = new KeySizes[] {
        new KeySizes(40, 2048, 0),
        new KeySizes(2048, 2048, 0),
      };

      SetKey(key);
    }
  }
}
