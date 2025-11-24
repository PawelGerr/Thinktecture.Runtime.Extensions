using System.Collections;

namespace Thinktecture.Collections;

internal sealed class SingleItemEnumerator<T> : IEnumerator<T>
{
   private int _index;

   public T Current => _index == 1 ? field : default!;

   object IEnumerator.Current => Current!;

   public SingleItemEnumerator(T item)
   {
      Current = item;
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
