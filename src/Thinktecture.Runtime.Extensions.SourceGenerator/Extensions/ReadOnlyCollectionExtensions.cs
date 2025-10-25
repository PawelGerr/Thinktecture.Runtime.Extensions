namespace Thinktecture;

public static class ReadOnlyCollectionExtensions
{
   public static int ComputeHashCode<T>(this IReadOnlyList<T> collection)
      where T : IEquatable<T>, IHashCodeComputable
   {
      var hashCode = 0;
      var count = collection.Count;

      for (var i = 0; i < count; i++)
      {
         hashCode = unchecked(hashCode * 397) ^ collection[i].GetHashCode();
      }

      return hashCode;
   }

   public static int ComputeHashCode<T>(
      this IReadOnlyList<T> collection,
      IEqualityComparer<T> comparer)
      where T : IEquatable<T>
   {
      var hashCode = 0;
      var count = collection.Count;

      for (var i = 0; i < count; i++)
      {
         hashCode = unchecked(hashCode * 397) ^ comparer.GetHashCode(collection[i]);
      }

      return hashCode;
   }
}
