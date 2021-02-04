using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   /// <summary>
   /// Extensions for <see cref="IReadOnlyCollection{T}"/>.
   /// </summary>
   public static class ReadOnlyCollectionExtensions
   {
      /// <summary>
      /// Splits the <paramref name="collection"/> in buckets of provided <paramref name="bucketSize"/>.
      /// </summary>
      /// <param name="collection">Collection to split in buckets.</param>
      /// <param name="bucketSize">The size of the buckets.</param>
      /// <typeparam name="T">Type of the item.</typeparam>
      /// <returns>An collection of buckets.</returns>
      /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
      /// <exception cref="ArgumentOutOfRangeException"><paramref name="bucketSize"/> is less than 1.</exception>
      public static IEnumerable<IReadOnlyCollection<T>> SplitInBuckets<T>(
         this IReadOnlyCollection<T> collection,
         int bucketSize)
      {
         if (collection is null)
            throw new ArgumentNullException(nameof(collection));
         if (bucketSize < 1)
            throw new ArgumentOutOfRangeException(nameof(bucketSize), "Bucket size cannot be less than 1.");

         if (collection.Count == 0)
            yield break;

         if (collection.Count <= bucketSize)
         {
            yield return collection;
            yield break;
         }

         var bucketCount = collection.Count / bucketSize;

         using var enumerator = collection.GetEnumerator();

         for (var i = 0; i < bucketCount; i++)
         {
            var bucket = new T[bucketSize];

            for (var j = 0; j < bucketSize; j++)
            {
               enumerator.MoveNext();
               bucket[j] = enumerator.Current;
            }

            yield return bucket;
         }

         var lastBucketSize = collection.Count % bucketSize;

         if (lastBucketSize != 0)
         {
            var bucket = new T[lastBucketSize];

            for (var j = 0; j < lastBucketSize; j++)
            {
               enumerator.MoveNext();
               bucket[j] = enumerator.Current;
            }

            yield return bucket;
         }
      }
   }
}
