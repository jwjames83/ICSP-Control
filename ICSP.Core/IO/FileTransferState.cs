namespace ICSP.Core.IO
{
  /// <summary>
  /// FileTransfer state
  /// </summary>
  public enum FileTransferState
  {
    Idle      /**/ = 0,
    RxData    /**/ = 1,
    TxResp    /**/ = 2,
    TxWaitAck /**/ = 3,
    TxData    /**/ = 4,
  }
}