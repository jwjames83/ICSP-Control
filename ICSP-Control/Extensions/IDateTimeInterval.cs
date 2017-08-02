using System;

namespace ICSPControl.Extensions
{
  public interface IDateTimeInterval
  {
    DateTime BeginTs { get; set; }
    DateTime EndTs { get; set; }
    TimeSpan TimeSpan { get; }
  }
}
