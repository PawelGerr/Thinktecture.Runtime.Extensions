using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.Collections;

internal sealed class SingleItemReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
{
   private readonly TKey _key;
   private readonly TValue _value;
   private readonly IEqualityComparer<TKey> _equalityComparer;

   public int Count => 1;

   public TValue this[TKey key] => _equalityComparer.Equals(_key, key) ? _value : throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");

   public IEnumerable<TKey> Keys => SingleItem.Collection(_key);
   public IEnumerable<TValue> Values => SingleItem.Collection(_value);

   public SingleItemReadOnlyDictionary(TKey key, TValue value, IEqualityComparer<TKey>? equalityComparer = null)
   {
      _key = key;
      _value = value;
      _equalityComparer = equalityComparer ?? EqualityComparer<TKey>.Default;
   }

   public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
   {
      if (_equalityComparer.Equals(_key, key))
      {
         value = _value;
         return true;
      }

      value = default;
      return false;
   }

   public bool ContainsKey(TKey key)
   {
      return _equalityComparer.Equals(_key, key);
   }

   public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
   {
      return new SingleItemEnumerator<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(_key, _value));
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }
}
