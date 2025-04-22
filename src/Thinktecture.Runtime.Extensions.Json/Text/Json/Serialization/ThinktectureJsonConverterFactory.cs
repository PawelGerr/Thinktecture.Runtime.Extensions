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

      var type = GetObjectType(typeToConvert);

      if (type is null)
         return false;

      if (!_skipObjectsWithJsonConverterAttribute)
         return true;

      var jsonConverterAttribute = type.GetCustomAttribute<JsonConverterAttribute>();

      if (jsonConverterAttribute is null)
         return true;

      if (jsonConverterAttribute.ConverterType == typeof(ThinktectureJsonConverterFactory))
         return true;

      return false;
   }

   private static Type? GetObjectType(Type typeToConvert)
   {
      // typeToConvert could be derived type (like nested Smart Enum)
      var metadata = MetadataLookup.Find(typeToConvert);

      if (metadata is Metadata.Keyed)
         return metadata.Type;

      if (typeToConvert.GetCustomAttributes<ObjectFactoryAttribute>().Any(a => a.UseForSerialization.HasFlag(SerializationFrameworks.SystemTextJson)))
         return typeToConvert;

      return null;
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      // typeToConvert could be derived type (like nested Smart Enum)
      var metadata = MetadataLookup.Find(typeToConvert) as Metadata.Keyed;
      var type = metadata?.Type ?? typeToConvert;

      var customFactory = type.GetCustomAttributes<ObjectFactoryAttribute>()
                              .LastOrDefault(a => a.UseForSerialization.HasFlag(SerializationFrameworks.SystemTextJson));

      Type keyType;

      if (customFactory is not null)
      {
         keyType = customFactory.Type;
      }
      else if (metadata is not null)
      {
         keyType = metadata.KeyType;
      }
      else
      {
         throw new InvalidOperationException($"No metadata for provided type '{typeToConvert.Name}' found.");
      }

      var validationErrorType = metadata?.ValidationErrorType ?? type.GetCustomAttribute<ValidationErrorAttribute>()?.Type ?? typeof(ValidationError);

      var converterType = keyType == typeof(string)
                             ? typeof(ThinktectureJsonConverter<,>).MakeGenericType(type, validationErrorType)
                             : typeof(ThinktectureJsonConverter<,,>).MakeGenericType(type, keyType, validationErrorType);
      var converter = Activator.CreateInstance(converterType, options);

      return (JsonConverter)(converter ?? throw new Exception($"Could not create converter of type '{converterType.Name}'."));
   }
}
