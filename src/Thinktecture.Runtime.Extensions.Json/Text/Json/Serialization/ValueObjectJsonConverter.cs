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
   where T : IKeyedValueObject<TKey>
#if NET7_0
 , IKeyedValueObject<T, TKey>
#endif
   where TKey : notnull
{
#if NET7_0
   private readonly bool _mayReturnInvalidObjects;
#else
   private readonly Func<TKey, T?> _convertFromKey;
#endif
   private readonly JsonConverter<TKey> _keyConverter;

#if NET7_0
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="JsonException"/> is thrown.</param>
   /// <param name="options">JSON serializer options.</param>
   public ValueObjectJsonConverter(
      bool mayReturnInvalidObjects,
      JsonSerializerOptions options)
   {
      if (options is null)
         throw new ArgumentNullException(nameof(options));

      _mayReturnInvalidObjects = mayReturnInvalidObjects;
      _keyConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));
   }
#else
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectJsonConverter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   /// <param name="options">JSON serializer options.</param>
   public ValueObjectJsonConverter(
      Func<TKey, T?> convertFromKey,
      JsonSerializerOptions options)
   {
      if (options is null)
         throw new ArgumentNullException(nameof(options));

      _convertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
      _keyConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));
   }
#endif

   /// <inheritdoc />
   public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      var key = _keyConverter.Read(ref reader, typeof(TKey), options);

      if (key is null)
         return default;

#if NET7_0
      var validationResult = T.Validate(key, out var obj);

      if (validationResult != ValidationResult.Success && !_mayReturnInvalidObjects)
         throw new JsonException(validationResult!.ErrorMessage ?? "JSON deserialization failed.");

      return obj;
#else
      try
      {
         return _convertFromKey(key);
      }
      catch (UnknownEnumIdentifierException ex)
      {
         throw new JsonException(ex.Message, ex);
      }
      catch (ValidationException ex)
      {
         throw new JsonException(ex.ValidationResult.ErrorMessage, ex);
      }
#endif
   }

   /// <inheritdoc />
   public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      _keyConverter.Write(writer, value.GetKey(), options);
   }
}
