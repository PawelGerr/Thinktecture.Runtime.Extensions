using System;
using System.Collections.Generic;
using Thinktecture.Collections;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   /// <summary>
   /// Extension methods for <see cref="IEnumerable{T}"/>.
   /// </summary>
   public static class EnumerableExtensions
   {
      /// <summary>
      /// Wraps the provided <paramref name="items"/> and exposes them as <see cref="IReadOnlyCollection{T}"/>.
      /// </summary>
      /// <param name="items">Items to expose as an <see cref="IReadOnlyCollection{T}"/>.</param>
      /// <param name="count">The number of <paramref name="items"/>.</param>
      /// <typeparam name="T">Item type.</typeparam>
      /// <returns>A wrapper around <paramref name="items"/> that implements <see cref="IReadOnlyCollection{T}"/>.</returns>
      /// <exception cref="ArgumentNullException"><paramref name="items"/> is <c>null</c>.</exception>
      /// <exception cref="ArgumentException"><paramref name="count"/> is negative.</exception>
      public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> items, int count)
      {
         return new EnumerableWrapperWithCount<T>(items, count);
      }

      /// <summary>
      /// Splits the <paramref name="collection"/> in buckets of provided <paramref name="bucketSize"/>.
      /// </summary>
      /// <param name="collection">Collection to split in buckets.</param>
      /// <param name="bucketSize">The size of the buckets.</param>
      /// <typeparam name="T">Type of the item.</typeparam>
      /// <returns>An collection of buckets.</returns>
      /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
      /// <exception cref="ArgumentOutOfRangeException"><paramref name="bucketSize"/> is less than 1.</exception>
      public static IEnumerable<IEnumerable<T>> SplitInBuckets<T>(
         this IEnumerable<T> collection,
         int bucketSize)
      {
         if (collection is null)
            throw new ArgumentNullException(nameof(collection));
         if (bucketSize < 1)
            throw new ArgumentOutOfRangeException(nameof(bucketSize), "Bucket size cannot be less than 1.");

         var bucket = new List<T>();

         foreach (var item in collection)
         {
            bucket.Add(item);

            if (bucket.Count == bucketSize)
            {
               yield return bucket;
               bucket = new List<T>();
            }
         }

         if (bucket.Count != 0)
            yield return bucket;
      }
   }
}
