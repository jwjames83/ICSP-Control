using System;

namespace ICSP.Extensions
{
  public static class ArrayExtensions
  {
    public static T[] Range<T>(this T[] data, int startIndex, int length)
    {
      T[] result = new T[length];

      Array.Copy(data, startIndex, result, 0, length);

      return result;
    }

    public static ushort GetBigEndianInt16(this byte[] data, int startIndex)
    {
      return (ushort)((data[startIndex] << 8)
           | data[startIndex + 1]);
    }

    public static int GetBigEndianInt32(this byte[] data, int startIndex)
    {
      return (data[startIndex] << 24)
          | (data[startIndex + 1] << 16)
          | (data[startIndex + 2] << 8)
          | data[startIndex + 3];
    }

    public static long GetBigEndianInt64(this byte[] data, int startIndex)
    {
      return (data[startIndex] << 56)
          | (data[startIndex + 1] << 48)
          | (data[startIndex + 2] << 40)
          | (data[startIndex + 3] << 32)
          | (data[startIndex + 4] << 24)
          | (data[startIndex + 5] << 16)
          | (data[startIndex + 6] << 8)
          | data[startIndex + 7];
    }

    public static byte[] Int32ToBigEndian(int value)
    {
      return new byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
    }

    public static byte[] Int16ToBigEndian(ushort value)
    {
      return new byte[] { (byte)(value >> 8), (byte)value };
    }

    public static byte[] Int16To8Bit(int value)
    {
      return new byte[] { (byte)value };
    }
  }
}
