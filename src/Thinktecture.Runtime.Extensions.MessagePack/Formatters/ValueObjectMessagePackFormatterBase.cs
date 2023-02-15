using MessagePack;
using MessagePack.Formatters;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public abstract class ValueObjectMessagePackFormatterBase<T, TKey> : IMessagePackFormatter<T>
   where T : IKeyedValueObject<T, TKey>?
   where TKey : notnull
{
   private readonly bool _mayReturnInvalidObjects;

   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="ValidationException"/> is thrown.</param>
   protected ValueObjectMessagePackFormatterBase(bool mayReturnInvalidObjects)
   {
      _mayReturnInvalidObjects = mayReturnInvalidObjects;
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
         var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
         formatter.Serialize(ref writer, value.GetKey(), options);
      }
   }

   /// <inheritdoc />
#pragma warning disable CS8766
   public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
#pragma warning restore CS8766
   {
      if (reader.TryReadNil())
         return default;

      var formatter = options.Resolver.GetFormatterWithVerify<TKey>();
      var key = formatter.Deserialize(ref reader, options);

      if (key is null)
         return default;

      var validationResult = T.Validate(key, out var obj);

      if (validationResult is not null)
         throw new ValidationException(validationResult.ErrorMessage);

      return obj;
   }
}
