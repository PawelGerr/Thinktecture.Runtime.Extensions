using System.Collections;

namespace Thinktecture.Collections;

internal sealed class SingleItemReadOnlyCollection<T> : IReadOnlyList<T>
{
   private readonly T _item;

   public int Count => 1;

   public T this[int index] => index == 0 ? _item : throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

   public SingleItemReadOnlyCollection(T item)
   {
      _item = item;
   }

   public IEnumerator<T> GetEnumerator()
   {
      return new SingleItemEnumerator<T>(_item);
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }
}
