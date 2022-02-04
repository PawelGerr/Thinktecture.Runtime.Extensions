using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.Internal;

namespace Thinktecture.Json;

/// <summary>
/// Non-generic converter for Value Objects.
/// </summary>
public class ValueObjectNewtonsoftJsonConverter : JsonConverter
{
   private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new();

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _cache.ContainsKey(objectType) || ValueObjectMetadataLookup.Find(objectType) is not null;
   }

   /// <inheritdoc />
   public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
   {
      if (writer is null)
         throw new ArgumentNullException(nameof(writer));

      if (value is null)
      {
         writer.WriteNull();
      }
      else
      {
         var converter = _cache.GetOrAdd(value.GetType(), CreateConverter);
         converter.WriteJson(writer, value, serializer);
      }
   }

   /// <inheritdoc />
   public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
   {
      var converter = _cache.GetOrAdd(objectType, CreateConverter);

      return converter.ReadJson(reader, objectType, existingValue, serializer);
   }

   private static JsonConverter CreateConverter(Type type)
   {
      var metadata = ValueObjectMetadataLookup.Find(type);

      if (metadata is null)
         throw new InvalidOperationException($"The provided type is not serializable by the '{nameof(ValueObjectNewtonsoftJsonConverter)}'. Type: {type.FullName}");

      var converterType = typeof(ValueObjectNewtonsoftJsonConverter<,>).MakeGenericType(metadata.Type, metadata.KeyType);
      var converter = Activator.CreateInstance(converterType, metadata.ConvertFromKey, metadata.ConvertToKey)
                      ?? throw new Exception($"Could not create a converter of type '{converterType.Name}'.");

      return (JsonConverter)converter;
   }
}

/// <summary>
/// <see cref="JsonConverter"/> for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public class ValueObjectNewtonsoftJsonConverter<T, TKey> : JsonConverter
   where TKey : notnull
{
   private static readonly Type _type = typeof(T);
   private static readonly Type _keyType = typeof(TKey);

   private readonly Func<TKey, T> _convertFromKey;
   private readonly Func<T, TKey> _convertToKey;

   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectNewtonsoftJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   /// <param name="convertToKey">Converts an instance of type <typeparamref name="T"/> to an instance of <typeparamref name="TKey"/>.</param>
   public ValueObjectNewtonsoftJsonConverter(
      Func<TKey, T> convertFromKey,
      Func<T, TKey> convertToKey)
   {
      _convertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
      _convertToKey = convertToKey ?? throw new ArgumentNullException(nameof(convertToKey));
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
         serializer.Serialize(writer, _convertToKey((T)value));
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
   }

   /// <inheritdoc />
   public override bool CanConvert(Type objectType)
   {
      return _type.IsAssignableFrom(objectType);
   }
}
