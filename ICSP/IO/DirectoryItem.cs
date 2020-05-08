namespace ICSP.IO
{
  public struct DirectoryItem
  {
    public string Name { get; set; }

    public DirectoryItemType Type { get; set; }

    public override string ToString()
    {
      return string.Format("Name={0}, Type={1}", Name, Type); 
    }
  }
}
