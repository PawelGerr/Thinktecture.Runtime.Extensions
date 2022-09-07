using System.Collections;

namespace Thinktecture.Collections;

internal class ReadOnlyLookup<TKey, TValue> : ILookup<TKey, TValue>
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
