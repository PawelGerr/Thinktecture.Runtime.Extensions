using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// JSON converter for instances of type <see cref="IEnum{TKey}"/>.
   /// </summary>
   /// <typeparam name="TEnum">Type of the enum.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class EnumJsonConverter<TEnum, TKey> : JsonConverter<TEnum>
      where TEnum : IEnum<TKey>
      where TKey : notnull
   {
      private readonly Func<TKey?, TEnum?> _convert;

      private JsonConverter<TKey>? _keyConverter;

      /// <summary>
      /// Initializes a new instance of <see cref="EnumJsonConverter{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="convert">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="TEnum"/>.</param>
      /// <param name="keyConverter">JSON converter for the key.</param>
      public EnumJsonConverter(
         Func<TKey?, TEnum?> convert,
         JsonConverter<TKey>? keyConverter = null)
      {
         _convert = convert ?? throw new ArgumentNullException(nameof(convert));
         _keyConverter = keyConverter;
      }

      /// <inheritdoc />
      public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
         var key = GetKeyConverter(options).Read(ref reader, typeof(TKey), options);
         return _convert(key);
      }

      /// <inheritdoc />
      public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
      {
         if (value is null)
            throw new ArgumentNullException(nameof(value));

         GetKeyConverter(options).Write(writer, value.GetKey(), options);
      }

      private JsonConverter<TKey> GetKeyConverter(JsonSerializerOptions options)
      {
         if (_keyConverter is null)
         {
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            var converter = options.GetConverter(typeof(TKey));

            _keyConverter = (JsonConverter<TKey>)converter ?? throw new JsonException($"The enum '{typeof(TEnum).Name}' is not JSON-serializable because there is no {nameof(JsonConverter)} for its key of type '{typeof(TKey)}'.");
         }

         return _keyConverter;
      }
   }
}
