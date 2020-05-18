using System;

namespace ICSPControl.Extensions
{
  public interface IIntervalData : IDateTimeInterval
  {
    DateTime BeginTsUtc { get; set; }
    DateTime? EndTsUtc { get; set; }
  }
}
