namespace Thinktecture.CodeAnalysis;

/// <summary>
/// The immutable arrays must not contain duplicates.
/// </summary>
public sealed class SetComparer<T> : IEqualityComparer<ImmutableArray<T>>
   where T : IEquatable<T>
{
   public static readonly SetComparer<T> Instance = new();

   public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y)
   {
      if (x.IsDefaultOrEmpty)
         return y.IsDefaultOrEmpty;

      if (y.IsDefaultOrEmpty)
         return false;

      if (x.Length != y.Length)
         return false;

      var hashSet = SetComparerPool.Instance.Lease(EqualityComparer<T>.Default);

      try
      {
         for (var i = 0; i < y.Length; i++)
         {
            hashSet.Add(y[i]);
         }

         for (var i = 0; i < x.Length; i++)
         {
            if (!hashSet.Contains(x[i]))
               return false;
         }

         return true;
      }
      finally
      {
         SetComparerPool.Instance.Return(hashSet);
      }
   }

   public int GetHashCode(ImmutableArray<T> obj)
   {
      // We cannot include the hash of the items, because the hashcode changes
      // if the order is different, but in (mathematical) sets we don't want to be order-specific.

      return obj.IsDefaultOrEmpty ? 0 : obj.Length.GetHashCode();
   }

   private sealed class SetComparerPool : ReusableHashSet<T>
   {
      private static readonly ThreadLocal<SetComparerPool> _instance = new(() => new SetComparerPool(1000));

      public static SetComparerPool Instance => _instance.Value;

      private SetComparerPool(int maxSetSize)
         : base(maxSetSize)
      {
      }
   }
}
