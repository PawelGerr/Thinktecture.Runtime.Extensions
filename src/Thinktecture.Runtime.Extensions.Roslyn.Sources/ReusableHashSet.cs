namespace Thinktecture;

public abstract class ReusableHashSet<T>(int maxSetSize)
{
   private HashSet<T>? _cache;

   public HashSet<T> Lease(IEqualityComparer<T> comparer)
   {
      var set = _cache;

      if (set is not null
          && ReferenceEquals(set.Comparer, comparer)
          && ReferenceEquals(set, Interlocked.CompareExchange(ref _cache, null, set)))
      {
         return set;
      }

      return new HashSet<T>(comparer);
   }

   public void Return(HashSet<T> set)
   {
      // "Capacity" doesn't exist in .netstandard2.0
      if (set.Count > maxSetSize)
         return;

      set.Clear();

      // Only cache if the slot is empty; otherwise, drop it
      Interlocked.CompareExchange(ref _cache, set, null);
   }
}
