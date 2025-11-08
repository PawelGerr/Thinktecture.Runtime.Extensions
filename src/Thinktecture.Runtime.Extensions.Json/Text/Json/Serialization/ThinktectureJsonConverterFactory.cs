using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
/// </summary>
[Obsolete("Use 'ThinktectureJsonConverterFactory' instead.")]
public sealed class ValueObjectJsonConverterFactory<T, TKey, TValidationError> : ThinktectureJsonConverterFactory<T, TKey, TValidationError>
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>;

/// <summary>
/// Factory for creation of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
/// </summary>
[Obsolete("Use 'ThinktectureJsonConverterFactory' instead.")]
public sealed class ValueObjectJsonConverterFactory<T, TValidationError> : ThinktectureJsonConverterFactory<T, TValidationError>
   where T : IObjectFactory<T, string, TValidationError>, IConvertible<string>
   where TValidationError : class, IValidationError<TValidationError>;

/// <summary>
/// Factory for creation of <see cref="ThinktectureJsonConverterFactory{T,TKey,TValidationError}"/>.
/// </summary>
[Obsolete("Use 'ThinktectureJsonConverterFactory' instead.")]
public sealed class ValueObjectJsonConverterFactory : ThinktectureJsonConverterFactory
{
   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectJsonConverterFactory"/>.
   /// </summary>
   public ValueObjectJsonConverterFactory()
      : this(true)
   {
   }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectJsonConverterFactory"/>.
   /// </summary>
   /// <param name="skipObjectsWithJsonConverterAttribute">
   /// Indication whether to skip value objects with <see cref="JsonConverterAttribute"/>.
   /// </param>
   public ValueObjectJsonConverterFactory(
      bool skipObjectsWithJsonConverterAttribute)
      : base(skipObjectsWithJsonConverterAttribute)
   {
   }
}

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
/// Factory for creation of <see cref="ThinktectureJsonConverterFactory{T,TKey,TValidationError}"/>.
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
   {
      _skipObjectsWithJsonConverterAttribute = skipObjectsWithJsonConverterAttribute;
   }

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

      var converterType = metadata.Value.KeyType == typeof(string)
                             ? typeof(ThinktectureJsonConverter<,>).MakeGenericType(metadata.Value.Type, metadata.Value.ValidationErrorType)
                             : typeof(ThinktectureJsonConverter<,,>).MakeGenericType(metadata.Value.Type, metadata.Value.KeyType, metadata.Value.ValidationErrorType);
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
}
