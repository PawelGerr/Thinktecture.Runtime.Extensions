using System.Diagnostics.CodeAnalysis;
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
public sealed class StructValueObjectMessagePackFormatter<T, TKey, TValidationError> : IMessagePackFormatter<T>, IMessagePackFormatter<T?>
   where T : struct, IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));

   /// <inheritdoc />
   public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
   {
      var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
      formatter.Serialize(ref writer, value.ToValue(), options);
   }

   /// <inheritdoc />
   public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
   {
      if (value is null)
      {
         writer.WriteNil();
      }
      else
      {
         Serialize(ref writer, value.Value, options);
      }
   }

   /// <inheritdoc />
   public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
   {
      if (!TryReadKey(ref reader, options, out var key))
         return default;

      return Deserialize(key);
   }

   /// <inheritdoc />
   T? IMessagePackFormatter<T?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
   {
      if (!TryReadKey(ref reader, options, out var key))
         return default;

      return Deserialize(key);
   }

   private static bool TryReadKey(
      ref MessagePackReader reader,
      MessagePackSerializerOptions options,
      [MaybeNullWhen(false)] out TKey key)
   {
      if (reader.TryReadNil())
      {
         key = default;
         return false;
      }

      var formatter = options.Resolver.GetFormatterWithVerify<TKey?>();
      key = formatter.Deserialize(ref reader, options);

      return key is not null;
   }

   private static T Deserialize(TKey key)
   {
      var validationError = T.Validate(key, null, out var obj);

      if (validationError is not null && !_mayReturnInvalidObjects)
         throw new ValidationException(validationError.ToString() ?? "MessagePack deserialization failed.");

      return obj;
   }
}
