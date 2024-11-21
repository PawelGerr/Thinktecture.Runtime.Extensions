using System.Collections;

namespace Thinktecture.Collections;

internal sealed class EnumerableWrapperWithCount<T> : IReadOnlyCollection<T>
{
   private readonly IEnumerable<T> _items;

   public int Count { get; }

   public EnumerableWrapperWithCount(IEnumerable<T> items, int count)
   {
      if (count < 0)
         throw new ArgumentOutOfRangeException(nameof(count), "The count cannot be negative.");

      _items = items ?? throw new ArgumentNullException(nameof(items));
      Count = count;
   }

   public IEnumerator<T> GetEnumerator()
   {
      return _items.GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return ((IEnumerable)_items).GetEnumerator();
   }
}
