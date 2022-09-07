using System.Collections;

namespace Thinktecture.Collections;

internal sealed class SingleItemEnumerator<T> : IEnumerator<T>
{
   private readonly T _item;

   private int _index;

   public T Current => _index == 1 ? _item : default!;

   object IEnumerator.Current => Current!;

   public SingleItemEnumerator(T item)
   {
      _item = item;
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
