namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="IReadOnlyList{T}"/>
/// </summary>
public static class ReadOnlyListExtensions
{
#if !NET6_0
   /// <summary>
   /// Splits the <paramref name="collection"/> in buckets of provided <paramref name="bucketSize"/>.
   /// </summary>
   /// <param name="collection">Collection to split in buckets.</param>
   /// <param name="bucketSize">The size of the buckets.</param>
   /// <typeparam name="T">Type of the item.</typeparam>
   /// <returns>An collection of buckets.</returns>
   /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
   /// <exception cref="ArgumentOutOfRangeException"><paramref name="bucketSize"/> is less than 1.</exception>
   public static IEnumerable<IReadOnlyList<T>> SplitInBuckets<T>(
      this IReadOnlyList<T> collection,
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

      for (var i = 0; i < bucketCount; i++)
      {
         var collectionIndex = i * bucketSize;
         var bucket = new T[bucketSize];

         for (var j = 0; j < bucketSize; j++, collectionIndex++)
         {
            bucket[j] = collection[collectionIndex];
         }

         yield return bucket;
      }

      var lastBucketSize = collection.Count % bucketSize;

      if (lastBucketSize != 0)
      {
         var collectionIndex = bucketCount * bucketSize;
         var bucket = new T[lastBucketSize];

         for (var j = 0; j < lastBucketSize; j++, collectionIndex++)
         {
            bucket[j] = collection[collectionIndex];
         }

         yield return bucket;
      }
   }
#endif
}
