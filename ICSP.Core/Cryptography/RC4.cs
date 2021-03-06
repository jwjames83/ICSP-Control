using System;
using System.Security.Cryptography;

using ICSP.Core.Extensions;

namespace ICSP.Core.Cryptography
{
  internal interface ICrypto
  {
    void SetKey(byte[] key);

    void TransformBlock(byte[] key, int offset, int count, byte[] cipher);
  }

  /// <summary>
  /// Represents the base class from which all implementations of the RC4 algorithm must derive.
  /// </summary>
  /// <see cref="https://docs.microsoft.com/en-gb/dotnet/api/system.security.cryptography.symmetricalgorithm?view=netstandard-2.0"/>
  /// <seealso cref="https://en.wikipedia.org/wiki/RC4"/>
  public abstract class RC4 : SymmetricAlgorithm, ICrypto
  {
    private int mI = 0;
    private int mJ = 0;

    private int mXSave = 0;
    private int mYSave = 0;

    private byte[] mPrivateKey = new byte[256];
    private byte[] mSave = new byte[256];

    private void SaveCipher()
    {
      Array.Copy(mPrivateKey, 0, mSave, 0, 256);

      mXSave = mI;
      mYSave = mJ;
    }

    private void RestoreCipher()
    {
      Array.Copy(mSave, 0, mPrivateKey, 0, 256);

      mI = mXSave;
      mJ = mYSave;

      KeyValue = mPrivateKey;
    }

    private void UpdateCipher(byte[] cipher)
    {
      if(cipher != null)
        Array.Copy(cipher, 0, mPrivateKey, 0, cipher.Length);

      KeyValue = mPrivateKey;
    }

    // Key-Scheduling Algorithm.
    public void SetKey(byte[] rgbKey)
    {
      var lKey = new byte[rgbKey.Length];

      rgbKey.CopyTo(lKey, 0);

      for(int i = 0; i < 256; i++)
        mPrivateKey[i] = (byte)i;

      for(int i = 0, j = 0; i < 256; i++)
      {
        j = (j + mPrivateKey[i] + lKey[i % lKey.Length]) % 256;

        mPrivateKey.Swap(i, j);
      }

      // Save key
      Array.Copy(mPrivateKey, 0, mSave, 0, 256);
      Array.Copy(mPrivateKey, 0, KeyValue, 0, 256);
    }

    /// <summary>
    /// The first index-pointers.
    /// </summary>
    private int i = 0;

    /// <summary>
    /// The second index-pointers.
    /// </summary>
    private int j = 0;

    /// <summary>
    /// Number of characters per byte.
    /// </summary>
    private const int CharsInByte = byte.MaxValue + 1;

    /// <summary>
    /// Pseudo-Random Generation Algorithm.
    /// </summary>
    /// <returns>Pseudo-Random Word (The keystream value K)</returns>
    private byte PRGA()
    {
      unchecked
      {
        i = (i + 1) % CharsInByte;
        j = (j + mPrivateKey[i]) % CharsInByte;

        mPrivateKey.Swap(i, j);

        return mPrivateKey[(mPrivateKey[i] + mPrivateKey[j]) % CharsInByte];
      }
    }

    /// <summary>
    /// Pseudo-Random Generation Algorithm.
    /// </summary>
    /// <param name="cipher">Array of bytes of ciphertext or plaintext.</param>
    private void PRGA(byte[] cipher)
    {
      unchecked
      {
        for(int k = 0; k < cipher.Length; k++)
          cipher[k] = (byte)(cipher[k] ^ PRGA());
      }
    }

    public void TransformBlockDefault(byte[] data, byte[] salt)
    {
      i = 0;
      j = 0;

      if(salt != null)
        UpdateCipher(salt);

      PRGA(data);

      if(salt != null)
        RestoreCipher();
    }

    public void TransformBlock(byte[] inputBuffer, int offset, int count, byte[] salt)
    {
      if(salt != null)
        UpdateCipher(salt);

      for(int i = offset; i < offset + count; i++)
      {
        mI = mI + 1 & 0xFF; // mod 256

        int j = mPrivateKey[mI] & 0xFF;

        mJ = mJ + j & 0xFF;

        // Swap values
        mPrivateKey[mI] = mPrivateKey[mJ];

        byte b = mPrivateKey[mJ];

        mPrivateKey[mJ] = (byte)(j & 0xFF);

        inputBuffer[i] = (byte)((inputBuffer[i] ^ mPrivateKey[j + b & 0xFF]) & 0xFF);
      }

      if(salt != null)
        RestoreCipher();
    }

    #region Transform

