using System.Collections;

namespace Thinktecture.Collections;

internal sealed class SingleItemLookup<TKey, TElement> : ILookup<TKey, TElement>
{
   private readonly TKey _key;
   private readonly IEnumerable<TElement> _elements;
   private readonly IEqualityComparer<TKey> _equalityComparer;

   public int Count => 1;

   public IEnumerable<TElement> this[TKey key] => _equalityComparer.Equals(_key, key) ? _elements : Enumerable.Empty<TElement>();

   public SingleItemLookup(TKey key, IEnumerable<TElement> elements, IEqualityComparer<TKey>? equalityComparer = null)
   {
      _key = key;
      _elements = elements;
      _equalityComparer = equalityComparer ?? EqualityComparer<TKey>.Default;
   }

   public bool Contains(TKey key)
   {
      return _equalityComparer.Equals(_key, key);
   }

   public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
   {
      return new GroupingEnumerator<TKey, TElement>(_key, _elements);
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }
}
