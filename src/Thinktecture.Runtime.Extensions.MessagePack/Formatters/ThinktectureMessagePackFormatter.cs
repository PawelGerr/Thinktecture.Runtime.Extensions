using MessagePack;
using MessagePack.Formatters;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'ThinktectureMessageFormatterResolver' instead.")]
public sealed class ValueObjectMessagePackFormatter<T, TKey, TValidationError> : ThinktectureMessagePackFormatter<T, TKey, TValidationError>
   where T : class, IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureMessagePackFormatter<T, TKey, TValidationError> : IMessagePackFormatter<T?>
   where T : class, IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <inheritdoc />
   public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
   {
      if (value is null)
      {
         writer.WriteNil();
      }
      else
      {
         var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
         formatter.Serialize(ref writer, value.ToValue(), options);
      }
   }

   /// <inheritdoc />
   public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
   {
      if (reader.TryReadNil())
         return null;

      var formatter = options.Resolver.GetFormatterWithVerify<TKey?>();
      var key = formatter.Deserialize(ref reader, options);

      if (key is null)
         return null;

      var validationError = T.Validate(key, null, out var obj);

      if (validationError is not null)
         throw new ValidationException(validationError.ToString() ?? "MessagePack deserialization failed.");

      return obj;
   }
}
