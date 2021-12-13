using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Thinktecture;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
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
      return ReadOnlyDictionary<TKey, TValue>.Instance;
   }

#if !NETSTANDARD2_1
   /// <summary>
   /// Returns empty set.
   /// </summary>
   /// <returns>An empty set.</returns>
   public static IReadOnlySet<T> Set<T>()
   {
      return ReadOnlySet<T>.Instance;
   }
#endif

   /// <summary>
   /// Returns empty lookup.
   /// </summary>
   /// <returns>An empty lookup.</returns>
   public static ILookup<TKey, TValue> Lookup<TKey, TValue>()
   {
      return ReadOnlyLookup<TKey, TValue>.Instance;
   }

   private class ReadOnlyLookup<TKey, TValue> : ILookup<TKey, TValue>
   {
      public static readonly ILookup<TKey, TValue> Instance = new ReadOnlyLookup<TKey, TValue>();

      public int Count => 0;
      public IEnumerable<TValue> this[TKey key] => Enumerable.Empty<TValue>();

      public bool Contains(TKey key)
      {
         return false;
      }

      public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
      {
         return Enumerable.Empty<IGrouping<TKey, TValue>>().GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   private class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
      where TKey : notnull
   {
      public static readonly IReadOnlyDictionary<TKey, TValue> Instance = new ReadOnlyDictionary<TKey, TValue>();

      public int Count => 0;
      public TValue this[TKey key] => throw new KeyNotFoundException();
      public IEnumerable<TKey> Keys => Enumerable.Empty<TKey>();
      public IEnumerable<TValue> Values => Enumerable.Empty<TValue>();

      public bool ContainsKey(TKey key)
      {
         return false;
      }

#pragma warning disable CS8767
      public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767
      {
         value = default;
         return false;
      }

      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
      {
         return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

#if !NETSTANDARD2_1
   private class ReadOnlySet<T> : IReadOnlySet<T>
   {
      public static readonly IReadOnlySet<T> Instance = new ReadOnlySet<T>();

      public int Count => 0;

      public IEnumerator<T> GetEnumerator()
      {
         return Enumerable.Empty<T>().GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public bool Contains(T item)
      {
         return false;
      }

      public bool IsProperSubsetOf(IEnumerable<T> other)
      {
         if (other is null)
            throw new ArgumentNullException(nameof(other));

         return other.Any();
      }

      public bool IsSubsetOf(IEnumerable<T> other)
      {
         return true;
      }

      public bool IsProperSupersetOf(IEnumerable<T> other)
      {
         return false;
      }

      public bool IsSupersetOf(IEnumerable<T> other)
      {
         return !other.Any();
      }

      public bool Overlaps(IEnumerable<T> other)
      {
         return false;
      }

      public bool SetEquals(IEnumerable<T> other)
      {
         return !other.Any();
      }
   }
#endif
}
