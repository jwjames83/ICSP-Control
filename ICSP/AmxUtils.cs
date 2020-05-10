using System;
using System.Text;

namespace ICSP
{
  public static class AmxUtils
  {

    public static byte[] Int32ToBigEndian(int value)
    {
      return new byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
    }

    public static byte[] Int16ToBigEndian(ushort value)
    {
      return new byte[] { (byte)((ushort)value >> 8), (byte)value };
    }

    public static byte[] Int16To8Bit(int value)
    {
      return new byte[] { (byte)value };
    }

    public static string GetString(byte[] bytes, int length)
    {
      return GetString(bytes, 0, length);
    }

    public static string GetString(byte[] bytes, int startIndex, int length)
    {
      if(bytes == null)
        throw new ArgumentNullException(nameof(bytes));
      
      return Encoding.GetEncoding(1252).GetString(bytes, startIndex, length).TrimEnd(new char[] { '\0', ' ' });
    }

    public static string GetUnicodeString(byte[] bytes, int startIndex, int length)
    {
      if(bytes == null)
        throw new ArgumentNullException(nameof(bytes));

      return Encoding.Unicode.GetString(bytes, startIndex, length).TrimEnd(new char[] { '\0', ' ' });
    }

    public static string GetNullStr(byte[] bytes, int offset)
    {
      return GetNullStr(bytes, ref offset);
    }
   
    public static string GetNullStr(byte[] bytes, ref int offset)
    {
      if(bytes == null)
        throw new ArgumentNullException(nameof(bytes));

      var lStr = string.Empty;

      // Search for 0
      var lIndex = Array.FindIndex(bytes, offset, x => x == 0);

      if(lIndex >= 0)
      {
        lIndex -= offset;

        lStr = Encoding.GetEncoding(1252).GetString(bytes, offset, lIndex);

        offset += lIndex + 1;
      }

      return lStr.TrimEnd(new char[] { '\0', ' ' });
    }
  }
}
