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
   /// <returns>A read-only collection with 1 item.</returns>
   public static IReadOnlyList<T> Collection<T>(T item)
   {
      return new SingleItemReadOnlyCollection<T>(item);
   }

   /// <summary>
   /// Creates a read-only dictionary with 1 item.
   /// </summary>
   /// <param name="key">The key.</param>
   /// <param name="value">The value.</param>
   /// <param name="equalityComparer">The key equality comparer.</param>
   /// <typeparam name="TKey">Type of the key</typeparam>
   /// <typeparam name="TValue">Type of the value.</typeparam>
   /// <returns>A read-only dictionary with 1 item.</returns>
   public static IReadOnlyDictionary<TKey, TValue> Dictionary<TKey, TValue>(
      TKey key,
      TValue value,
      IEqualityComparer<TKey>? equalityComparer = null)
      where TKey : notnull
   {
      return new SingleItemReadOnlyDictionary<TKey, TValue>(key, value, equalityComparer);
   }

   /// <summary>
   /// Creates a lookup with 1 item.
   /// </summary>
   /// <param name="key">The key.</param>
   /// <param name="elements">The elements.</param>
   /// <param name="equalityComparer">The key equality comparer.</param>
   /// <typeparam name="TKey">Type of the key</typeparam>
   /// <typeparam name="TElement">Type of the elements.</typeparam>
   /// <returns>A lookup with 1 item.</returns>
   public static ILookup<TKey, TElement> Lookup<TKey, TElement>(
      TKey key,
      IEnumerable<TElement> elements,
      IEqualityComparer<TKey>? equalityComparer = null)
      where TKey : notnull
   {
      return new SingleItemLookup<TKey, TElement>(key, elements, equalityComparer);
   }

#if !NETSTANDARD2_1
   /// <summary>
   /// Creates a read-only set with 1 item.
   /// </summary>
   /// <param name="item">The single item in the set.</param>
   /// <param name="equalityComparer">Equality comparer.</param>
   /// <typeparam name="T">Type of the item.</typeparam>
   /// <returns>A read-only set with 1 item.</returns>
   public static IReadOnlySet<T> Set<T>(T item, IEqualityComparer<T>? equalityComparer = null)
   {
      return new SingleItemReadOnlySet<T>(item, equalityComparer ?? EqualityComparer<T>.Default);
   }
#endif
}
