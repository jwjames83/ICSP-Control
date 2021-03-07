using System;

namespace ICSP.Core
{
  public class ExtendedAddress
  {
    public const int MaxExtAddressLen = 30;

    public ExtendedAddress()
    {
    }

    public ExtendedAddress(byte[] extAddress)
    {
      Array.Copy(extAddress, 0, this.ExtAddress, 0, ExtAddressLength);

      // Fill with 0
      for(int i = ExtAddressLength; i < MaxExtAddressLen; i++)
        this.ExtAddress[i] = 0;
    }

    /// <summary>
    /// 8-bit value. Used to indicate type of extended address to follow.
    /// </summary>
    public ExtAddressType ExtAddressType { get; private set; }

    /// <summary>
    /// 8-bit value. Used to indicate length in bytes of extended address to follow
    /// </summary>
    public byte ExtAddressLength { get; private set; }

    /// <summary>
    /// Extended Address as indicated by Address Type and Length.
    /// </summary>
    public byte[] ExtAddress { get; private set; }
  }
}