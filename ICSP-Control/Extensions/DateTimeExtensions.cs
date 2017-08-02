using System;
using ICSPControl.DevStuff;

namespace ICSPControl.Extensions
{
  public static class DateTimeExtensions
  {
    public static DateTime ChangeKind(this DateTime dt, DateTimeKind kind)
    {
      return DateTime.SpecifyKind(dt, kind);
    }

    public static DateTime ToLocalTime(this DateTime dt, TimeZone timeZone)
    {
      return TimeConverter.ToLocalTime(dt, timeZone);
    }

    public static DateTime ToRoundedTime(this DateTime dt)
    {
      return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, (dt.Millisecond / 10) * 10, dt.Kind);
    }
  }
}
