namespace Thinktecture;

public static class ImmutableArrayExtensions
{
   public static bool Contains<T>(this ImmutableArray<T> array, T item, IEqualityComparer<T> comparer)
   {
      if (array.IsDefaultOrEmpty)
         return false;

      for (var i = 0; i < array.Length; i++)
      {
         var arrayItem = array[i];

         if (comparer.Equals(arrayItem, item))
            return true;
      }

      return false;
   }

   public static ImmutableArray<T> RemoveAll<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, bool> predicate, TArg arg)
   {
      if (array.IsDefaultOrEmpty)
         return array;

      ImmutableArray<T> newArray;

      for (var i = 0; i < array.Length; i++)
      {
         var item = array[i];

         if (predicate(item, arg))
         {
            if (!newArray.IsDefault)
               continue;

            newArray = ImmutableArray.CreateRange(array, 0, i, elem => elem);
         }
         else if (!newArray.IsDefault)
         {
            newArray = newArray.Add(item);
         }
      }

      return newArray.IsDefault ? array : newArray;
   }

   public static T? FirstOrDefault<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, bool> predicate, TArg arg)
   {
      if (array.IsDefaultOrEmpty)
         return default;

      for (var i = 0; i < array.Length; i++)
      {
         var item = array[i];

         if (predicate(item, arg))
            return item;
      }

      return default;
   }

   public static int ComputeHashCode<T>(this ImmutableArray<T> collection)
      where T : IEquatable<T>, IHashCodeComputable
   {
      if (collection.IsDefaultOrEmpty)
         return 0;

      var hashCode = typeof(T).GetHashCode();

      for (var i = 0; i < collection.Length; i++)
      {
         hashCode = (hashCode * 397) ^ collection[i].GetHashCode();
      }

      return hashCode;
   }

   public static ImmutableArray<T> Distinct<T>(
      this ImmutableArray<T> array)
   {
      return array.Distinct(EqualityComparer<T>.Default);
   }

   public static ImmutableArray<T> Distinct<T>(
      this ImmutableArray<T> array,
      IEqualityComparer<T> comparer)
   {
      if (array.IsDefaultOrEmpty || array.Length == 1)
         return array;

      var set = SetForDistinct<T>.Instance.Lease(comparer);
      ImmutableArray<T> distinctArray;

      for (var i = 0; i < array.Length; i++)
      {
         var arrayItem = array[i];

         // unique item
         if (set.Add(arrayItem))
         {
            if (!distinctArray.IsDefault)
               distinctArray = distinctArray.Add(arrayItem);
         }
         // duplicate
         else if (distinctArray.IsDefault)
         {
            distinctArray = ImmutableArray.CreateRange(array, 0, i, arg => arg);
         }
      }

      SetForDistinct<T>.Instance.Return(set);

      return distinctArray.IsDefault ? array : distinctArray;
   }

   private class SetForDistinct<T> : ReusableHashSet<T>
   {
      private static readonly ThreadLocal<SetForDistinct<T>> _instance = new(() => new SetForDistinct<T>(1000));

      public static SetForDistinct<T> Instance => _instance.Value;

      private SetForDistinct(int maxSetSize)
         : base(maxSetSize)
      {
      }
   }
}
