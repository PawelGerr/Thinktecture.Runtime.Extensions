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
      // unless the user opted out via the skipSpanBasedDeserialization callback
      if (CanUseSpanParsableConverter(metadata.Value.Type) && _skipSpanBasedDeserialization?.Invoke(metadata.Value.Type) != true)
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
   private static bool CanUseSpanParsableConverter(Type type)
   {
      // ThinktectureSpanParsableJsonConverter requires:
      // - IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      // - IConvertible<ReadOnlySpan<char>>
      //
      // Types that support this:
      // 1. String-based Smart Enums - implement the interfaces directly (not tracked in metadata)
      // 2. Value Objects with [ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)] - tracked in ObjectFactories metadata

      var metadata = MetadataLookup.Find(type);

      if (metadata is null)
         return false;

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
#endif
}
