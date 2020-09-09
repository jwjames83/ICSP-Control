using System;

namespace ICSP.WebProxy.Models
{
  public class ErrorViewModel
  {
    public int ErrorCode { get; set; }

    public string RequestId { get; set; }

    public bool ShowRequestId
    {
      get
      {
        return !string.IsNullOrEmpty(RequestId);
      }
    }
  }
}
