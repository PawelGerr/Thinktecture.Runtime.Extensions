using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Thinktecture.Internal;

/// <summary>
/// For internal use only.
/// </summary>
public static class KeyedValueObjectMetadataLookup
{
   private static readonly ConcurrentDictionary<Type, KeyedValueObjectMetadata> _metadata = new();

   /// <summary>
   /// Searches for <see cref="KeyedValueObjectMetadata"/> for provided <paramref name="type"/>.
   /// </summary>
   /// <param name="type">Type to search for <see cref="KeyedValueObjectMetadata"/>.</param>
   /// <returns>An instance of <see cref="KeyedValueObjectMetadata"/> if the <paramref name="type"/> is a keyed value object; otherwise <c>null</c>.</returns>
   /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <c>null</c>.</exception>
   public static KeyedValueObjectMetadata? Find(Type type)
   {
      ArgumentNullException.ThrowIfNull(type);

      type = Nullable.GetUnderlyingType(type) ?? type;

      if (_metadata.TryGetValue(type, out var metadata))
         return metadata;

      if (!typeof(IKeyedValueObject).IsAssignableFrom(type))
         return null;

      // The initializer of the assembly/module containing the smart enum/value object is not executed yet
      RuntimeHelpers.RunModuleConstructor(type.Assembly.ManifestModule.ModuleHandle);

      if (_metadata.TryGetValue(type, out metadata))
         return metadata;

      throw new Exception($"Could not retries metadata for type '{type.FullName}'.");
   }

   /// <summary>
   /// Adds metadata of a value object to lookup.
   /// </summary>
   /// <param name="type">Implementation type of the value object.</param>
   /// <param name="valueObjectMetadata">Metadata of the value object.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="type"/> of <paramref name="valueObjectMetadata"/> is <c>null</c>.</exception>
   /// <exception cref="ArgumentException">The <paramref name="type"/> has been added to lookup already.</exception>
   public static void AddMetadata(Type type, KeyedValueObjectMetadata valueObjectMetadata)
   {
      ArgumentNullException.ThrowIfNull(type);
      ArgumentNullException.ThrowIfNull(valueObjectMetadata);

      if (!_metadata.TryAdd(type, valueObjectMetadata))
         throw new ArgumentException($"Multiple metadata instances for type '{type.FullName}'.");
   }
}
