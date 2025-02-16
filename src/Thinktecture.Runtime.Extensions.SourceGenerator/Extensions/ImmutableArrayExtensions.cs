namespace Thinktecture;

public static class ImmutableArrayExtensions
{
   public static bool Contains<T>(this ImmutableArray<T> array, T item, IEqualityComparer<T> comparer)
   {
      if (array.IsDefaultOrEmpty)
         return false;

      return array.IndexOf(item, 0, comparer) >= 0;
   }

   public static ImmutableArray<T> RemoveAll<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, bool> predicate, TArg arg)
   {
      if (array.IsDefaultOrEmpty)
         return ImmutableArray<T>.Empty;

      ImmutableArray<T> newArray;
      var all = true;
      var none = true;

      for (var i = 0; i < array.Length; i++)
      {
         var item = array[i];

         if (predicate(item, arg))
         {
            all = false;

            if (none || !newArray.IsDefault)
               continue;

            newArray = ImmutableArray.CreateRange(array, 0, i, elem => elem);
         }
         else
         {
            none = false;

            if (!newArray.IsDefault)
            {
               newArray = newArray.Add(item);
            }
            else if (!all)
            {
               newArray = [item];
            }
         }
      }

      if (none)
         return ImmutableArray<T>.Empty;

      return all || newArray.IsDefault ? array : newArray;
   }

   public static ImmutableArray<T> Where<T>(this ImmutableArray<T> array, Func<T, bool> predicate)
   {
      return array.Where(predicate, null, 0);
   }

   public static ImmutableArray<T> Where<T, TArg>(
      this ImmutableArray<T> array,
      Func<T, TArg, bool>? predicate,
      TArg arg)
   {
      return array.Where(null, predicate, arg);
   }

   private static ImmutableArray<T> Where<T, TArg>(
      this ImmutableArray<T> array,
      Func<T, bool>? predicate,
      Func<T, TArg, bool>? predicateWithArg,
      TArg arg)
   {
      if (array.IsDefaultOrEmpty)
         return ImmutableArray<T>.Empty;

      ImmutableArray<T> newArray;
      var all = true;
      var none = true;

      for (var i = 0; i < array.Length; i++)
      {
         var item = array[i];

         if (predicateWithArg?.Invoke(item, arg)
             ?? predicate?.Invoke(item)
             ?? throw new Exception("Both predicates must not be null"))
         {
            none = false;

            if (!newArray.IsDefault)
            {
               newArray = newArray.Add(item);
            }
            else if (!all)
            {
               newArray = [item];
            }
         }
         else
         {
            all = false;

            if (none || !newArray.IsDefault)
               continue;

            newArray = ImmutableArray.CreateRange(array, 0, i, static elem => elem);
         }
      }

      if (none)
         return ImmutableArray<T>.Empty;

      return all || newArray.IsDefault ? array : newArray;
   }

   public static ImmutableArray<TResult> SelectWhere<T, TResult>(
      this ImmutableArray<T> array,
      SelectWhereDelegate<T, TResult> predicate)
   {
      return array.SelectWhere(predicate, null, 0);
   }

   public static ImmutableArray<TResult> SelectWhere<T, TArg, TResult>(
      this ImmutableArray<T> array,
      SelectWhereDelegate<T, TArg, TResult> predicate,
      TArg arg)
   {
      return array.SelectWhere(null, predicate, arg);
   }

   private static ImmutableArray<TResult> SelectWhere<T, TArg, TResult>(
      this ImmutableArray<T> array,
      SelectWhereDelegate<T, TResult>? predicate,
      SelectWhereDelegate<T, TArg, TResult>? predicateWithArg,
      TArg arg)
   {
      if (array.IsDefaultOrEmpty)
         return ImmutableArray<TResult>.Empty;

      var newArray = ImmutableArray<TResult>.Empty;

      for (var i = 0; i < array.Length; i++)
      {
         var item = array[i];

         if (predicateWithArg?.Invoke(item, arg, out var newItem)
             ?? predicate?.Invoke(item, out newItem)
             ?? throw new Exception("Both predicates must not be null"))
         {
            newArray = newArray.Add(newItem);
         }
      }

      return newArray;
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
      where T : IHashCodeComputable
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

   public static int ComputeHashCode<T, TProperty>(this ImmutableArray<T> collection, Func<T, TProperty> selector)
      where TProperty : IHashCodeComputable
   {
      if (collection.IsDefaultOrEmpty)
         return 0;

      var hashCode = typeof(T).GetHashCode();

      for (var i = 0; i < collection.Length; i++)
      {
         hashCode = (hashCode * 397) ^ (selector(collection[i])?.GetHashCode() ?? 0);
      }

      return hashCode;
   }

   public static int ComputeHashCode<T>(this ImmutableArray<T> collection, Func<T, string> selector)
   {
      if (collection.IsDefaultOrEmpty)
         return 0;

      var hashCode = typeof(T).GetHashCode();

      for (var i = 0; i < collection.Length; i++)
      {
         hashCode = (hashCode * 397) ^ (selector(collection[i])?.GetHashCode() ?? 0);
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
      if (array.IsDefaultOrEmpty)
         return ImmutableArray<T>.Empty;

      if (array.Length == 1)
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
            distinctArray = ImmutableArray.CreateRange(array, 0, i, static arg => arg);
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
