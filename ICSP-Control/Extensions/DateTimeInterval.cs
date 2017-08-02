using System;

using ICSPControl.DevStuff;

namespace ICSPControl.Extensions
{
  [Serializable]
  public class DateTimeInterval : IDateTimeInterval
  {
    private DateTime mEndDate;
    private DateTime mStartDate;
    private TimeSpan mTimeSpan;

    public DateTimeInterval()
    {
    }

    public DateTimeInterval(DateTime startDate, DateTime endDate)
    {
      mStartDate = startDate;
      mEndDate = endDate;
      mTimeSpan = endDate.Subtract(startDate);

      Guard.ValidateInterval(new DateTimeInterval[] { this });
    }

    public DateTimeInterval Clone()
    {
      return new DateTimeInterval(StartDate, EndDate);
    }

    public bool Contains(DateTimeInterval interval)
    {
      if (interval == null)
        return false;

      return (StartDate <= interval.StartDate && EndDate >= interval.EndDate);
    }

    public bool Contains(DateTime dt)
    {
      return (dt >= StartDate && dt <= EndDate);
    }

    public bool Equals(DateTimeInterval intervalToCompare)
    {
      if (intervalToCompare == null)
        return false;

      return (StartDate.Equals(intervalToCompare.StartDate) && EndDate.Equals(intervalToCompare.EndDate));
    }

    public override bool Equals(object obj)
    {
      var lInterval = obj as DateTimeInterval;

      if (lInterval == null)
        return false;

      return (lInterval.StartDate == StartDate && lInterval.EndDate == EndDate);
    }

    public override int GetHashCode()
    {
      return (StartDate.GetHashCode() ^ EndDate.GetHashCode());
    }

    private void RecalcTimeSpan()
    {
      mTimeSpan = EndDate.Subtract(StartDate);
    }

    public override string ToString()
    {
      return string.Format("{0} - {1}", StartDate, EndDate);
    }

    public DateTimeInterval ToUtcDateTimeInterval()
    {
      return ToUtcDateTimeInterval(TimeZone.CurrentTimeZone);
    }

    public DateTimeInterval ToUtcDateTimeInterval(TimeZone timeZone)
    {
      var lStartDate = (StartDate.Kind == DateTimeKind.Utc) ? StartDate : TimeConverter.ToUniversalTime(StartDate, timeZone);

      return new DateTimeInterval(lStartDate, (EndDate.Kind == DateTimeKind.Utc) ? EndDate : TimeConverter.ToUniversalTime(EndDate, timeZone));
    }

    DateTime IDateTimeInterval.BeginTs
    {
      get
      {
        return StartDate;
      }
      set
      {
        StartDate = value;
      }
    }

    DateTime IDateTimeInterval.EndTs
    {
      get
      {
        return EndDate;
      }
      set
      {
        EndDate = value;
      }
    }

    public DateTimeKind DateTimeIntervalKind
    {
      get
      {
        if (StartDate.Kind != EndDate.Kind)
          return DateTimeKind.Unspecified;

        return StartDate.Kind;
      }
    }

    public DateTime EndDate
    {
      get
      {
        return mEndDate;
      }
      set
      {
        mEndDate = value;
        RecalcTimeSpan();
      }
    }

    public DateTime StartDate
    {
      get
      {
        return mStartDate;
      }
      set
      {
        mStartDate = value;
        RecalcTimeSpan();
      }
    }

    public TimeSpan TimeSpan
    {
      get
      {
        return mTimeSpan;
      }
    }
  }
}
