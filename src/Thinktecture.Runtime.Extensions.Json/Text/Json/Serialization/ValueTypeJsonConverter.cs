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
      private readonly JsonConverter<TKey> _keyConverter;

      /// <summary>
      /// Initializes a new instance of <see cref="ValueTypeJsonConverter{TEnum,TKey}"/>.
      /// </summary>
      /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
      /// <param name="convertToKey">Converts an instance of type <typeparamref name="T"/> to an instance of <typeparamref name="TKey"/>.</param>
      /// <param name="options">JSON serializer options.</param>
      public ValueTypeJsonConverter(
         Func<TKey, T> convertFromKey,
         Func<T, TKey> convertToKey,
         JsonSerializerOptions options)
      {
         if (options is null)
            throw new ArgumentNullException(nameof(options));

         _convertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
         _convertToKey = convertToKey ?? throw new ArgumentNullException(nameof(convertToKey));
         _keyConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));
      }

      /// <inheritdoc />
      public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {
         var key = _keyConverter.Read(ref reader, typeof(TKey), options);

         if (key is null)
            return default;

         return _convertFromKey(key);
      }

      /// <inheritdoc />
      public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
      {
         if (value is null)
            throw new ArgumentNullException(nameof(value));

         _keyConverter.Write(writer, _convertToKey(value), options);
      }
   }
}
