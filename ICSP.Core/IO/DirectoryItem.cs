using System;
using System.Linq;

namespace ICSP.Core.IO
{
  public struct DirectoryItem
  {
    public string Name { get; set; }

    public DirectoryItemType Type { get; set; }

    public int FileSize { get; set; }

    public DateTime DateTime { get; set; }

    public byte[] GetDateTime()
    {
      return new byte[] { (byte)DateTime.Month, (byte)DateTime.Day, }
        .Concat(AmxUtils.Int16ToBigEndian((ushort)DateTime.Year))
        .Concat(new byte[] { (byte)DateTime.Hour, (byte)DateTime.Minute, (byte)DateTime.Second }).ToArray();
    }

    public override string ToString()
    {
      return string.Format("Name={0}, Type={1}", Name, Type); 
    }
  }
}
