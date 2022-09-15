namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public sealed class ValueObjectMessagePackFormatter<T, TKey> : ValueObjectMessagePackFormatterBase<T, TKey>
   where TKey : notnull
{
   /// <summary>
   /// Initializes a new instance of <see cref="ValueObjectMessagePackFormatter{T,TKey}"/>.
   /// </summary>
   /// <param name="convertFromKey">Converts an instance of type <typeparamref name="TKey"/> to an instance of <typeparamref name="T"/>.</param>
   /// <param name="convertToKey">Converts an instance of type <typeparamref name="T"/> to an instance of <typeparamref name="TKey"/>.</param>
   public ValueObjectMessagePackFormatter(Func<TKey, T> convertFromKey, Func<T, TKey> convertToKey)
      : base(convertFromKey, convertToKey)
   {
   }
}
