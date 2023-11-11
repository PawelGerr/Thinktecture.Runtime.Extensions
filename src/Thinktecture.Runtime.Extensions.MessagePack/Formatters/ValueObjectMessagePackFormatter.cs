using MessagePack;
using MessagePack.Formatters;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class ValueObjectMessagePackFormatter<T, TKey> : IMessagePackFormatter<T?>
   where T : class, IValueObjectFactory<T, TKey>, IValueObjectConverter<TKey>
   where TKey : notnull
{
   private static readonly bool _mayReturnInvalidObjects = typeof(IValidatableEnum).IsAssignableFrom(typeof(T));

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
         return default;

      var formatter = options.Resolver.GetFormatterWithVerify<TKey?>();
      var key = formatter.Deserialize(ref reader, options);

      if (key is null)
         return default;

      var validationResult = T.Validate(key, null, out var obj);

      if (validationResult is not null && !_mayReturnInvalidObjects)
         throw new ValidationException(validationResult.ErrorMessage ?? "MessagePack deserialization failed.");

      return obj;
   }
}
