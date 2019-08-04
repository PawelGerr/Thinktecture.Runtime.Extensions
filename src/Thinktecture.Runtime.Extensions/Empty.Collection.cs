using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Thinktecture
{
   [SuppressMessage("ReSharper", "UnusedMember.Global")]
   public partial class Empty
   {
      /// <summary>
      /// Returns empty non-generic collection of type <see cref="IEnumerable"/>.
      /// </summary>
      /// <returns>An empty collection.</returns>
      [NotNull]
      public static IEnumerable Collection()
      {
         return Enumerable.Empty<object>();
      }

      /// <summary>
      /// Returns empty generic collection of type <see cref="IReadOnlyList{T}"/>.
      /// </summary>
      /// <returns>An empty collection.</returns>
      [NotNull]
      public static IReadOnlyList<T> Collection<T>()
      {
#if NET45
         return ReadOnlyList<T>.Instance;
#else
			return Array.Empty<T>();
#endif
      }

      /// <summary>
      /// Returns empty dictionary.
      /// </summary>
      /// <returns>An empty dictionary.</returns>
      [NotNull]
      public static IReadOnlyDictionary<TKey, TValue> Dictionary<TKey, TValue>()
      {
         return ReadOnlyDictionary<TKey, TValue>.Instance;
      }

      private class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
      {
         public static readonly IReadOnlyDictionary<TKey, TValue> Instance = new ReadOnlyDictionary<TKey, TValue>();

         public int Count => 0;
         public TValue this[TKey key] => throw new KeyNotFoundException();

         [NotNull]
         public IEnumerable<TKey> Keys => Enumerable.Empty<TKey>();

         [NotNull]
         public IEnumerable<TValue> Values => Enumerable.Empty<TValue>();

         public bool ContainsKey(TKey key)
         {
            return false;
         }

         public bool TryGetValue(TKey key, [CanBeNull] out TValue value)
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

#if NET45
      private class ReadOnlyList<T> : IReadOnlyList<T>
      {
         public static readonly IReadOnlyList<T> Instance = new ReadOnlyList<T>();

         public int Count => 0;
         public T this[int index] => throw new ArgumentOutOfRangeException(nameof(index), index, "Index was out of range. Must be non-negative and less than the size of the collection.");

         public IEnumerator<T> GetEnumerator()
         {
            return Enumerable.Empty<T>().GetEnumerator();
         }

         IEnumerator IEnumerable.GetEnumerator()
         {
            return GetEnumerator();
         }
      }
#endif
   }
}
