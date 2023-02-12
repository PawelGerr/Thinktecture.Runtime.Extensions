using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// JSON converter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class ValueObjectJsonConverter<T, TKey> : JsonConverter<T>
   where T : IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   private readonly bool _mayReturnInvalidObjects;
   private readonly JsonConverter<TKey> _keyConverter;

   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="JsonException"/> is thrown.</param>
   /// <param name="options">JSON serializer options.</param>
   public ValueObjectJsonConverter(
      bool mayReturnInvalidObjects,
      JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(options);

      _mayReturnInvalidObjects = mayReturnInvalidObjects;
      _keyConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));
   }

   /// <inheritdoc />
   public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      var key = _keyConverter.Read(ref reader, typeof(TKey), options);

      if (key is null)
         return default;

      var validationResult = T.Validate(key, out var obj);

      if (validationResult != ValidationResult.Success && !_mayReturnInvalidObjects)
         throw new JsonException(validationResult!.ErrorMessage ?? "JSON deserialization failed.");

      return obj;
   }

   /// <inheritdoc />
   public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      _keyConverter.Write(writer, value.GetKey(), options);
   }
}
