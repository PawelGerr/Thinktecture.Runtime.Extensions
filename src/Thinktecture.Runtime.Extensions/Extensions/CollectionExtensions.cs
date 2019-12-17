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
      /// Adds all items of <paramref name="collection"/> to <paramref name="destination"/>.
      /// </summary>
      /// <param name="destination">Collection to add items to.</param>
      /// <param name="collection">Collection to read items from.</param>
      /// <typeparam name="TOutputCollection">Type of the destination collection.</typeparam>
      /// <typeparam name="TItem">Type of the collection item.</typeparam>
      /// <exception cref="ArgumentNullException">
      /// <paramref name="destination"/> is <c>null</c>
      /// - or <paramref name="collection"/> is <c>null</c>.
      /// </exception>
      public static void Add<TOutputCollection, TItem>(
         this TOutputCollection destination,
         IEnumerable<TItem> collection)
         where TOutputCollection : class, ICollection<TItem>
      {
         if (destination == null)
            throw new ArgumentNullException(nameof(destination));
         if (collection == null)
            throw new ArgumentNullException(nameof(collection));

         foreach (var item in collection)
         {
            destination.Add(item);
         }
      }
   }
}
