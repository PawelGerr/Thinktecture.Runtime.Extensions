#if NET9_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thinktecture.Text.Json.Serialization;

/// <summary>
/// Factory for creation of <see cref="ThinktectureSpanParsableJsonConverter{T,TValidationError}"/> with zero-allocation deserialization support.
/// </summary>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureSpanParsableJsonConverterFactory<T, TValidationError> : JsonConverterFactory
   where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>, IConvertible<ReadOnlySpan<char>>
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <inheritdoc />
   public override bool CanConvert(Type typeToConvert)
   {
      return typeof(T).IsAssignableFrom(typeToConvert);
   }

   /// <inheritdoc />
   public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
   {
      ArgumentNullException.ThrowIfNull(typeToConvert);
      ArgumentNullException.ThrowIfNull(options);

      return new ThinktectureSpanParsableJsonConverter<T, TValidationError>(options);
   }
}

#endif