    /// <summary>
    /// Performs a cryptographic transformation of data using the RC4 algorithm. This class cannot be inherited.
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-gb/dotnet/api/system.security.cryptography.icryptotransform?view=netstandard-2.0"/>
    private sealed class Transform : ICryptoTransform
    {
      /// <summary>
      /// Initializes a new instance of the RC4Transform class by using secret key.
      /// </summary>
      /// <param name="rgbKey">The secret key to use for the algorithm.</param>
      /// <remarks>The block size is equal to the size of the S-Box table.</remarks>
      internal Transform(byte[] rgbKey)
      {
        S = new byte[CharsInByte];

        KSA(rgbKey);
      }

      /// <summary>
      /// An array used as a replacement table, called an S-box.
      /// </summary>
      private readonly byte[] S;

      /// <summary>
      /// The first index-pointers.
      /// </summary>
      private int i = 0;

      /// <summary>
      /// The second index-pointers.
      /// </summary>
      private int j = 0;

      /// <summary>
      /// Number of characters per byte.
      /// </summary>
      private const int CharsInByte = byte.MaxValue + 1;

      /// <summary>
      /// Key-Scheduling Algorithm.
      /// </summary>
      /// <param name="rgbKey">The secret key to use for the algorithm.</param>
      private void KSA(byte[] rgbKey)
      {
        var key = new byte[rgbKey.Length];

        rgbKey.CopyTo(key, 0);

        for(int i = 0; i < CharsInByte; i++)
          S[i] = (byte)i;

        for(int i = 0, j = 0; i < CharsInByte; i++)
        {
          j = (j + S[i] + key[i % key.Length]) % CharsInByte;

          S.Swap(i, j);
        }
      }

      /// <summary>
      /// Pseudo-Random Generation Algorithm.
      /// </summary>
      /// <returns>Pseudo-Random Word (The keystream value K)</returns>
      private byte PRGA()
      {
        unchecked
        {
          i = (i + 1) % CharsInByte;
          j = (j + S[i]) % CharsInByte;

          S.Swap(i, j);

          return S[(S[i] + S[j]) % CharsInByte];
        }
      }

      /// <summary>
      /// Pseudo-Random Generation Algorithm.
      /// </summary>
      /// <param name="cipher">Array of bytes of ciphertext or plaintext.</param>
      private void PRGA(byte[] cipher)
      {
        unchecked
        {
          for(int k = 0; k < cipher.Length; k++)
            cipher[k] = (byte)(cipher[k] ^ PRGA());
        }
      }

      #region ICryptoTransform

      /// <summary>
      /// Gets a value indicating whether the current transform can be reused.
      /// </summary>
      public bool CanReuseTransform { get { return false; } }

      /// <summary>
      /// Gets a value indicating whether multiple blocks can be transformed.
      /// </summary>
      public bool CanTransformMultipleBlocks { get { return true; } }

      /// <summary>
      /// Gets the input block size.
      /// </summary>
      public int InputBlockSize { get { return CharsInByte; } }

      /// <summary>
      /// Gets the output block size.
      /// </summary>
      public int OutputBlockSize { get { return CharsInByte; } }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      { }

      /// <summary>
      /// Transforms the specified region of the input byte array and copies the resulting transform to the specified region of the output byte array.
      /// </summary>
      /// <param name="inputBuffer">The input for which to compute the transform.</param>
      /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
      /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
      /// <param name="outputBuffer">The output to which to write the transform.</param>
      /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
      /// <returns>The number of bytes written.</returns>
      public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
      {
        if(inputBuffer == null)
          throw new ArgumentNullException(nameof(inputBuffer));

        if(outputBuffer == null)
          throw new ArgumentNullException(nameof(inputBuffer));

        if(inputOffset < 0)
          throw new ArgumentOutOfRangeException(nameof(inputOffset));

        if(inputCount < 0 || inputCount > inputBuffer.Length)
          throw new ArgumentException("Value was invalid.");

        if(inputBuffer.Length - inputCount < inputOffset)
          throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

        var cipher = new byte[inputCount];

        Array.Copy(inputBuffer, inputOffset, cipher, 0, inputCount);

        PRGA(cipher);

        cipher.CopyTo(outputBuffer, outputOffset);

        return cipher.Length;
      }

      /// <summary>
      /// Transforms the specified region of the specified byte array.
      /// </summary>
      /// <param name="inputBuffer">The input for which to compute the transform.</param>
      /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
      /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
      /// <returns>The computed transform</returns>
      public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
      {
        if(inputBuffer == null)
          throw new ArgumentNullException(nameof(inputBuffer));

        if(inputOffset < 0)
          throw new ArgumentOutOfRangeException(nameof(inputOffset));

        if(inputCount < 0 || inputCount > inputBuffer.Length)
          throw new ArgumentException("Value was invalid.");

        if(inputBuffer.Length - inputCount < inputOffset)
          throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

        var cipher = new byte[inputCount];

        Array.Copy(inputBuffer, inputOffset, cipher, 0, inputCount);

        PRGA(cipher);

        return cipher;
      }

