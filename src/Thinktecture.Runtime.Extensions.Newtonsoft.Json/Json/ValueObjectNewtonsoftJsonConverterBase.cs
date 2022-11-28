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
   where T : IKeyedValueObject<TKey>
#if NET7_0
 , IKeyedValueObject<T, TKey>
#endif
   where TKey : notnull
{
   private static readonly Type _type = typeof(T);
   private static readonly Type _keyType = typeof(TKey);

#if NET7_0
   private readonly bool _mayReturnInvalidObjects;
#else
   private readonly Func<TKey, T?> _convertFromKey;
#endif

#if NET7_0
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="JsonSerializationException"/> is thrown.</param>
   protected ValueObjectNewtonsoftJsonConverterBase(bool mayReturnInvalidObjects)
   {
      _mayReturnInvalidObjects = mayReturnInvalidObjects;
   }
#else
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   protected ValueObjectNewtonsoftJsonConverterBase(Func<TKey, T?> convertFromKey)
   {
      _convertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
   }
#endif

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

#if NET7_0
      var validationResult = T.Validate(key, out var obj);

      if (validationResult != ValidationResult.Success && !_mayReturnInvalidObjects)
         throw new JsonSerializationException(validationResult!.ErrorMessage ?? "JSON deserialization failed.");

      return obj;
#else
      try
      {
         return _convertFromKey(key);
      }
      catch (UnknownEnumIdentifierException ex)
      {
         throw new JsonSerializationException(ex.Message, ex);
      }
      catch (ValidationException ex)
      {
         throw new JsonSerializationException(ex.ValidationResult.ErrorMessage!, ex);
      }
#endif
   }

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _type.IsAssignableFrom(objectType);
   }
}
