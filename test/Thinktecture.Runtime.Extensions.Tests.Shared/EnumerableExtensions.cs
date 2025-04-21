using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.Runtime.Tests;

public static class EnumerableExtensions
{
   public static IEnumerable<(T1, T2, T3, T4, T5)> CrossJoin<T1, T2, T3, T4, T5>(
      this IEnumerable<(T1, T2, T3, T4)> source,
      IEnumerable<T5> other)
   {
      return source.CrossJoin(other, (x, y) => (x.Item1, x.Item2, x.Item3, x.Item4, y));
   }

   public static IEnumerable<(T1, T2, T3, T4)> CrossJoin<T1, T2, T3, T4>(
      this IEnumerable<(T1, T2, T3)> source,
      IEnumerable<T4> other)
   {
      return source.CrossJoin(other, (x, y) => (x.Item1, x.Item2, x.Item3, y));
   }

   public static IEnumerable<(T1, T2, T3)> CrossJoin<T1, T2, T3>(
      this IEnumerable<(T1, T2)> source,
      IEnumerable<T3> other)
   {
      return source.CrossJoin(other, (x, y) => (x.Item1, x.Item2, y));
   }

   public static IEnumerable<(T1, T2)> CrossJoin<T1, T2>(
      this IEnumerable<T1> source,
      IEnumerable<T2> other)
   {
      return source.CrossJoin(other, (x, y) => (x, y));
   }

   public static IEnumerable<TResult> CrossJoin<T, TOther, TResult>(
      this IEnumerable<T> source,
      IEnumerable<TOther> other,
      Func<T, TOther, TResult> predicate)
   {
      return source.Join(other, _ => true, _ => true, predicate);
   }
}
