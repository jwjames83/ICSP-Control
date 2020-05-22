namespace ICSP.Core.IO
{
  public enum FileTransferStatusCode : ushort
  {
    NoError                   /**/ = 0x0000, // No Error, Success
    InsufficientMemory        /**/ = 0x0001, // Insufficient memory to complete the transfer
    BusyDownloading           /**/ = 0x0002, // Busy downloading
    BusyUploading             /**/ = 0x0003, // Busy uploading
    UnsupportedFileType       /**/ = 0x0004, // Unsupported file type
    ErrorOpeningFile          /**/ = 0x0005, // Error opening file
    WriteError                /**/ = 0x0006, // Write Error, transfer aborted
    Timeout                   /**/ = 0x0007, // Timeout, transfer aborted
    ReadError                 /**/ = 0x0008, // Read Error, transfer aborted
    InvalidFirmwareID         /**/ = 0x0009, // Firmware ID is invalid
    Cancel                    /**/ = 0x000A, // Cancel, abort the transfer in progress
    IncompatibleFileVersion   /**/ = 0x000B, // Incompatible file version
    ErrorCreatingDirectory    /**/ = 0x000C, // Error creating directory
    ErrorRemovingDirectory    /**/ = 0x000D, // Error removing directory
    ErrorRemovingFile         /**/ = 0x000E, // Error removing file
    InvalidPathSpecification  /**/ = 0x000F, // Invalid path specification
    DirectoryAlreadyExists    /**/ = 0x0010, // Directory already exists
    InsufficientAccessRights  /**/ = 0x0011, // Insufficient access rights
  }
}
