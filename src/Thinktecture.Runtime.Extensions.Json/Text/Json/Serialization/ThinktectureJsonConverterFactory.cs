using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// Factory for creation of <see cref="ThinktectureJsonConverterFactory{T,TKey,TValidationError}"/>.
/// </summary>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureJsonConverterFactory<T, TKey, TValidationError> : JsonConverterFactory
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
      return typeof(T).IsAssignableFrom(typeToConvert);
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      return new ThinktectureJsonConverter<T, TKey, TValidationError>(options);
   }
}

/// <summary>
/// Factory for creation of <see cref="ThinktectureJsonConverter{T,TValidationError}"/>.
/// </summary>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureJsonConverterFactory<T, TValidationError> : JsonConverterFactory
   where T : IObjectFactory<T, string, TValidationError>, IConvertible<string>
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
      return typeof(T).IsAssignableFrom(typeToConvert);
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      return new ThinktectureJsonConverter<T, TValidationError>(options);
   }
}

/// <summary>
/// Factory for creation of <see cref="ThinktectureJsonConverterFactory{T,TKey,TValidationError}"/>.
/// </summary>
public class ThinktectureJsonConverterFactory : JsonConverterFactory
{
   private readonly bool _skipObjectsWithJsonConverterAttribute;
#if NET9_0_OR_GREATER
   private readonly Func<Type, bool>? _skipSpanBasedDeserialization;
#endif

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureJsonConverterFactory"/>.
   /// </summary>
   public ThinktectureJsonConverterFactory()
      : this(true)
   {
   }

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureJsonConverterFactory"/>.
   /// </summary>
   /// <param name="skipObjectsWithJsonConverterAttribute">
   /// Indication whether to skip value objects with <see cref="JsonConverterAttribute"/>.
   /// </param>
   public ThinktectureJsonConverterFactory(
      bool skipObjectsWithJsonConverterAttribute)
#if NET9_0_OR_GREATER
      : this(skipObjectsWithJsonConverterAttribute, null)
   {
   }

   /// <summary>
   /// Initializes new instance of <see cref="ThinktectureJsonConverterFactory"/>.
   /// </summary>
   /// <param name="skipObjectsWithJsonConverterAttribute">
   /// Indication whether to skip value objects with <see cref="JsonConverterAttribute"/>.
   /// </param>
   /// <param name="skipSpanBasedDeserialization">
   /// Optional callback to determine whether to skip zero-allocation span-based deserialization for a specific type.
   /// When this callback returns <c>true</c> for a type, the regular key-type-based converter will be used instead.
   /// This callback is only invoked for types that support span-based deserialization.
   /// The callback is ignored for types whose only System.Text.Json conversion mechanism is a
   /// <see cref="ReadOnlySpan{T}"/> of <see cref="char"/> object factory, because no regular key-type-based
   /// converter can handle such a type; the span-based converter is kept for them even when the callback returns <c>true</c>.
   /// </param>
   public ThinktectureJsonConverterFactory(
      bool skipObjectsWithJsonConverterAttribute,
      Func<Type, bool>? skipSpanBasedDeserialization)
   {
      _skipObjectsWithJsonConverterAttribute = skipObjectsWithJsonConverterAttribute;
      _skipSpanBasedDeserialization = skipSpanBasedDeserialization;
   }
#else
   {
      _skipObjectsWithJsonConverterAttribute = skipObjectsWithJsonConverterAttribute;
   }
#endif

   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
      // Handling of Nullable<T> should be done by System.Text.Json
      if (typeToConvert.IsValueType && Nullable.GetUnderlyingType(typeToConvert) is not null)
         return false;

      var metadata = FindMetadataForConversion(typeToConvert);

      if (metadata is null)
         return false;

      if (!_skipObjectsWithJsonConverterAttribute)
         return true;

      var jsonConverterAttribute = metadata.Value.Type.GetCustomAttribute<JsonConverterAttribute>();

      if (jsonConverterAttribute is null)
         return true;

      if (jsonConverterAttribute.ConverterType == typeof(ThinktectureJsonConverterFactory))
         return true;

      return false;
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      var metadata = FindMetadataForConversion(typeToConvert);

      if (metadata is null)
         throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");

