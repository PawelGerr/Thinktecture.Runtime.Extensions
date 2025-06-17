using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public static class MetadataLookup
{
   private static readonly ConcurrentDictionary<Type, Metadata> _metadata = new();
   private static readonly ConcurrentDictionary<Type, IReadOnlyList<ObjectFactoryMetadata>> _objectFactories = new();

   /// <summary>
   /// Searches for <see cref="Metadata"/> for provided <paramref name="type"/>.
   /// </summary>
   /// <param name="type">Type to search <see cref="Metadata"/> for.</param>
   /// <returns>An instance of <see cref="Metadata"/> if the <paramref name="type"/> has metadata; otherwise <c>null</c>.</returns>
   public static Metadata? Find(Type? type)
   {
      if (type is null)
         return null;

      type = UnwrapNullable(type);

      if (_metadata.TryGetValue(type, out var metadata))
         return metadata;

      if (!typeof(IMetadataOwner).IsAssignableFrom(type))
         return null;

      metadata = SearchBaseTypesForMetadata(type);
      _metadata.TryAdd(type, metadata);

      return metadata;
   }

   /// <summary>
   /// Searches for <see cref="ConversionMetadata"/> for the provided <paramref name="type"/>.
   /// </summary>
   /// <param name="type">Type to search <see cref="ConversionMetadata"/> for.</param>
   /// <param name="objectFactoryFilter">A filter to select the appropriate <see cref="ObjectFactoryMetadata"/>.</param>
   /// <param name="metadataFilter">A filter to select the appropriate <see cref="Metadata.Keyed"/>.</param>
   /// <returns>
   /// An instance of <see cref="ConversionMetadata"/> if the <paramref name="type"/> has conversion metadata; otherwise, <c>null</c>.
   /// </returns>
   public static ConversionMetadata? FindMetadataForConversion(
      Type? type,
      Func<ObjectFactoryMetadata, bool> objectFactoryFilter,
      Func<Metadata.Keyed, bool> metadataFilter)
   {
      if (type is null)
         return null;

      type = UnwrapNullable(type);
      var metadata = Find(type);

      // If the provided type equals the metadata type, then we can use it directly
      if (metadata?.Type == type)
      {
         var keyedMetadata = metadata as Metadata.Keyed;

         if (keyedMetadata is not null && !metadataFilter(keyedMetadata))
            keyedMetadata = null;

         // Object factories have priority over metadata
         var metadataFromFactory = GetConversionMetadata(type, keyedMetadata, metadata.ObjectFactories, objectFactoryFilter);

         if (metadataFromFactory is not null)
            return metadataFromFactory;

         if (keyedMetadata is null)
            return null;

         var (fromCtor, fromFactory) = GetFromExpressions(keyedMetadata);

         return new ConversionMetadata(
            type,
            keyedMetadata.KeyType,
            keyedMetadata.ValidationErrorType,
            fromCtor,
            fromFactory);
      }

      // Otherwise, we search for object factories
      if (!_objectFactories.TryGetValue(type, out var objectFactories))
      {
         objectFactories = type.FindObjectFactoryMetadata();

         if (objectFactories.Count == 0)
            return null;

         _objectFactories.TryAdd(type, objectFactories);
      }

      return GetConversionMetadata(type, null, objectFactories, objectFactoryFilter);
   }

   private static (LambdaExpression? FromCtor, LambdaExpression? FromFactory) GetFromExpressions(Metadata.Keyed keyedMetadata)
   {
      return keyedMetadata.Switch(
         keyedSmartEnum: m => (null, m.ConvertFromKeyExpression),
         keyedValueObject: m => ((LambdaExpression?)m.ConvertFromKeyExpressionViaConstructor, m.ConvertFromKeyExpression));
   }

   private static ConversionMetadata? GetConversionMetadata(
      Type type,
      Metadata.Keyed? keyedObject,
      IReadOnlyList<ObjectFactoryMetadata> objectFactories,
      Func<ObjectFactoryMetadata, bool> objectFactoryFilter)
   {
      var objectFactory = objectFactories.LastOrDefault(objectFactoryFilter);

      if (objectFactory is null)
         return null;

      // If it is a keyed value object, then we have a constructor as well
      var (fromCtor, fromFactory) = keyedObject?.KeyType == objectFactory.ValueType
                                       ? GetFromExpressions(keyedObject)
                                       : (null, null);

      return new ConversionMetadata(type, objectFactory.ValueType, objectFactory.ValidationErrorType, fromCtor, fromFactory);
   }

   private static Type UnwrapNullable(Type type)
   {
      return type.IsClass ? type : Nullable.GetUnderlyingType(type) ?? type;
   }

   private static Metadata SearchBaseTypesForMetadata(Type type)
   {
      var typeToCheck = type;

      while (typeToCheck is not null)
      {
         if (_metadata.TryGetValue(typeToCheck, out var metadata))
            return metadata;

         var property = typeToCheck.GetProperty(
            "global::Thinktecture.Internal.IMetadataOwner.Metadata",
            BindingFlags.Static | BindingFlags.NonPublic);

         if (property is not null)
         {
            return (Metadata?)property.GetValue(null)
                   ?? throw new InvalidOperationException($"Could not retrieve metadata for type '{type.FullName}'.");
         }

         typeToCheck = typeToCheck.BaseType;
      }

      throw new Exception($"Could not retries metadata for type '{type.FullName}'.");
   }
}
