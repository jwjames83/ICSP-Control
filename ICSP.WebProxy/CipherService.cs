using Microsoft.AspNetCore.DataProtection;

namespace ICSP.WebProxy
{
  public class CipherService
  {
    private readonly IDataProtectionProvider mDataProtectionProvider;

    private readonly string Purpose = typeof(CipherService).FullName;

    public CipherService(IDataProtectionProvider dataProtectionProvider)
    {
      mDataProtectionProvider = dataProtectionProvider;
    }

    public string Encrypt(string input)
    {
      var lProtector = mDataProtectionProvider.CreateProtector(Purpose);

      return lProtector.Protect(input);
    }

    public string Decrypt(string cipherText)
    {
      var lProtector = mDataProtectionProvider.CreateProtector(Purpose);

      return lProtector.Unprotect(cipherText);
    }
  }
}
