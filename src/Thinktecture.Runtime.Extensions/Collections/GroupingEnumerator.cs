using System.Collections;

namespace Thinktecture.Collections;

internal class GroupingEnumerator<TKey, TElement> : IEnumerator<IGrouping<TKey, TElement>>, IGrouping<TKey, TElement>
{
   private readonly IEnumerable<TElement> _elements;

   private int _index;

   public TKey Key { get; }

   public IGrouping<TKey, TElement> Current => _index == 1 ? this : default!;

   object IEnumerator.Current => Current;

   public GroupingEnumerator(TKey key, IEnumerable<TElement> elements)
   {
      _elements = elements;
      Key = key;
   }

   public IEnumerator<TElement> GetEnumerator()
   {
      return _elements.GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }

   public bool MoveNext()
   {
      return _index++ == 0;
   }

   public void Reset()
   {
      _index = 0;
   }

   public void Dispose()
   {
   }
}
