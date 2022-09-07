using System.Collections;

namespace Thinktecture.Collections;

#if !NETSTANDARD2_1
internal sealed class SingleItemReadOnlySet<T> : IReadOnlySet<T>
{
   private readonly T _item;
   private readonly IEqualityComparer<T> _comparer;

   public int Count => 1;

   public SingleItemReadOnlySet(T item, IEqualityComparer<T> comparer)
   {
      _item = item;
      _comparer = comparer;
   }

   public IEnumerator<T> GetEnumerator()
   {
      return new SingleItemEnumerator<T>(_item);
   }

   IEnumerator IEnumerable.GetEnumerator()
   {
      return GetEnumerator();
   }

   public bool Contains(T item)
   {
      return _comparer.Equals(_item, item);
   }

   public bool IsProperSubsetOf(IEnumerable<T> other)
   {
      var foundItem = false;
      var foundOtherItem = false;

      foreach (var item in other)
      {
         if (_comparer.Equals(_item, item))
         {
            if (foundOtherItem)
               return true;

            foundItem = true;
         }
         else
         {
            if (foundItem)
               return true;

            foundOtherItem = true;
         }
      }

      return false;
   }

   public bool IsProperSupersetOf(IEnumerable<T> other)
   {
      return !other.Any();
   }

   public bool IsSubsetOf(IEnumerable<T> other)
   {
      return other.Contains(_item, _comparer);
   }

   public bool IsSupersetOf(IEnumerable<T> other)
   {
      return other.All(item => _comparer.Equals(_item, item));
   }

   public bool Overlaps(IEnumerable<T> other)
   {
      return other.Contains(_item, _comparer);
   }

   public bool SetEquals(IEnumerable<T> other)
   {
      var found = false;

      foreach (var item in other)
      {
         if (!_comparer.Equals(_item, item))
            return false;

         found = true;
      }

      return found;
   }
}
#endif
