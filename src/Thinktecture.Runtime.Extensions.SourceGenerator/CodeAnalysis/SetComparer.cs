namespace Thinktecture.CodeAnalysis;

public sealed class SetComparer<T> : IEqualityComparer<ImmutableArray<T>>
   where T : IEquatable<T>
{
   public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y)
   {
      if (x.IsDefaultOrEmpty)
         return y.IsDefaultOrEmpty;

      if (x.Length != y.Length)
         return false;

      for (var i = 0; i < x.Length; i++)
      {
         if (!y.Contains(x[i]))
            return false;
      }

      return true;
   }

   public int GetHashCode(ImmutableArray<T> obj)
   {
      // We cannot include the hash of the items, because the hashcode changes
      // if the order is different, but in (mathematical) sets we don't want to be order-specific.

      return obj.IsDefaultOrEmpty ? 0 : obj.Length.GetHashCode();
   }
}
