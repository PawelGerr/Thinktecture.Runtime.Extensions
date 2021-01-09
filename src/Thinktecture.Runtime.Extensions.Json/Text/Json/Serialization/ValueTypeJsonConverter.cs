using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization
{
   /// <summary>
   /// JSON converter for value types.
   /// </summary>
   /// <typeparam name="T">Type of the value type.</typeparam>
   /// <typeparam name="TKey">Type of the key.</typeparam>
   public class ValueTypeJsonConverter<T, TKey> : JsonConverter<T>
      where TKey : notnull
   {
      private readonly Func<TKey, T> _convertFromKey;
      private readonly Func<T, TKey> _convertToKey;

      private JsonConverter<TKey>? _keyConverter;

      /// <summary>
      /// Initializes a new instance of <see cref="ValueTypeJsonConverter{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
      /// <param name="convertToKey">Converts an instance of type <typeparamref name="T"/> to an instance of <typeparamref name="TKey"/>.</param>
      /// <param name="keyConverter">JSON converter for the key.</param>
      public ValueTypeJsonConverter(
         Func<TKey, T> convertFromKey,
         Func<T, TKey> convertToKey,
         JsonConverter<TKey>? keyConverter = null)
      {
         _convertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
         _convertToKey = convertToKey ?? throw new ArgumentNullException(nameof(convertToKey));
         _keyConverter = keyConverter;
      }

      /// <inheritdoc />
      public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
         var key = GetKeyConverter(options).Read(ref reader, typeof(TKey), options);

         if (key is null)
            return default;

         return _convertFromKey(key);
      }

      /// <inheritdoc />
      public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
      {
         if (value is null)
            throw new ArgumentNullException(nameof(value));

         GetKeyConverter(options).Write(writer, _convertToKey(value), options);
      }

      private JsonConverter<TKey> GetKeyConverter(JsonSerializerOptions options)
      {
         if (_keyConverter is null)
         {
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            var converter = options.GetConverter(typeof(TKey));

            _keyConverter = (JsonConverter<TKey>)converter ?? throw new JsonException($"The type '{typeof(T).Name}' is not JSON-serializable because there is no {nameof(JsonConverter)} for its key member of type '{typeof(TKey)}'.");
         }

         return _keyConverter;
      }
   }
}
