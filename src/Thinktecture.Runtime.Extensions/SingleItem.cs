using Thinktecture.Collections;

namespace Thinktecture;

/// <summary>
/// Partial class containing convenience members.
/// </summary>
public static class SingleItem
{
   /// <summary>
   /// Creates a read-only collection with 1 item.
   /// </summary>
   /// <param name="item">The single item in the collection.</param>
   /// <typeparam name="T">Type of the item.</typeparam>
   /// <returns>A read-only collection.</returns>
   public static IReadOnlyList<T> Collection<T>(T item)
   {
      return new SingleItemReadOnlyCollection<T>(item);
   }

#if !NETSTANDARD2_1
   /// <summary>
   /// Creates a read-only set with 1 item.
   /// </summary>
   /// <param name="item">The single item in the set.</param>
   /// <param name="equalityComparer">Equality comparer.</param>
   /// <typeparam name="T">Type of the item.</typeparam>
   /// <returns>A read-only set.</returns>
   public static IReadOnlySet<T> Set<T>(T item, IEqualityComparer<T>? equalityComparer = null)
   {
      return new SingleItemReadOnlySet<T>(item, equalityComparer ?? EqualityComparer<T>.Default);
   }
#endif
}
