using System.Collections;

namespace Thinktecture.Collections;

internal sealed class EmptyLookup<TKey, TValue> : ILookup<TKey, TValue>
{
   public static readonly ILookup<TKey, TValue> Instance = new EmptyLookup<TKey, TValue>();

   public int Count => 0;
   public IEnumerable<TValue> this[TKey key] => [];

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
