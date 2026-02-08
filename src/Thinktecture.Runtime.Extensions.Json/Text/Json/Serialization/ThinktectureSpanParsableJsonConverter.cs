#if NET9_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Internal;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// JSON converter for types implementing IObjectFactory with ReadOnlySpan&lt;char&gt; support,
/// enabling zero-allocation deserialization.
/// </summary>
/// <typeparam name="T">Type of the object.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureSpanParsableJsonConverter<T, TValidationError> : JsonConverter<T>
   where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>, IConvertible<ReadOnlySpan<char>>
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _disallowDefaultValues = typeof(IDisallowDefaultValue).IsAssignableFrom(typeof(T));

   /// <summary>
   /// Initializes a new instance of <see cref="ThinktectureSpanParsableJsonConverter{T,TValidationError}"/>.
   /// </summary>
   /// <param name="options">JSON serializer options.</param>
   public ThinktectureSpanParsableJsonConverter(JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(options);
   }

   /// <inheritdoc />
   public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
      {
         if (_disallowDefaultValues)
            throw new JsonException($"Cannot convert null to type \"{typeof(T).Name}\" because it doesn't allow default values.");

         return default;
      }

      var validationError = Utf8JsonReaderHelper.ValidateFromUtf8<T, TValidationError>(ref reader, null, out var obj);

      if (validationError is not null)
         throw new JsonException(validationError.ToString() ?? "JSON deserialization failed.");

      return obj;
   }

   /// <inheritdoc />
   public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      writer.WriteStringValue(value.ToValue());
   }

   /// <inheritdoc />
   public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      return Read(ref reader, typeToConvert, options) ?? throw new JsonException("Property name cannot be null.");
   }

   /// <inheritdoc />
   public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
   {
      if (value is null)
         throw new ArgumentNullException(nameof(value));

      writer.WritePropertyName(value.ToValue());
   }
}
#endif
