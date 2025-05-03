using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// JSON converter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'ThinktectureJsonConverterFactory' instead.")]
public sealed class ValueObjectJsonConverter<T, TKey, TValidationError> : ThinktectureJsonConverter<T, TKey, TValidationError>
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="options">JSON serializer options.</param>
   public ValueObjectJsonConverter(JsonSerializerOptions options)
      : base(options)
   {
   }
}

/// <summary>
/// JSON converter for string-based Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'ThinktectureJsonConverterFactory' instead.")]
public sealed class ValueObjectJsonConverter<T, TValidationError> : ThinktectureJsonConverter<T, TValidationError>
   where T : IObjectFactory<T, string, TValidationError>, IConvertible<string>
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="options">JSON serializer options.</param>
   public ValueObjectJsonConverter(JsonSerializerOptions options)
      : base(options)
   {
   }
}

/// <summary>
/// JSON converter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureJsonConverter<T, TKey, TValidationError> : JsonConverter<T>
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));
   private static readonly bool _disallowDefaultValues = typeof(IDisallowDefaultValue).IsAssignableFrom(typeof(T));

   private readonly JsonConverter<TKey> _keyConverter;

   /// <summary>
   /// Initializes a new instance of <see cref="ThinktectureJsonConverter{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="options">JSON serializer options.</param>
   public ThinktectureJsonConverter(JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(options);

      _keyConverter = (JsonConverter<TKey>)options.GetCustomMemberConverter(typeof(TKey));
   }

   /// <inheritdoc />
   public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      var key = _keyConverter.Read(ref reader, typeof(TKey), options);

      if (key is null)
      {
         if (_disallowDefaultValues)
            throw new JsonException($"Cannot convert null to type \"{typeof(T).Name}\" because it doesn't allow default values.");

         return default;
      }

      var validationError = T.Validate(key, null, out var obj);

      if (validationError is not null && !_mayReturnInvalidObjects)
         throw new JsonException(validationError.ToString() ?? "JSON deserialization failed.");

      return obj;
   }

   /// <inheritdoc />
   public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      _keyConverter.Write(writer, value.ToValue(), options);
   }
}

/// <summary>
/// JSON converter for string-based Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureJsonConverter<T, TValidationError> : JsonConverter<T>
   where T : IObjectFactory<T, string, TValidationError>, IConvertible<string>
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));
   private static readonly bool _disallowDefaultValues = typeof(IDisallowDefaultValue).IsAssignableFrom(typeof(T));

   /// <summary>
   /// Initializes a new instance of <see cref="ThinktectureJsonConverter{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="options">JSON serializer options.</param>
   public ThinktectureJsonConverter(JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(options);
   }

   /// <inheritdoc />
   public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      var key = reader.GetString();

      if (key is null)
      {
         if (_disallowDefaultValues)
            throw new JsonException($"Cannot convert null to type \"{typeof(T).Name}\" because it doesn't allow default values.");

         return default;
      }

      var validationError = T.Validate(key, null, out var obj);

      if (validationError is not null && !_mayReturnInvalidObjects)
         throw new JsonException(validationError.ToString() ?? "JSON deserialization failed.");

      return obj;
   }

   /// <inheritdoc />
   public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      writer.WriteStringValue(value.ToValue());
   }

   /// <inheritdoc />
   public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return Read(ref reader, typeToConvert, options) ?? base.ReadAsPropertyName(ref reader, typeToConvert, options);
   }

   /// <inheritdoc />
   public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      writer.WritePropertyName(value.ToValue());
   }
}
