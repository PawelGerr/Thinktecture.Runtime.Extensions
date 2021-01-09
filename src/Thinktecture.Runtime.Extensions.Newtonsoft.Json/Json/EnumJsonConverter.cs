using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Json
{
   /// <summary>
   /// Non-generic converter for <see cref="IEnum{TKey}"/>.
   /// </summary>
   public class EnumJsonConverter : JsonConverter
   {
      private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new();

      /// <inheritdoc />
      public override bool CanConvert(Type objectType)
      {
         return _cache.ContainsKey(objectType) || EnumMetadataLookup.FindEnum(objectType) is not null;
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
         var enumMetadata = EnumMetadataLookup.FindEnum(type);

         if (enumMetadata is null)
            throw new InvalidOperationException($"The provided type does not derive from 'Enum<,>'. Type: {type.Name}");

         var converterType = typeof(EnumJsonConverter<,>).MakeGenericType(enumMetadata.EnumType, enumMetadata.KeyType);
         var converter = Activator.CreateInstance(converterType, enumMetadata.ConvertFromKey)
                         ?? throw new Exception($"Could not create a converter of type '{converterType.Name}'.");

         return (JsonConverter)converter;
      }
   }

   /// <summary>
   /// <see cref="JsonConverter"/> for <see cref="IEnum{TKey}"/>
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumJsonConverter<TEnum, TKey> : JsonConverter<TEnum?>
      where TEnum : IEnum<TKey>
      where TKey : notnull
   {
      private readonly Func<TKey?, TEnum?> _convert;

      /// <summary>
      /// Initializes a new instance of <see cref="EnumJsonConverter{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="convert">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="TEnum"/>.</param>
      public EnumJsonConverter(Func<TKey?, TEnum?> convert)
      {
         _convert = convert ?? throw new ArgumentNullException(nameof(convert));
      }

      /// <inheritdoc />
      public override void WriteJson(JsonWriter writer, TEnum? value, JsonSerializer serializer)
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
            serializer.Serialize(writer, value.GetKey());
         }
      }

      /// <inheritdoc />
      public override TEnum? ReadJson(JsonReader reader, Type objectType, TEnum? existingValue, bool hasExistingValue, JsonSerializer serializer)
      {
         if (reader is null)
            throw new ArgumentNullException(nameof(reader));
         if (serializer is null)
            throw new ArgumentNullException(nameof(serializer));

         if (reader.TokenType == JsonToken.Null)
            return default;

         var token = serializer.Deserialize<JToken>(reader);

         if (token is null || token.Type == JTokenType.Null)
            return default;

         var key = token.ToObject<TKey>(serializer);
         return _convert(key);
      }
   }
}
