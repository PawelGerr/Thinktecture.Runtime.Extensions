using System.ComponentModel.DataAnnotations;

namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class ValueObjectMessagePackFormatter<T, TKey> : ValueObjectMessagePackFormatterBase<T, TKey>
   where T : class, IKeyedValueObject<T, TKey>
   where TKey : notnull
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="mayReturnInvalidObjects">Indication whether invalid should be returned on deserialization. If <c>false</c> then a <see cref="ValidationException"/> is thrown.</param>
   public ValueObjectMessagePackFormatter(bool mayReturnInvalidObjects)
      : base(mayReturnInvalidObjects)
   {
   }
}
