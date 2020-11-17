using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Json
{
   /// <summary>
   /// Non-generic converter for <see cref="Enum{TEnum, TKey}"/> and <see cref="Enum{TEnum}"/>.
   /// </summary>
   public class EnumJsonConverter : JsonConverter
   {
      private static readonly ConcurrentDictionary<Type, JsonConverter> _cache = new ConcurrentDictionary<Type, JsonConverter>();

      /// <inheritdoc />
      public override bool CanConvert(Type objectType)
      {
         return _cache.ContainsKey(objectType) || objectType.FindGenericEnumTypeDefinition() != null;
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
         var enumType = type.FindGenericEnumTypeDefinition();

         if (enumType is null)
            throw new InvalidOperationException($"The provided type does not derive from 'Enum<,>'. Type: {type.Name}");

         var converterType = typeof(EnumJsonConverter<,>).MakeGenericType(enumType.GenericTypeArguments);
         var converter = Activator.CreateInstance(converterType)
            ?? throw new Exception($"Could not create a converter of type '{converterType.Name}'.");

         return (JsonConverter)converter;
      }
   }

   /// <summary>
   /// <see cref="JsonConverter"/> for <see cref="Enum{TEnum}"/>
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   public class EnumJsonConverter<TEnum> : EnumJsonConverter<TEnum, string>
      where TEnum : Enum<TEnum>
   {
   }

   /// <summary>
   /// <see cref="JsonConverter"/> for <see cref="Enum{TEnum, TKey}"/>
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumJsonConverter<TEnum, TKey> : JsonConverter<Enum<TEnum, TKey>?>
      where TEnum : Enum<TEnum, TKey>
      where TKey : notnull
   {
      /// <inheritdoc />
      public override void WriteJson(JsonWriter writer, Enum<TEnum, TKey>? value, JsonSerializer serializer)
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
            serializer.Serialize(writer, value.Key);
         }
      }

      /// <inheritdoc />
      public override Enum<TEnum, TKey>? ReadJson(JsonReader reader, Type objectType, Enum<TEnum, TKey>? existingValue, bool hasExistingValue, JsonSerializer serializer)
      {
         if (reader is null)
            throw new ArgumentNullException(nameof(reader));
         if (serializer is null)
            throw new ArgumentNullException(nameof(serializer));

         if (reader.TokenType == JsonToken.Null)
            return null;

         var token = serializer.Deserialize<JToken>(reader);

         if (token is null || token.Type == JTokenType.Null)
            return null;

         var key = token.ToObject<TKey>(serializer);
         return Enum<TEnum, TKey>.Get(key);
      }
   }
}
