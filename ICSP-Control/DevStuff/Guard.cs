using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using ICSPControl.Extensions;

namespace ICSPControl.DevStuff
{
  public static class Guard
  {
    public static void ArgumentNotNull(object value, string argumentName)
    {
      if (value == null)
        throw new ArgumentNullException(argumentName);
    }

    public static void ArgumentNotNull(object value, string argumentName, string message)
    {
      if (value == null)
        throw new ArgumentNullException(argumentName, message);
    }

    public static void ArgumentNotStringEmptyOrNull(string value, string argumentName)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException("String is null or empty.", argumentName);
    }

    public static void ArgumentStringIsFilled(string value, string argumentName)
    {
      if (!string.IsNullOrEmpty(value))
        throw new ArgumentException("String is filled", argumentName);
    }

    public static void CheckArgument(bool failingCondition, string message)
    {
      CheckArgument(failingCondition, string.Empty, message);
    }

    public static void CheckArgument(bool failingCondition, string argumentName, string message)
    {
      if (failingCondition)
        throw new ArgumentException(argumentName, message);
    }

    public static void CheckDate(DateTime dt, string argumentName)
    {
      if (dt != dt.Date)
        throw new ArgumentException("Incoming DateTime must only contain the Date-part of a DateTime.", argumentName);
    }

    public static void CheckIntervalListForGaps(IEnumerable<IIntervalData> intervalList)
    {
      CheckIntervalListForGaps(intervalList, string.Empty);
    }

    public static void CheckIntervalListForGaps(IEnumerable<IIntervalData> intervalList, string argumentName)
    {
      var lIntervals = (from c in intervalList
                        orderby c.BeginTsUtc
                        select c).ToList<IIntervalData>();

      for (int i = 0; i < (lIntervals.Count - 1); i++)
      {
        IIntervalData data = lIntervals[i];
        IIntervalData data2 = lIntervals[i + 1];

        if (data.EndTsUtc != data2.BeginTsUtc && data.EndTsUtc.HasValue && data.EndTsUtc.Value.Date == data2.BeginTsUtc.Date)
        {
          var lSb = new StringBuilder();
          
          intervalList.ForEach(a => lSb.AppendLine(string.Format("BeginTsUtc:{0:dd.MM.yyyy} {0:HH:mm:ss.fff} EndTsUtc:{1:dd.MM.yyyy} {1:HH:mm:ss.fff}", a.BeginTsUtc, a.EndTsUtc)));
          
          throw new Exception(string.Format("{0}: Found intervall gap. First Interval: BeginTsUtc:{1:dd.MM.yyyy} {1:HH:mm:ss.fff} EndTsUtc:{2:dd.MM.yyyy} {2:HH:mm:ss.fff}\r\nSecond Interval: BeginTsUtc:{3:dd.MM.yyyy} {3:HH:mm:ss.fff} EndTsUtc:{4:dd.MM.yyyy} {4:HH:mm:ss.fff}\r\nIntervalList:\r\n{5}", new object[] { argumentName, data.BeginTsUtc, data.EndTsUtc, data2.BeginTsUtc, data2.EndTsUtc, lSb.ToString() }));
        }
      }
    }

    public static void CheckLocal(DateTime dt, string argumentName)
    {
      if (dt.Kind != DateTimeKind.Local)
        throw new ArgumentException("Incoming DateTime is not DateTimeKind.Local.", argumentName);
    }

    public static T EnsureRange<T>(T value, T minValueInclusive, T maxValueInclusive) where T : IComparable<T>
    {
      if (value.CompareTo(minValueInclusive) < 0)
        return minValueInclusive;

      if (value.CompareTo(maxValueInclusive) > 0)
        return maxValueInclusive;

      return value;
    }

    public static string EnsureStringLength(string s, int maxLength)
    {
      if (!string.IsNullOrEmpty(s) && (s.Length > maxLength))
        return s.Substring(0, maxLength);

      return s;
    }

