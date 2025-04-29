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
[Obsolete("Use 'ThinktectureStructMessagePackFormatter' instead.")]
public sealed class StructValueObjectMessagePackFormatter<T, TKey, TValidationError> : ThinktectureStructMessagePackFormatter<T, TKey, TValidationError>
   where T : struct, IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public class ThinktectureStructMessagePackFormatter<T, TKey, TValidationError> : IMessagePackFormatter<T>, IMessagePackFormatter<T?>
   where T : struct, IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   private static readonly bool _disallowDefaultValues = typeof(IDisallowDefaultValue).IsAssignableFrom(typeof(T));

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
      var formatter = options.Resolver.GetFormatterWithVerify<TKey?>();
      var key = formatter.Deserialize(ref reader, options);

      if (key is null)
      {
         if (_disallowDefaultValues)
            throw new MessagePackSerializationException($"Cannot convert null to type \"{typeof(T).Name}\" because it doesn't allow default values.");

         return default;
      }

      return Deserialize(key);
   }

   /// <inheritdoc />
   T? IMessagePackFormatter<T?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
   {
      if (!TryReadKey(ref reader, options, out var key))
         return null; // nullable struct

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

      if (validationError is not null)
         throw new ValidationException(validationError.ToString() ?? "MessagePack deserialization failed.");

      return obj;
   }
}
