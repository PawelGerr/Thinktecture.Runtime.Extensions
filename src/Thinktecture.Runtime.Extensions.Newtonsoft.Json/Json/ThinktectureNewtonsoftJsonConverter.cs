using Newtonsoft.Json;

namespace Thinktecture.Json;

/// <summary>
/// <see cref="JsonConverter"/> for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'ThinktectureNewtonsoftJsonConverterFactory' instead.")]
public sealed class ValueObjectNewtonsoftJsonConverter<T, TKey, TValidationError> : ThinktectureNewtonsoftJsonConverter<T, TKey, TValidationError>
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>;

/// <summary>
/// <see cref="JsonConverter"/> for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureNewtonsoftJsonConverter<T, TKey, TValidationError> : JsonConverter
   where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly Type _type = typeof(T);
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));
   private static readonly bool _disallowDefaultValues = typeof(IDisallowDefaultValue).IsAssignableFrom(typeof(T));

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _type.IsAssignableFrom(objectType);
   }

   /// <inheritdoc />
   public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
   {
      ArgumentNullException.ThrowIfNull(writer);
      ArgumentNullException.ThrowIfNull(serializer);

      if (value is null)
      {
         writer.WriteNull();
      }
      else
      {
         serializer.Serialize(writer, ((T)value).ToValue());
      }
   }

   /// <inheritdoc />
   public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
   {
      ArgumentNullException.ThrowIfNull(reader);
      ArgumentNullException.ThrowIfNull(serializer);

      if (reader.TokenType == JsonToken.Null)
      {
         var isNullable = Nullable.GetUnderlyingType(objectType) == _type;

         if (isNullable || (objectType.IsClass && !_disallowDefaultValues))
            return null;
      }

      var key = serializer.Deserialize<TKey>(reader);

      if (key is null)
      {
         if (_disallowDefaultValues)
            throw new JsonException($"Cannot convert null to type \"{typeof(T).Name}\" because it doesn't allow default values.");

         return null;
      }

      var validationError = T.Validate(key, null, out var obj);

      if (validationError is not null && !_mayReturnInvalidObjects)
         throw new JsonSerializationException(validationError.ToString() ?? "JSON deserialization failed.");

      return obj;
   }
}
