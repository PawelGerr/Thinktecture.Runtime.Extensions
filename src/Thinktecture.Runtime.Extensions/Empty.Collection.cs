using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Thinktecture
{
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
   }
}
