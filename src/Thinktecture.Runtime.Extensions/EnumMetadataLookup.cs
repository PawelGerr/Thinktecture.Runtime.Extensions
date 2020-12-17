using System;
using System.Collections.Concurrent;

namespace Thinktecture
{
   public static class EnumMetadataLookup
   {
      private static readonly ConcurrentDictionary<Type, EnumMetadata> _enumMetadata = new();

      public static EnumMetadata? FindEnum(Type concreteType)
      {
         if (concreteType is null)
            throw new ArgumentNullException(nameof(concreteType));

         return _enumMetadata.TryGetValue(concreteType, out var metadata) ? metadata : null;
      }

      public static void AddEnumMetadata(Type concreteType, EnumMetadata enumMetadata)
      {
         if (concreteType is null)
            throw new ArgumentNullException(nameof(concreteType));
         if (enumMetadata is null)
            throw new ArgumentNullException(nameof(enumMetadata));

         if (!_enumMetadata.TryAdd(concreteType, enumMetadata))
            throw new Exception($"Multiple enum metadata for type '{concreteType.FullName}'.");
      }
   }
}
