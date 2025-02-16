using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Collections;

internal sealed class EmptyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
   where TKey : notnull
{
   public static readonly IReadOnlyDictionary<TKey, TValue> Instance = new EmptyDictionary<TKey, TValue>();

   public int Count => 0;
   public TValue this[TKey key] => throw new KeyNotFoundException();
   public IEnumerable<TKey> Keys => [];
   public IEnumerable<TValue> Values => [];

   public bool ContainsKey(TKey key)
   {
      return false;
   }

   public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
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
