using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class StructValueObjectMessagePackFormatter<T, TKey> : StructValueObjectMessagePackFormatterBase<T, TKey>
   where T : struct, IKeyedValueObject<TKey>
#if NET7_0
 , IKeyedValueObject<T, TKey>
#endif
   where TKey : notnull
{
#if NET7_0
   /// <summary>
   /// Initializes a new instance of <see cref="StructValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="ValidationException"/> is thrown.</param>
   public StructValueObjectMessagePackFormatter(bool mayReturnInvalidObjects)
      : base(mayReturnInvalidObjects)
   {
   }
#else
   /// <summary>
   /// Initializes a new instance of <see cref="StructValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   public StructValueObjectMessagePackFormatter(Func<TKey, T> convertFromKey)
      : base(convertFromKey)
   {
   }
#endif
}
