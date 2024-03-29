using System.Collections;

namespace Thinktecture.Collections;

internal sealed class EmptySet<T> : IReadOnlySet<T>
{
   public static readonly IReadOnlySet<T> Instance = new EmptySet<T>();

   public int Count => 0;

   public IEnumerator<T> GetEnumerator()
   {
      return Enumerable.Empty<T>().GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }

   public bool Contains(T item)
   {
      return false;
   }

   public bool IsProperSubsetOf(IEnumerable<T> other)
   {
      ArgumentNullException.ThrowIfNull(other);

      return other.Any();
   }

   public bool IsSubsetOf(IEnumerable<T> other)
   {
      return true;
   }

   public bool IsProperSupersetOf(IEnumerable<T> other)
   {
      return false;
   }

   public bool IsSupersetOf(IEnumerable<T> other)
   {
      return !other.Any();
   }

   public bool Overlaps(IEnumerable<T> other)
   {
      return false;
   }

   public bool SetEquals(IEnumerable<T> other)
   {
      return !other.Any();
   }
}