    public static void GreaterThan<T>(T value, T compareValue, string message) where T : IComparable<T>
    {
      if (value.CompareTo(compareValue) <= 0)
        throw new ArgumentOutOfRangeException("value", message);
    }

    public static void GreaterThanOrEqual<T>(T value, T compareValue, string message) where T : IComparable<T>
    {
      if (value.CompareTo(compareValue) < 0)
        throw new ArgumentOutOfRangeException("value", message);
    }

    public static bool IsInRange<T>(T value, T minValueInclusive, T maxValueInclusive) where T : IComparable<T>
    {
      return ((value.CompareTo(minValueInclusive) >= 0) && (value.CompareTo(maxValueInclusive) <= 0));
    }

    public static void LessThan<T>(T value, T compareValue, string message) where T : IComparable<T>
    {
      if (value.CompareTo(compareValue) > 0)
        throw new ArgumentOutOfRangeException("value", message);
    }

    public static void ListItemsAreTypeOf<T>(IEnumerable<T> enumerable, Type type, string argumentName)
    {
      enumerable.ForEach<T>(delegate(T data)
      {
        if (data.GetType() != type)
        {
          throw new ArgumentException(string.Format("List item is not type of {0}", type.ToString()), "argumentName");
        }
      });
    }

    public static void ListItemsNotNull(IList list, string argumentName)
    {
      int lIndex = 0;

      foreach (var item in list)
      {
        if (item == null)
          throw new ArgumentNullException(argumentName, string.Format("Incoming List contain empty items List Count: {0} Index: {1}", list.Count, lIndex));

        lIndex++;
      }
    }

    public static void ListNotNullOrEmpty(IEnumerable enumerable, string argumentName)
    {
      ArgumentNotNull(enumerable, argumentName);

      foreach (var item in enumerable)
      {
        object current = item;
        return;
      }

      throw new ArgumentException("IEnumerable must not be empty.", argumentName);
    }

    public static void ListNotNullOrEmpty(IList list, string argumentName)
    {
      ArgumentNotNull(list, argumentName);

      if (list.Count == 0)
        throw new ArgumentException("List must not be empty.", argumentName);
    }

    private static string StackTraceToString()
    {
      var lSb = new StringBuilder(256);
      var lFrames = new StackTrace().GetFrames();

      for (int i = 1; i < lFrames.Length; i++)
      {
        MethodBase method = lFrames[i].GetMethod();
        lSb.AppendLine(string.Format("{0}:{1}", (method.ReflectedType != null) ? method.ReflectedType.Name : string.Empty, method.Name));
      }

      return lSb.ToString();
    }

    public static void TypeIsAssignableFrom(Type assignee, Type providedType, string message)
    {
      if (!assignee.IsAssignableFrom(providedType))
        throw new ArgumentException(message);
    }

    public static void ValidateDateTime(DateTime dt, string argumentName)
    {
      if (dt.Equals(DateTime.MinValue))
        throw new ArgumentException("DateTime must not be DateTime.Min", argumentName);
    }

    public static void ValidateInterval(params DateTimeInterval[] intervals)
    {
      foreach (DateTimeInterval interval in intervals)
      {
        if (interval.EndDate < interval.StartDate)
          throw new Exception(string.Format("Das angegebene Enddatum ist kleiner als das angegebene Startdatum.\r\nStartdatum: {0}\r\nEnddatum: {1}", interval.StartDate, interval.EndDate));
      }
    }

    public static void ValidateInterval(DateTime beginTs, DateTime endTs)
    {
      ValidateInterval(new DateTimeInterval[] { new DateTimeInterval(beginTs, endTs) });
    }

    public static void ValidateRange<T>(T value, T minValueInclusive, T maxValueInclusive, string message) where T : IComparable<T>
    {
      if (!IsInRange<T>(value, minValueInclusive, maxValueInclusive))
        throw new ArgumentOutOfRangeException("value", message);
    }
  }
}