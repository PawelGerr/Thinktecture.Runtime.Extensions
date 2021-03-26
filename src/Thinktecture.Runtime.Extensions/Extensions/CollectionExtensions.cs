using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   /// <summary>
   /// Extensions for <see cref="ICollection{T}"/>
   /// </summary>
   public static class CollectionExtensions
   {
      /// <summary>
      /// Adds all items of <paramref name="source"/> to <paramref name="collection"/>.
      /// </summary>
      /// <param name="collection">Collection to add items to.</param>
      /// <param name="source">Collection to read items from.</param>
      /// <typeparam name="TOutputCollection">Type of the destination collection.</typeparam>
      /// <typeparam name="TItem">Type of the collection item.</typeparam>
      /// <returns>The provided <paramref name="collection"/> for chaining.</returns>
      /// <exception cref="ArgumentNullException">
      /// <paramref name="collection"/> is <c>null</c>
      /// - or <paramref name="source"/> is <c>null</c>.
      /// </exception>
      public static TOutputCollection Add<TOutputCollection, TItem>(
         this TOutputCollection collection,
         IEnumerable<TItem> source)
         where TOutputCollection : class, ICollection<TItem>
      {
         if (collection == null)
            throw new ArgumentNullException(nameof(collection));
         if (source == null)
            throw new ArgumentNullException(nameof(source));

         foreach (var item in source)
         {
            collection.Add(item);
         }

         return collection;
      }

      /// <summary>
      /// Adds the <paramref name="item"/> to <see cref="collection"/>
      /// and return the provided <paramref name="item"/>.
      /// </summary>
      /// <param name="collection">Collection to add items to.</param>
      /// <param name="item">Item to add to collection.</param>
      /// <typeparam name="TOutputCollection">Type of the destination collection.</typeparam>
      /// <typeparam name="TItem">Type of the collection item.</typeparam>
      /// <returns>The provided <paramref name="item"/>.</returns>
      /// <exception cref="ArgumentNullException">
      /// <paramref name="collection"/> is <c>null</c> is <c>null</c>.
      /// </exception>
      public static TItem AddReturn<TOutputCollection, TItem>(
         this TOutputCollection collection,
         TItem item)
         where TOutputCollection : class, ICollection<TItem>
      {
         if (collection == null)
            throw new ArgumentNullException(nameof(collection));

         collection.Add(item);

         return item;
      }

      /// <summary>
      /// Adds the <paramref name="item"/> to <see cref="collection"/>
      /// and return the provided <paramref name="collection"/> for chaining.
      /// </summary>
      /// <param name="collection">Collection to add items to.</param>
      /// <param name="item">Item to add to collection.</param>
      /// <typeparam name="TOutputCollection">Type of the destination collection.</typeparam>
      /// <typeparam name="TItem">Type of the collection item.</typeparam>
      /// <returns>The provided <paramref name="collection"/> for chaining.</returns>
      /// <exception cref="ArgumentNullException">
      /// <paramref name="collection"/> is <c>null</c> is <c>null</c>.
      /// </exception>
      public static TOutputCollection AddChain<TOutputCollection, TItem>(
         this TOutputCollection collection,
         TItem item)
         where TOutputCollection : class, ICollection<TItem>
      {
         if (collection == null)
            throw new ArgumentNullException(nameof(collection));

         collection.Add(item);

         return collection;
      }
   }
}
