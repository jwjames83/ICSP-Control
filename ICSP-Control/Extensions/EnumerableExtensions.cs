using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ICSPControl.Extensions
{
  public static class EnumerableExtensions
  {
    public static TSource OneOrDefault<TSource>(this IEnumerable<TSource> source)
    {
      if (source.Count() == 1)
        return source.First();
      else
        return default(TSource);
    }

    public static TSource OneOrDefault<TSource>(this IQueryable<TSource> source)
    {
      if (source.Count() == 1)
        return source.First();
      else
        return default(TSource);
    }

    public static IEnumerable<T> Between<T>(this IEnumerable<T> source, Func<T, bool> endPredicate)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }
      if (endPredicate == null)
      {
        throw new ArgumentNullException("endPredicate");
      }
      return BetweenIterator<T>(source, b => true, endPredicate);
    }

    public static IEnumerable<T> Between<T>(this IEnumerable<T> source, Func<T, bool> startPredicate, Func<T, bool> endPredicate)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }
      if (startPredicate == null)
      {
        throw new ArgumentNullException("startPredicate");
      }
      if (endPredicate == null)
      {
        throw new ArgumentNullException("endPredicate");
      }
      return BetweenIterator<T>(source, startPredicate, endPredicate);
    }

    private static IEnumerable<T> BetweenIterator<T>(IEnumerable<T> source, Func<T, bool> startPredicate, Func<T, bool> endPredicate)
    {
      bool iteratorVariable0 = false;
      foreach (T iteratorVariable1 in source)
      {
        if (startPredicate(iteratorVariable1))
        {
          iteratorVariable0 = true;
        }
        if (iteratorVariable0)
        {
          if (!endPredicate(iteratorVariable1))
          {
            yield return iteratorVariable1;
          }
          else
          {
            break;
          }
        }
      }
    }

    public static bool Contains<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
    {
      foreach (TSource local in source)
      {
        if (predicate(local))
        {
          return true;
        }
      }
      return false;
    }

    public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, Comparison<TSource> comparision)
    {
      if (comparision == null)
      {
        Comparer<TSource> comparer1 = Comparer<TSource>.Default;
        comparision = new Comparison<TSource>(comparer1.Compare);
      }
      foreach (TSource local in source)
      {
        if (comparision(local, value) == 0)
        {
          return true;
        }
      }
      return false;
    }

    public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, Comparison<TSource> comparision)
    {
      if (comparision == null)
      {
        Comparer<TSource> comparer1 = Comparer<TSource>.Default;
        comparision = new Comparison<TSource>(comparer1.Compare);
      }
      List<TSource> list = new List<TSource>();
      foreach (TSource local in source)
      {
        if (!list.Contains<TSource>(local, comparision))
        {
          list.Add(local);
        }
      }
      return list;
    }

    public static IEnumerable<TMemberType> Distinct<TSource, TMemberType>(this IEnumerable<TSource> source, Func<TSource, TMemberType> func)
    {
      return source.Distinct<TSource, TMemberType>(func, null);
    }

    public static IEnumerable<TMemberType> Distinct<TSource, TMemberType>(this IEnumerable<TSource> source, Func<TSource, TMemberType> func, Comparison<TMemberType> comparision)
    {
      if (comparision == null)
      {
        Comparer<TMemberType> comparer1 = Comparer<TMemberType>.Default;
        comparision = new Comparison<TMemberType>(comparer1.Compare);
      }
      List<TMemberType> list = new List<TMemberType>();
      foreach (TSource local in source)
      {
        TMemberType local2 = func(local);
        if (!list.Contains<TMemberType>(local2, comparision))
        {
          list.Add(local2);
        }
      }
      return list;
    }

    public static T FirstOrDefault<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> func, T defaultValue)
    {
      TSource arg = source.FirstOrDefault<TSource>();
      if (arg == null)
      {
        return defaultValue;
      }
      return func(arg);
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
      if (source != null)
      {
        foreach (T local in source)
        {
          action(local);
        }
      }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
      T[] localArray = source.ToArray<T>();
      for (int i = 0; i < localArray.Length; i++)
      {
        action(localArray[i], i);
      }
    }

    public static void ForEachRange<T>(this IEnumerable<T> source, int rangeSize, Action<IEnumerable<T>> action)
    {
      // Guard.ArgumentNotNull(source, "source must not be null!");
      // Guard.GreaterThan<int>(rangeSize, 0, "Range size must be greater then 0");

      int num = 0;
      List<T> list = new List<T>(rangeSize);
      foreach (T local in source)
      {
        list.Add(local);
        if (num == (rangeSize - 1))
        {
          action(list);
          list.Clear();
          num = 0;
        }
        num++;
      }
      if (list.Count > 0)
      {
        action(list);
      }
    }

    public static bool IsEmptyOrNull<T>(this IEnumerable<T> source)
    {
      if (source != null)
      {
        using (IEnumerator<T> enumerator = source.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            T current = enumerator.Current;
            return false;
          }
        }
      }
      return true;
    }

    //public static DataTable ToDataTable<TSource>(this IEnumerable<TSource> source)
    //{
    //  return source.ToDataTable<TSource>(string.Empty);
    //}

    //public static DataTable ToDataTable<TSource>(this IEnumerable<TSource> source, string tableName)
    //{
    //  if (!typeof(TSource).IsPrimitive && !(typeof(TSource) == typeof(string)))
    //  {
    //    return ToDataTableFromComplex<TSource>(source, tableName);
    //  }
    //  return ToDataTableFromPrimitive<TSource>(source, tableName);
    //}

    //private static DataTable ToDataTableFromComplex<TSource>(IEnumerable<TSource> source, string tableName)
    //{
    //  DataTable dt = new DataTable(tableName);
    //  var propInfoList = (from p in typeof(TSource).GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
    //    where p.CanRead
    //    select p).Select(((Func<PropertyInfo, int, <>f__AnonymousType0<string, Type, int, MethodInfo>>) ((f, i) => new { Name = f.Name, PropertyType = f.PropertyType, Index = i, GetMethod = f.GetGetMethod() }))).ToList();

    //  propInfoList.ForEach(delegate (<>f__AnonymousType0<string, Type, int, MethodInfo> p) {      dt.Columns.Add(p.Name, NullableHelper.GetType(p.PropertyType));    });
    //  source.ForEach<TSource>(delegate (TSource itm) {
    //    DataRow row = dt.NewRow();
    //    foreach (var type in propInfoList)
    //    {
    //      row[type.Index] = type.GetMethod.Invoke(itm, null) ?? DBNull.Value;
    //    }
    //    dt.Rows.Add(row);
    //  });
    //  return dt;
    //}

    private static DataTable ToDataTableFromPrimitive<TSource>(IEnumerable<TSource> source, string tableName)
    {
      DataTable table = new DataTable(tableName);
      table.Columns.Add("Data", typeof(TSource));
      foreach (TSource local in source)
      {
        DataRow row = table.NewRow();
        row[0] = local;
        table.Rows.Add(row);
      }
      return table;
    }

  }
}