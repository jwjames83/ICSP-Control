namespace ICSP.Core.IO
{
  public enum FileTransferErrorCode : ushort
  {
    Unknown                       /**/ = 00, // No Error, Success
    NoMemory                      /**/ = 01, // Insufficient memory to complete the transfer
    BusyDownloading               /**/ = 02, // Busy downloading
    BusyUploading                 /**/ = 03, // Busy uploading
    UnsupportedType               /**/ = 04, // Unsupported file type
    OpenError                     /**/ = 05, // Error opening file
    WriteError                    /**/ = 06, // Write Error, transfer aborted
    Timeout                       /**/ = 07, // Timeout, transfer aborted
    ReadError                     /**/ = 08, // Read Error, transfer aborted
    InvalidID                     /**/ = 09, // Firmware ID is invalid
    CancelAbortTransferInProgress /**/ = 10, // Cancel, abort the transfer in progress
    IncompatibleVersion           /**/ = 11, // Incompatible file version
    ErrorCreatingDirectory        /**/ = 12, // Error creating directory
    ErrorRemovingDirectory        /**/ = 13, // Error removing directory
    ErrorRemovingFile             /**/ = 14, // Error removing file
    InvalidPath                   /**/ = 15, // Invalid path specification
    DirectoryAlreadyExists        /**/ = 16, // Directory already exists
    RXRespChoke                   /**/ = 17, // Insufficient access rights
    TXDataChoke                   /**/ = 18, // 
    TXDataNAK                     /**/ = 19, // 
  }
}
