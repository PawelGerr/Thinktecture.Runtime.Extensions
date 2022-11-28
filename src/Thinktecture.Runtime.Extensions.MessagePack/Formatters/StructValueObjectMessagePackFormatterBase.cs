using System.Diagnostics.CodeAnalysis;
using MessagePack;
using MessagePack.Formatters;

#if NET7_0
using System.ComponentModel.DataAnnotations;
#endif

namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public abstract class StructValueObjectMessagePackFormatterBase<T, TKey> : IMessagePackFormatter<T>, IMessagePackFormatter<T?>
   where T : struct, IKeyedValueObject<TKey>
#if NET7_0
 , IKeyedValueObject<T, TKey>
#endif
   where TKey : notnull
{
#if NET7_0
   private readonly bool _mayReturnInvalidObjects;
#else
   private readonly Func<TKey, T> _convertFromKey;
#endif

#if NET7_0
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="ValidationException"/> is thrown.</param>
   protected StructValueObjectMessagePackFormatterBase(bool mayReturnInvalidObjects)
   {
      _mayReturnInvalidObjects = mayReturnInvalidObjects;
   }
#else
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   protected StructValueObjectMessagePackFormatterBase(Func<TKey, T> convertFromKey)
   {
      _convertFromKey = convertFromKey ?? throw new ArgumentNullException(nameof(convertFromKey));
   }
#endif

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
#if NET7_0
      var validationResult = T.Validate(key, out var obj);

      if (validationResult != ValidationResult.Success && !_mayReturnInvalidObjects)
         throw new ValidationException(validationResult!.ErrorMessage ?? "MessagePack deserialization failed.");

      return obj;
#else
      return _convertFromKey(key);
#endif
   }
}
