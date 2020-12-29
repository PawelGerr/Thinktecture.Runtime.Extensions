using System;
using System.Collections.Concurrent;

namespace Thinktecture
{
   /// <summary>
   /// Provides lookup for implementations of <see cref="IEnum{TKey}"/>.
   /// </summary>
   public static class EnumMetadataLookup
   {
      private static readonly ConcurrentDictionary<Type, EnumMetadata> _enumMetadata = new();

      /// <summary>
      /// Searches for <see cref="EnumMetadata"/> for provided <paramref name="type"/>.
      /// </summary>
      /// <param name="type">Type to search for <see cref="EnumMetadata"/> for.</param>
      /// <returns>An instance of <see cref="EnumMetadata"/> if the <paramref name="type"/> is a valid implementation of <see cref="IEnum{TKey}"/>; otherwise <c>null</c>.</returns>
      /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <c>null</c>.</exception>
      public static EnumMetadata? FindEnum(Type type)
      {
         if (type is null)
            throw new ArgumentNullException(nameof(type));

         return _enumMetadata.TryGetValue(type, out var metadata) ? metadata : null;
      }

      /// <summary>
      /// Adds metadata of an implementation of <see cref="IEnum{TKey}"/> to lookup.
      /// </summary>
      /// <param name="type">Implementation of <see cref="IEnum{TKey}"/>.</param>
      /// <param name="enumMetadata">Metadata of an enumeration.</param>
      /// <exception cref="ArgumentNullException">If <paramref name="type"/> of <paramref name="enumMetadata"/> is <c>null</c>.</exception>
      /// <exception cref="ArgumentException">The <paramref name="type"/> has been added to lookup already.</exception>
      public static void AddEnumMetadata(Type type, EnumMetadata enumMetadata)
      {
         if (type is null)
            throw new ArgumentNullException(nameof(type));
         if (enumMetadata is null)
            throw new ArgumentNullException(nameof(enumMetadata));

         if (!_enumMetadata.TryAdd(type, enumMetadata))
            throw new ArgumentException($"Multiple enum metadata for type '{type.FullName}'.");
      }
   }
}
