using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// JSON converter for instances of type <see cref="Enum{TEnum}"/>.
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   public class EnumJsonConverter<TEnum> : EnumJsonConverter<TEnum, string>
      where TEnum : Enum<TEnum, string>
   {
      /// <summary>
      /// Initializes new instance of type <see cref="EnumJsonConverter{TEnum}"/>.
      /// </summary>
      /// <param name="keyConverter">JSON converter for the key.</param>
      public EnumJsonConverter([CanBeNull] JsonConverter<string> keyConverter = null)
         : base(keyConverter)
      {
      }
   }

   /// <summary>
   /// JSON converter for instances of type <see cref="Enum{TEnum,TKey}"/>.
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumJsonConverter<TEnum, TKey> : JsonConverter<TEnum>
      where TEnum : Enum<TEnum, TKey>
   {
      private JsonConverter<TKey> _keyConverter;

      /// <summary>
      /// Initializes new instance of type <see cref="EnumJsonConverter{TEnum,Tkey}"/>.
      /// </summary>
      /// <param name="keyConverter">JSON converter for the key.</param>
      public EnumJsonConverter([CanBeNull] JsonConverter<TKey> keyConverter = null)
      {
         _keyConverter = keyConverter;
      }

      /// <inheritdoc />
      public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
         var key = GetKeyConverter(options).Read(ref reader, typeof(TKey), options);
         return Enum<TEnum, TKey>.Get(key);
      }

      /// <inheritdoc />
      public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
      {
         GetKeyConverter(options).Write(writer, value.Key, options);
      }

      [NotNull]
      private JsonConverter<TKey> GetKeyConverter([NotNull] JsonSerializerOptions options)
      {
         if (_keyConverter is null)
         {
            var converter = options.GetConverter(typeof(TKey));

            _keyConverter = (JsonConverter<TKey>)converter ?? throw new JsonException($"The enum '{typeof(TEnum).Name}' is not JSON-serializable because there is no {nameof(JsonConverter)} for its key of type '{typeof(TKey)}'.");
         }

         return _keyConverter;
      }
   }
}
