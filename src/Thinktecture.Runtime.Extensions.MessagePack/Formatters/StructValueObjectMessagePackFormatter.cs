using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class StructValueObjectMessagePackFormatter<T, TKey> : StructValueObjectMessagePackFormatterBase<T, TKey>
   where T : struct, IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   /// <summary>
   /// Initializes a new instance of <see cref="StructValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="ValidationException"/> is thrown.</param>
   public StructValueObjectMessagePackFormatter(bool mayReturnInvalidObjects)
      : base(mayReturnInvalidObjects)
   {
   }
}
