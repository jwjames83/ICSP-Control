using System;
using System.Globalization;

namespace ICSPControl.DevStuff
{
  public static class TimeConverter
  {
    public static DateTime? FromAppDateTimeString(string appDateTimeString)
    {
      try
      {
        if (appDateTimeString.Length == 8)
        {
          var lTime = DateTime.ParseExact(appDateTimeString, "yyyyMMdd", CultureInfo.InvariantCulture);
          
          if (lTime.Equals(DateTime.MaxValue))
            return null;

          return new DateTime?(lTime);
        }

        if (appDateTimeString.Length == 14)
        {
          var lTime = DateTime.ParseExact(appDateTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
          
          if (lTime.Equals(DateTime.MinValue))
            return null;

          return new DateTime?(lTime);
        }

        return null;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static DateTime? MergeToUniversalTime(DateTime? dtDate, DateTime? dtTime)
    {
      if (!dtDate.HasValue)
        return null;

      if (!dtTime.HasValue)
        return new DateTime?(TimeZone.CurrentTimeZone.ToUniversalTime(dtDate.Value));

      var lTime = new DateTime(dtDate.Value.Year, dtDate.Value.Month, dtDate.Value.Day, dtTime.Value.Hour, dtTime.Value.Minute, dtTime.Value.Second);

      return new DateTime?(TimeZone.CurrentTimeZone.ToUniversalTime(lTime));
    }

    public static double? MinToSec(double? min)
    {
      if (!min.HasValue)
        return null;

      double? lMin = min;

      if (!lMin.HasValue)
        return null;

      return new double?(lMin.GetValueOrDefault() * 60.0);
    }

    public static double? SecToMin(double? sec)
    {
      if (!sec.HasValue)
        return null;

      double? lSec = sec;

      if (!lSec.HasValue)
        return null;

      return new double?(lSec.GetValueOrDefault() / 60.0);
    }

    public static string ToCoscomDateTimeString(DateTime dt)
    {
      return dt.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }

    public static string ToCoscomDateTimeStringWithMiliseconds(DateTime dt)
    {
      return dt.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
    }

    public static string ToEuroDateTimeString(DateTime dt)
    {
      return dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public static string ToGermanDateString(DateTime dt)
    {
      return dt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
    }

    public static string ToGermanDateTimeString(DateTime dt)
    {
      return dt.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public static DateTime ToLocalTime(DateTime dt)
    {
      return TimeZone.CurrentTimeZone.ToLocalTime(dt);
    }

    public static DateTime? ToLocalTime(DateTime? dt)
    {
      if (!dt.HasValue)
        return null;

      return new DateTime?(TimeZone.CurrentTimeZone.ToLocalTime(dt.Value));
    }

    public static DateTime ToLocalTime(DateTime dt, TimeZone timezone)
    {
      return timezone.ToLocalTime(dt);
    }

    public static DateTime? ToLocalTime(DateTime? dt, TimeZone timezone)
    {
      if (!dt.HasValue)
        return null;

      return new DateTime?(timezone.ToLocalTime(dt.Value));
    }

    public static DateTime ToUniversalTime(DateTime dt)
    {
      return TimeZone.CurrentTimeZone.ToUniversalTime(dt);
    }

    public static DateTime? ToUniversalTime(DateTime? dt)
    {
      if (!dt.HasValue)
        return null;

      return new DateTime?(TimeZone.CurrentTimeZone.ToUniversalTime(dt.Value));
    }

    public static DateTime ToUniversalTime(DateTime dt, TimeZone timezone)
    {
      return timezone.ToUniversalTime(dt);
    }

    public static DateTime? ToUniversalTime(DateTime? dt, TimeZone timezone)
    {
      if (!dt.HasValue)
        return null;

      return new DateTime?(timezone.ToUniversalTime(dt.Value));
    }
  }
}
