using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Json;

/// <summary>
/// <see cref="JsonConverter"/> for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public abstract class ValueObjectNewtonsoftJsonConverterBase<T, TKey> : JsonConverter
   where T : IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   private static readonly Type _type = typeof(T);
   private static readonly Type _keyType = typeof(TKey);

   private readonly bool _mayReturnInvalidObjects;

   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="JsonSerializationException"/> is thrown.</param>
   protected ValueObjectNewtonsoftJsonConverterBase(bool mayReturnInvalidObjects)
   {
      _mayReturnInvalidObjects = mayReturnInvalidObjects;
   }

   /// <inheritdoc />
   public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
   {
      if (writer is null)
         throw new ArgumentNullException(nameof(writer));
      if (serializer is null)
         throw new ArgumentNullException(nameof(serializer));

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
      if (reader is null)
         throw new ArgumentNullException(nameof(reader));
      if (serializer is null)
         throw new ArgumentNullException(nameof(serializer));

      if (reader.TokenType == JsonToken.Null)
      {
         if (objectType.IsClass || Nullable.GetUnderlyingType(objectType) == _type)
            return null;

         if (_keyType.IsClass)
            return default(T);

         throw new JsonException($"Cannot convert 'Null' to a struct of type '{_keyType.Name}'.");
      }

      var token = serializer.Deserialize<JToken>(reader);

      if (token is null || token.Type == JTokenType.Null)
         return null;

      var key = token.ToObject<TKey>(serializer);

      if (key is null)
         return null;

      var validationResult = T.Validate(key, out var obj);

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
