using System;

namespace ICSP.Core.IO
{
  public sealed class GetDirectoryInfoEventArgs : EventArgs
  {
    public GetDirectoryInfoEventArgs(string path)
    {
      Path = path;
    }

    public string Path { get; }
  }

  public sealed class DirectoryInfoEventArgs : EventArgs
  {
    public DirectoryInfoEventArgs(string fullPath)
    {
      FullPath = fullPath;
    }

    public string FullPath { get; }
  }

  public sealed class DirectoryItemEventArgs : EventArgs
  {
    public DirectoryItemEventArgs(string fileName)
    {
      FileName = fileName;
    }

    public string FileName { get; }
  }

  public sealed class DeleteFileEventArgs : EventArgs
  {
    public DeleteFileEventArgs(string fileName)
    {
      FileName = fileName;
    }

    public string FileName { get; }
  }

  public sealed class CreatDirectoryEventArgs : EventArgs
  {
    public CreatDirectoryEventArgs(string directory)
    {
      Directory = directory;
    }

    public string Directory { get; }
  }

  public sealed class TransferFilesInitializeEventArgs : EventArgs
  {
    public TransferFilesInitializeEventArgs(ushort fileCount)
    {
      FileCount = fileCount;
    }

    public ushort FileCount { get; }
  }

  public sealed class TransferSingleFileInfoEventArgs : EventArgs
  {
    public TransferSingleFileInfoEventArgs(int fileSize, string fileName)
    {
      FileSize = fileSize;

      FileName = fileName;
    }

    public int FileSize { get; }

    public string FileName { get; }
  }

  public sealed class TransferGetFileAccessTokenEventArgs : EventArgs
  {
    public TransferGetFileAccessTokenEventArgs(int size, string fileName)
    {
      Size = size;

      FileName = fileName;
    }

    public int Size { get; }

    public string FileName { get; }
  }

  public sealed class TransferFileDataEventArgs : EventArgs
  {
    public TransferFileDataEventArgs(ushort junkSize, string fileName)
    {
      JunkSize = junkSize;

      FileName = fileName;
    }

    public ushort JunkSize { get; }

    public string FileName { get; }
  }
}
