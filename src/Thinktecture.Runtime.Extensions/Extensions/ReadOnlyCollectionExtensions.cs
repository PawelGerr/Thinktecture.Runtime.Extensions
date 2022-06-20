namespace Thinktecture;

/// <summary>
/// Extensions for <see cref="IReadOnlyCollection{T}"/>.
/// </summary>
public static class ReadOnlyCollectionExtensions
{
   /// <summary>
   /// Projects and wraps the provided <paramref name="items"/> and exposes the projected values as <see cref="IReadOnlyCollection{TResult}"/>.
   /// </summary>
   /// <param name="items">Items to project to <see cref="IReadOnlyCollection{TResult}"/>.</param>
   /// <param name="selector">Returns a item of type <typeparamref name="TResult"/> for each item of type <typeparamref name="T"/>.</param>
   /// <typeparam name="T">Item type.</typeparam>
   /// <typeparam name="TResult">Result type.</typeparam>
   /// <returns>A collection of type <see cref="IReadOnlyCollection{TResult}"/>.</returns>
   /// <exception cref="ArgumentNullException">
   ///   <paramref name="items"/> or <paramref name="selector"/> is <c>null</c>.
   /// </exception>
   public static IReadOnlyCollection<TResult> ToReadOnlyCollection<T, TResult>(this IReadOnlyCollection<T> items, Func<T, TResult> selector)
   {
      return items.Select(selector).ToReadOnlyCollection(items.Count);
   }

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
#endif
}
