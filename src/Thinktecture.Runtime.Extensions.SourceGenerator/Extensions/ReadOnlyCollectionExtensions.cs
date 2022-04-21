namespace Thinktecture;

public static class ReadOnlyCollectionExtensions
{
   public static bool EqualsTo<T>(this IReadOnlyList<T> collection, IReadOnlyList<T> other)
      where T : IEquatable<T>
   {
      if (collection.Count != other.Count)
         return false;

      for (var i = 0; i < collection.Count; i++)
      {
         var item = collection[i];
         var otherItem = other[i];

         if (!item.Equals(otherItem))
            return false;
      }

      return true;
   }

   public static int ComputeHashCode<T>(this IReadOnlyList<T> collection)
      where T : IEquatable<T>
   {
      var hashCode = typeof(T).GetHashCode();

      for (var i = 0; i < collection.Count; i++)
      {
         hashCode = (hashCode * 397) ^ collection[i].GetHashCode();
      }

      return hashCode;
   }
}
