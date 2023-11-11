namespace Thinktecture;

public abstract class ReusableHashSet<T>
{
   private readonly int _maxSetSize;

   private (HashSet<T> Set, IEqualityComparer<T> Comparer)? _cacheWithComparer;

   protected ReusableHashSet(int maxSetSize)
   {
      _maxSetSize = maxSetSize;
   }

   public HashSet<T> Lease(IEqualityComparer<T> comparer)
   {
      if (_cacheWithComparer is null || !ReferenceEquals(_cacheWithComparer?.Comparer, comparer))
         return new HashSet<T>(comparer);

      var set = _cacheWithComparer.Value.Set;
      _cacheWithComparer = null;

      return set;
   }

   public void Return(HashSet<T> set)
   {
      if (set.Count > _maxSetSize)
         return;

      set.Clear();
      _cacheWithComparer = (set, set.Comparer);
   }
}
