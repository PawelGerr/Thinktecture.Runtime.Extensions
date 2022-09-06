using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Thinktecture.Internal;

/// <summary>
/// For internal use only.
/// </summary>
public static class ValueObjectMetadataLookup
{
   private static readonly ConcurrentDictionary<Type, ValueObjectMetadata> _metadata = new();

   /// <summary>
   /// Searches for <see cref="ValueObjectMetadata"/> for provided <paramref name="type"/>.
   /// </summary>
   /// <param name="type">Type to search for <see cref="ValueObjectMetadata"/> for.</param>
   /// <returns>An instance of <see cref="ValueObjectMetadata"/> if the <paramref name="type"/> is an value object; otherwise <c>null</c>.</returns>
   /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <c>null</c>.</exception>
   public static ValueObjectMetadata? Find(Type type)
   {
      if (type is null)
         throw new ArgumentNullException(nameof(type));

      type = Nullable.GetUnderlyingType(type) ?? type;

      if (type.IsPrimitive)
         return null;

      if (_metadata.TryGetValue(type, out var metadata))
         return metadata;

      if (type.GetCustomAttribute<KeyedValueObjectAttribute>() is null)
         return null;

      // The initializer of the assembly/module containing the enum/value object is not executed yet
      RuntimeHelpers.RunModuleConstructor(type.Assembly.ManifestModule.ModuleHandle);
      _metadata.TryGetValue(type, out metadata);

      return metadata;
   }

   /// <summary>
   /// Adds metadata of a value object to lookup.
   /// </summary>
   /// <param name="type">Implementation type of the value object.</param>
   /// <param name="valueObjectMetadata">Metadata of the value object.</param>
   /// <exception cref="ArgumentNullException">If <paramref name="type"/> of <paramref name="valueObjectMetadata"/> is <c>null</c>.</exception>
   /// <exception cref="ArgumentException">The <paramref name="type"/> has been added to lookup already.</exception>
   public static void AddMetadata(Type type, ValueObjectMetadata valueObjectMetadata)
   {
      if (type is null)
         throw new ArgumentNullException(nameof(type));
      if (valueObjectMetadata is null)
         throw new ArgumentNullException(nameof(valueObjectMetadata));

      if (!_metadata.TryAdd(type, valueObjectMetadata))
         throw new ArgumentException($"Multiple metadata instances for type '{type.FullName}'.");
   }
}
