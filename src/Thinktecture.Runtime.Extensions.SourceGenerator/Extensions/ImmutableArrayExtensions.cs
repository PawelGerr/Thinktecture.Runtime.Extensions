namespace Thinktecture;

public static class ImmutableArrayExtensions
{
   public static ImmutableArray<T> RemoveAll<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, bool> predicate, TArg arg)
   {
      if (array.IsDefaultOrEmpty)
         return ImmutableArray<T>.Empty;

      var length = array.Length;

      // Find first element to remove (fast-path to return original if none)
      var index = 0;

      for (; index < length; index++)
      {
         if (predicate(array[index], arg))
            break;
      }

      if (index == length)
         return array;

      var builder = ImmutableArray.CreateBuilder<T>(length - 1);

      // Copy prefix [0, index) in one shot if possible
      if (index > 0)
      {
         builder.AddRange(array.AsSpan(0, index));
      }

      // For the suffix, add contiguous keep-runs via AddRange to minimize per-item Add overhead
      var segmentStart = index + 1;

      for (var i = segmentStart; i < length; i++)
      {
         if (!predicate(array[i], arg))
            continue;

         var runLength = i - segmentStart;

         if (runLength > 0)
            builder.AddRange(array.AsSpan(segmentStart, runLength));

         segmentStart = i + 1;
      }

      // Tail run
      if (segmentStart < length)
      {
         builder.AddRange(array.AsSpan(segmentStart, length - segmentStart));
      }

      return builder.Count == 0 ? ImmutableArray<T>.Empty : builder.DrainToImmutable();
   }

   public static ImmutableArray<TResult> SelectWhere<T, TResult>(
      this ImmutableArray<T> array,
      SelectWhere<T, TResult> predicate)
   {
      return array.SelectWhere<T, object?, TResult>(predicate, null, null);
   }

   public static ImmutableArray<TResult> SelectWhere<T, TArg, TResult>(
      this ImmutableArray<T> array,
      SelectWhere<T, TArg, TResult> predicate,
      TArg arg)
   {
      return array.SelectWhere(null, predicate, arg);
   }

   private static ImmutableArray<TResult> SelectWhere<T, TArg, TResult>(
      this ImmutableArray<T> array,
      SelectWhere<T, TResult>? predicate,
      SelectWhere<T, TArg, TResult>? predicateWithArg,
      TArg arg)
   {
      if (array.IsDefaultOrEmpty)
         return ImmutableArray<TResult>.Empty;

      var length = array.Length;
      ImmutableArray<TResult>.Builder? builder = null;

      for (var i = 0; i < length; i++)
      {
         var item = array[i];

         if (predicateWithArg?.Invoke(item, arg, out var newItem)
             ?? predicate?.Invoke(item, out newItem)
             ?? throw new InvalidOperationException("Both predicates must not be null"))
         {
            builder ??= ImmutableArray.CreateBuilder<TResult>(length - i);
            builder.Add(newItem);
         }
      }

      if (builder is null || builder.Count == 0)
         return ImmutableArray<TResult>.Empty;

      return builder.DrainToImmutable();
   }

   public static int ComputeHashCode<T>(this ImmutableArray<T> collection)
      where T : IHashCodeComputable
   {
      if (collection.IsDefaultOrEmpty)
         return 0;

      var hashCode = 0;
      var count = collection.Length;

      for (var i = 0; i < count; i++)
      {
         hashCode = unchecked(hashCode * 397) ^ collection[i].GetHashCode();
      }

      return hashCode;
   }

   public static ImmutableArray<T> Distinct<T>(
      this ImmutableArray<T> array)
   {
      return array.Distinct(EqualityComparer<T>.Default);
   }

   public static ImmutableArray<T> Distinct<T>(
      this ImmutableArray<T> array,
      IEqualityComparer<T> comparer)
   {
      if (array.IsDefaultOrEmpty)
         return ImmutableArray<T>.Empty;

      if (array.Length == 1)
         return array;

      var set = SetForDistinct<T>.Instance.Lease(comparer);

      // First pass: count unique items
      var uniqueCount = 0;

      for (var i = 0; i < array.Length; i++)
      {
         if (set.Add(array[i]))
            uniqueCount++;
      }

      // No duplicates -> return original
      if (uniqueCount == array.Length)
      {
         SetForDistinct<T>.Instance.Return(set);
         return array;
      }

      // Prepare for second pass
      set.Clear();

      var builder = ImmutableArray.CreateBuilder<T>(uniqueCount);

      for (var i = 0; i < array.Length; i++)
      {
         var item = array[i];

         if (set.Add(item))
            builder.Add(item);
      }

      SetForDistinct<T>.Instance.Return(set);

      return builder.DrainToImmutable();
   }

   private sealed class SetForDistinct<T> : ReusableHashSet<T>
   {
      private static readonly ThreadLocal<SetForDistinct<T>> _instance = new(() => new SetForDistinct<T>(1000));

      public static SetForDistinct<T> Instance => _instance.Value;

      private SetForDistinct(int maxSetSize)
         : base(maxSetSize)
      {
      }
   }
}