      Type converterType;

#if NET9_0_OR_GREATER
      // Use zero-allocation span-based converter if the type supports IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      // unless the user opted out via the skipSpanBasedDeserialization callback. The opt-out is ignored when the type
      // cannot be handled by the regular converter (e.g. a type whose only mechanism is a ReadOnlySpan<char> object
      // factory), because the span-based converter is then the only option.
      if (CanUseSpanParsableConverter(metadata.Value)
          && (_skipSpanBasedDeserialization?.Invoke(metadata.Value.Type) != true || !CanUseRegularConverter(metadata.Value)))
      {
         converterType = typeof(ThinktectureSpanParsableJsonConverter<,>).MakeGenericType(metadata.Value.Type, metadata.Value.ValidationErrorType);
      }
      else
#endif
      {
         converterType = metadata.Value.KeyType == typeof(string) || metadata.Value.KeyType == typeof(ReadOnlySpan<char>)
                            ? typeof(ThinktectureJsonConverter<,>).MakeGenericType(metadata.Value.Type, metadata.Value.ValidationErrorType)
                            : typeof(ThinktectureJsonConverter<,,>).MakeGenericType(metadata.Value.Type, metadata.Value.KeyType, metadata.Value.ValidationErrorType);
      }

      var converter = Activator.CreateInstance(converterType, options);

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }

   private static ConversionMetadata? FindMetadataForConversion(Type typeToConvert)
   {
      return MetadataLookup.FindMetadataForConversion(
         typeToConvert,
         f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.SystemTextJson),
         _ => true);
   }

#if NET9_0_OR_GREATER
   private static bool CanUseSpanParsableConverter(ConversionMetadata conversionMetadata)
   {
      // ThinktectureSpanParsableJsonConverter requires:
      // - IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      // - IConvertible<ReadOnlySpan<char>>
      //
      // Types that support this:
      // 1. String-based Smart Enums - implement the interfaces directly (not tracked in metadata)
      // 2. Value Objects with [ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)] - tracked in ObjectFactories metadata

      // FindMetadataForConversion already applied object-factory priority, so the resolved KeyType is the value type
      // to serialize. If an object factory with a non-string value type has priority (e.g. a string-keyed Smart Enum
      // with [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson)]), the span-based
      // converter must not be used, because it would serialize the string key instead of the object factory value.
      if (conversionMetadata.KeyType != typeof(string) && conversionMetadata.KeyType != typeof(ReadOnlySpan<char>))
         return false;

      // The span interfaces are generated inside #if NET9_0_OR_GREATER using the target framework of the DEFINING
      // assembly, but the metadata (e.g. DisableSpanBasedJsonConversion, object-factory value type) is emitted
      // unconditionally. A type compiled for net8.0 but consumed by a net9.0+ application therefore reports span
      // capability in its metadata without implementing the interfaces. Verify the concrete type actually implements
      // IConvertible<ReadOnlySpan<char>> (emitted together with IObjectFactory<T, ReadOnlySpan<char>, TValidationError>),
      // otherwise constructing the span-based converter would throw an ArgumentException.
      if (!typeof(IConvertible<ReadOnlySpan<char>>).IsAssignableFrom(conversionMetadata.Type))
         return false;

      var metadata = MetadataLookup.Find(conversionMetadata.Type);

      if (metadata is null)
      {
         // The type has no IMetadataOwner metadata, so it is a standalone [ObjectFactory] type that was resolved
         // through the object-factory fallback of FindMetadataForConversion. The resolved KeyType is the winning
         // object factory's value type. A ReadOnlySpan<char> value type means a span-based System.Text.Json factory
         // has priority, and the IConvertible<ReadOnlySpan<char>> check above already confirmed the type implements
         // the required interface, so the span-based converter is the correct (and only) choice for it.
         return conversionMetadata.KeyType == typeof(ReadOnlySpan<char>);
      }

      // String-based Smart Enums (unless DisableSpanBasedJsonConversion is set)
      if (metadata is Metadata.Keyed.SmartEnum smartEnum && smartEnum.KeyType == typeof(string))
      {
         return !smartEnum.DisableSpanBasedJsonConversion;
      }

      // Check ObjectFactories metadata (for types with [ObjectFactory<ReadOnlySpan<char>>])
      // Note: All ObjectFactories for a type share the same ValidationErrorType, so we only check ValueType
      for (var i = 0; i < metadata.ObjectFactories.Count; i++)
      {
         var factory = metadata.ObjectFactories[i];

         if (factory.UseForSerialization.HasSerializationFramework(SerializationFrameworks.SystemTextJson)
             && factory.ValueType == typeof(ReadOnlySpan<char>))
         {
            return true;
         }
      }

      return false;
   }

   private static bool CanUseRegularConverter(ConversionMetadata conversionMetadata)
   {
      // For a resolved KeyType of ReadOnlySpan<char>, CreateConverter selects ThinktectureJsonConverter<T, TValidationError>,
      // which requires the type to implement IConvertible<string>. A type whose only System.Text.Json mechanism is a
      // ReadOnlySpan<char> object factory (for example a complex Value Object with only [ObjectFactory<ReadOnlySpan<char>>])
      // does not implement IConvertible<string>, so it cannot be handled by the regular converter.
      if (conversionMetadata.KeyType == typeof(ReadOnlySpan<char>))
         return typeof(IConvertible<string>).IsAssignableFrom(conversionMetadata.Type);

      return true;
   }
#endif
}
