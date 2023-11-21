using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// JSON converter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
public sealed class ValueObjectJsonConverter<T, TKey, TValidationError> : JsonConverter<T>
   where T : IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));

   private readonly JsonConverter<TKey> _keyConverter;

   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectJsonConverter{T,TKey,TValidationError}"/>.
   /// </summary>
   /// <param name="options">JSON serializer options.</param>
   public ValueObjectJsonConverter(JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(options);

      _keyConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));
   }

   /// <inheritdoc />
   public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      var key = _keyConverter.Read(ref reader, typeof(TKey), options);

      if (key is null)
         return default;

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
