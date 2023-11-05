using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Json;

/// <summary>
/// <see cref="JsonConverter"/> for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class ValueObjectNewtonsoftJsonConverter<T, TKey> : JsonConverter
   where T : IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   private static readonly Type _type = typeof(T);
   private static readonly Type _keyType = typeof(TKey);
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));

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
         serializer.Serialize(writer, ((T)value).GetKey());
      }
   }

   /// <inheritdoc />
   public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
   {
      ArgumentNullException.ThrowIfNull(reader);
      ArgumentNullException.ThrowIfNull(serializer);

      if (reader.TokenType == JsonToken.Null)
      {
         if (objectType.IsClass || Nullable.GetUnderlyingType(objectType) == _type)
            return null;

         if (_keyType.IsClass)
            return default(T);

         throw new JsonException($"Cannot convert 'Null' to a struct of type '{_keyType.Name}', which is the underlying type of '{typeof(T).FullName}'.");
      }

      var token = serializer.Deserialize<JToken>(reader);

      if (token is null || token.Type == JTokenType.Null)
         return null;

      var key = token.ToObject<TKey>(serializer);

      if (key is null)
         return null;

      var validationResult = T.Validate(key, null, out var obj);

      if (validationResult != ValidationResult.Success && !_mayReturnInvalidObjects)
         throw new JsonSerializationException(validationResult!.ErrorMessage ?? "JSON deserialization failed.");

      return obj;
   }

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _type.IsAssignableFrom(objectType);
   }
}
