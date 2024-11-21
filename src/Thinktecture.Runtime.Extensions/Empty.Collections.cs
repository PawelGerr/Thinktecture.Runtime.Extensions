using System.Collections;
using Thinktecture.Collections;

namespace Thinktecture;

public partial class Empty
{
   /// <summary>
   /// Returns empty non-generic collection of type <see cref="IEnumerable"/>.
   /// </summary>
   /// <returns>An empty collection.</returns>
   public static IEnumerable Collection()
   {
      return Enumerable.Empty<object>();
   }

   /// <summary>
   /// Returns empty generic collection of type <see cref="IReadOnlyList{T}"/>.
   /// </summary>
   /// <returns>An empty collection.</returns>
   public static IReadOnlyList<T> Collection<T>()
   {
      return Array.Empty<T>();
   }

   /// <summary>
   /// Returns empty dictionary.
   /// </summary>
   /// <returns>An empty dictionary.</returns>
   public static IReadOnlyDictionary<TKey, TValue> Dictionary<TKey, TValue>()
      where TKey : notnull
   {
      return EmptyDictionary<TKey, TValue>.Instance;
   }

   /// <summary>
   /// Returns empty set.
   /// </summary>
   /// <returns>An empty set.</returns>
   public static IReadOnlySet<T> Set<T>()
   {
      return EmptySet<T>.Instance;
   }

   /// <summary>
   /// Returns empty lookup.
   /// </summary>
   /// <returns>An empty lookup.</returns>
   public static ILookup<TKey, TValue> Lookup<TKey, TValue>()
   {
      return EmptyLookup<TKey, TValue>.Instance;
   }
}
