namespace Thinktecture;

public static class ReadOnlyCollectionExtensions
{
   public static int ComputeHashCode<T>(this IReadOnlyList<T> collection)
      where T : IEquatable<T>, IHashCodeComputable
   {
      var hashCode = typeof(T).GetHashCode();

      for (var i = 0; i < collection.Count; i++)
      {
         hashCode = (hashCode * 397) ^ collection[i].GetHashCode();
      }

      return hashCode;
   }

   public static int ComputeHashCode<T>(
      this IReadOnlyList<T> collection,
      IEqualityComparer<T> comparer)
      where T : IEquatable<T>
   {
      var hashCode = typeof(T).GetHashCode();

      for (var i = 0; i < collection.Count; i++)
      {
         hashCode = (hashCode * 397) ^ comparer.GetHashCode(collection[i]);
      }

      return hashCode;
   }

   public static int ComputeHashCode(this IReadOnlyList<string> collection)
   {
      var hashCode = typeof(string).GetHashCode();

      for (var i = 0; i < collection.Count; i++)
      {
         hashCode = (hashCode * 397) ^ collection[i].GetHashCode();
      }

      return hashCode;
   }
}
