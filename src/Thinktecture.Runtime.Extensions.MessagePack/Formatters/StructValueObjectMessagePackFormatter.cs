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
public sealed class StructValueObjectMessagePackFormatter<T, TKey> : IMessagePackFormatter<T>, IMessagePackFormatter<T?>
   where T : struct, IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));

   /// <inheritdoc />
   public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
   {
      var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
      formatter.Serialize(ref writer, value.GetKey(), options);
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
#pragma warning disable CS8766
   public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
#pragma warning restore CS8766
   {
      if (!TryReadKey(ref reader, options, out var key))
         return default;

      return Deserialize(key);
   }

   /// <inheritdoc />
#pragma warning disable CS8766
   T? IMessagePackFormatter<T?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
#pragma warning restore CS8766
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

      var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
      key = formatter.Deserialize(ref reader, options);

      return key is not null;
   }

   private T Deserialize(TKey key)
   {
      var validationResult = T.Validate(key, out var obj);

      if (validationResult is not null && !_mayReturnInvalidObjects)
         throw new ValidationException(validationResult.ErrorMessage ?? "MessagePack deserialization failed.");

      return obj;
   }
}