      #endregion ICryptoTransform
    }

    #endregion Transform

    #region RC4

    /// <summary>
    /// Initializes a new instance of the RC4 class.
    /// </summary>
    internal RC4()
    {
      // Recommended length for use in the US is 128 bits.
      KeySizeValue = 128;

      // Usually, it is 256 bits. However, to increase safety, it is necessary to increase this value.
      BlockSizeValue = 256;

      // Response size cannot be larger than block size.
      FeedbackSizeValue = BlockSizeValue;

      // The algorithm has no max limit on the size of the S-box. Limitations only in performance and x86/x64 system.
      LegalBlockSizesValue = new KeySizes[] { new KeySizes(256, 256, 0) };

      // The key length can be 8-2048 bits (1 - 256 bytes).
      LegalKeySizesValue = new KeySizes[] { new KeySizes(8, 2048, 0) };

      // The CBC mode may also be considered a stream cipher with n-bit blocks playing the role of very large characters.
      ModeValue = CipherMode.CBC;

      PaddingValue = PaddingMode.None;

      KeyValue = new byte[256];
    }

    /// <summary>
    /// Creates an instance of a cryptographic object to perform the RC4 algorithm.
    /// </summary>
    /// <returns>An instance of a cryptographic object.</returns>
    new static public RC4 Create()
    {
      return new RC4CryptoServiceProvider();
    }

    static public RC4 Create(byte[] key)
    {
      return new RC4CryptoServiceProvider(key);
    }

    #endregion RC4

    #region SymmetricAlgorithm

    /// <summary>
    /// Gets the feedback size, in bits, of the cryptographic operation.
    /// </summary>
    /// <remarks>Response size is always equal to block size, unlike the base class.</remarks>
    public override int FeedbackSize { get { return BlockSize; } }

    /// <summary>
    /// Gets the initialization vector (IV) for the symmetric algorithm.
    /// </summary>
    /// <remarks>Initialization vector is not used in RC4.</remarks>
    public override byte[] IV { get { return null; } }

    /// <summary>
    /// Creates a symmetric encryptor object with the current Key property and initialization vector (IV).
    /// </summary>
    /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
    /// <param name="rgbIV">The initialization vector to use for the symmetric algorithm (not used in RC4).</param>
    /// <returns>A symmetric encryptor object.</returns>
    /// <remarks>Use the CreateDecryptor overload with the same parameters to decrypt the result of this method.</remarks>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
    {
      // setKey(rgbKey, rgbKey.Length);

      // return new Transform(m_mSave);

      return new Transform(rgbKey);
    }

    /// <summary>
    /// Creates a symmetric decryptor object with the current Key property and initialization vector (IV).
    /// </summary>
    /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
    /// <param name="rgbIV">The initialization vector to use for the symmetric algorithm (not used in RC4).</param>
    /// <returns>A symmetric decryptor object.</returns>
    /// <remarks>This method decrypts an encrypted message created using the CreateEncryptor overload with the same parameters</remarks>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
    {
      return CreateEncryptor(rgbKey, rgbIV);
    }

    /// <summary>
    /// Creates a symmetric encryptor object.
    /// </summary>
    /// <returns>A symmetric encryptor object.</returns>
    /// <remarks>
    /// If the current Key property is null, the GenerateKey method is called to create a new random Key.
    /// Use the CreateDecryptor overload with the same signature to decrypt the result of this method.
    /// </remarks>
    public override ICryptoTransform CreateEncryptor()
    {
      if(KeyValue == null)
        GenerateKey();

      return new Transform(KeyValue);
    }

    /// <summary>
    /// Creates a symmetric decryptor object.
    /// </summary>
    /// <returns>A symmetric decryptor object.</returns>
    /// <remarks>This method decrypts an encrypted message created using the CreateEncryptor overload with the same signature.</remarks>
    public override ICryptoTransform CreateDecryptor()
    {
      return new Transform(KeyValue);
    }

    /// <summary>
    /// Generates a random key (Key) to use for the algorithm.
    /// </summary>
    public override void GenerateKey()
    {
      // Recommended length for use in the US is 128 bits.
      if(KeySizeValue == 0)
        KeySizeValue = 128;

      KeyValue = new byte[KeySizeValue / 8];

      using(var random = RandomNumberGenerator.Create())
      {
        random.GetBytes(KeyValue);
      }
    }

    /// <summary>
    /// Generates a random initialization vector (IV) to use for the algorithm.
    /// </summary>
    /// <remarks>In general, there is no reason to use this method, because it is not used in RC4 stream cipher.</remarks>
    public override void GenerateIV()
    {
      // Initialization vector is not used in RC4.
    }

    #endregion SymmetricAlgorithm
  }
}
