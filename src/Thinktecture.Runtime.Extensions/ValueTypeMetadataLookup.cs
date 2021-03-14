using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Thinktecture.Internal;

namespace Thinktecture
{
   /// <summary>
   /// Provides lookup for implementations of <see cref="IEnum{TKey}"/>.
   /// </summary>
   public static class ValueTypeMetadataLookup
   {
      private static readonly ConcurrentDictionary<Type, ValueTypeMetadata> _metadata = new();

      /// <summary>
      /// Searches for <see cref="ValueTypeMetadata"/> for provided <paramref name="type"/>.
      /// </summary>
      /// <param name="type">Type to search for <see cref="ValueTypeMetadata"/> for.</param>
      /// <returns>An instance of <see cref="ValueTypeMetadata"/> if the <paramref name="type"/> is an value type; otherwise <c>null</c>.</returns>
      /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <c>null</c>.</exception>
      public static ValueTypeMetadata? Find(Type type)
      {
         if (type is null)
            throw new ArgumentNullException(nameof(type));

         type = Nullable.GetUnderlyingType(type) ?? type;

         if (type.IsPrimitive)
            return null;

         if (_metadata.TryGetValue(type, out var metadata))
            return metadata;

         if (type.GetCustomAttribute<KeyedValueTypeAttribute>() is null)
            return null;

         // The initializer of the assembly/module containing the enum/value type is not executed yet
         RuntimeHelpers.RunModuleConstructor(type.Assembly.ManifestModule.ModuleHandle);
         _metadata.TryGetValue(type, out metadata);

         return metadata;
      }

      /// <summary>
      /// Adds metadata of a value type to lookup.
      /// </summary>
      /// <param name="type">Implementation type of the value type.</param>
      /// <param name="valueTypeMetadata">Metadata of the value type.</param>
      /// <exception cref="ArgumentNullException">If <paramref name="type"/> of <paramref name="valueTypeMetadata"/> is <c>null</c>.</exception>
      /// <exception cref="ArgumentException">The <paramref name="type"/> has been added to lookup already.</exception>
      public static void AddMetadata(Type type, ValueTypeMetadata valueTypeMetadata)
      {
         if (type is null)
            throw new ArgumentNullException(nameof(type));
         if (valueTypeMetadata is null)
            throw new ArgumentNullException(nameof(valueTypeMetadata));

         if (!_metadata.TryAdd(type, valueTypeMetadata))
            throw new ArgumentException($"Multiple metadata instances for type '{type.FullName}'.");
      }
   }
}
