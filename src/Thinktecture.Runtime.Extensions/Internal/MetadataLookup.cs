using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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
   private static readonly ConcurrentDictionary<Type, (Type OwningType, IReadOnlyList<ObjectFactoryMetadata> Factories)> _objectFactories = new();

   /// <summary>
   /// Searches for <see cref="Metadata"/> for provided <paramref name="type"/>.
   /// </summary>
   /// <param name="type">Type to search <see cref="Metadata"/> for.</param>
   /// <returns>An instance of <see cref="Metadata"/> if the <paramref name="type"/> has metadata; otherwise <c>null</c>.</returns>
   public static Metadata? Find(Type? type)
   {
      if (!IsCandidate(type))
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
      if (!IsCandidate(type))
         return null;

      type = UnwrapNullable(type);
      var metadata = Find(type);

      // If metadata was found, use it directly. The metadata may belong to a base type, for example when the
      // provided type is the runtime type of an item of a derived (nested) Smart Enum. In that case the conversion
      // metadata is built from the metadata's own type, because that is the type the converters are created for.
      if (metadata is not null)
      {
         var metadataType = metadata.Type;
         var keyedMetadata = metadata as Metadata.Keyed;

         if (keyedMetadata is not null && !metadataFilter(keyedMetadata))
            keyedMetadata = null;

         // Object factories have priority over metadata
         var metadataFromFactory = GetConversionMetadata(metadataType, keyedMetadata, metadata.ObjectFactories, objectFactoryFilter);

         if (metadataFromFactory is not null)
            return metadataFromFactory;

         // No fallback to the standalone object-factory search below: a type implementing IMetadataOwner
         // surfaces all of its object factories via metadata.ObjectFactories, so the reflection-based
         // search cannot find any additional factories.
         if (keyedMetadata is null)
            return null;

         var (fromCtor, fromFactory) = GetFromExpressions(keyedMetadata);

         return new ConversionMetadata(
            metadataType,
            keyedMetadata.KeyType,
            keyedMetadata.ValidationErrorType,
            fromCtor,
            fromFactory);
      }

      // Otherwise, we search for object factories
      if (!_objectFactories.TryGetValue(type, out var objectFactories))
      {
         var (owningType, factories) = type.FindObjectFactoryMetadata();

         if (factories.Count == 0)
            return null;

         objectFactories = (owningType!, factories);
         _objectFactories.TryAdd(type, objectFactories);
      }

      // The conversion metadata must use the type that actually owns the object factories (and their generated
      // IObjectFactory<T, ...> interface), not the requested type. For a derived class of a standalone [ObjectFactory]
      // type the factories are declared on the base type, so consumers' MakeGenericType calls would otherwise violate
      // the invariant IObjectFactory<T, ...> constraint and throw ArgumentException. This mirrors the keyed branch above,
      // which builds the conversion metadata from metadata.Type.
      return GetConversionMetadata(objectFactories.OwningType, null, objectFactories.Factories, objectFactoryFilter);
   }

   private static bool IsCandidate([NotNullWhen(true)] Type? type)
   {
      return type is { IsPrimitive: false, IsArray: false, IsEnum: false, IsPointer: false };
   }

   private static (LambdaExpression? FromCtor, LambdaExpression? FromFactory) GetFromExpressions(Metadata.Keyed keyedMetadata)
   {
      return keyedMetadata.Switch(
         smartEnum: m => (null, m.ConvertFromKeyExpression),
         valueObject: m => ((LambdaExpression?)m.ConvertFromKeyExpressionViaConstructor, m.ConvertFromKeyExpression));
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

      return new ConversionMetadata(
         type,
         objectFactory.ValueType,
         objectFactory.ValidationErrorType,
         objectFactory.ConvertFromKeyExpressionViaConstructor ?? fromCtor,
         fromFactory);
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
                   ?? throw new InvalidOperationException($"Could not retrieve metadata for type '{typeToCheck.FullName}'.");
         }

         typeToCheck = typeToCheck.BaseType;
      }

      throw new InvalidOperationException($"Could not retrieve metadata for type '{type.FullName}'.");
   }
}
