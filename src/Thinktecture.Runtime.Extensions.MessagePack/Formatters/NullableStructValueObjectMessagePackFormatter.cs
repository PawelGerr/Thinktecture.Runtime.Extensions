namespace Thinktecture.Formatters;

/// <summary>
/// MessagePack formatter for Value Objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key.</typeparam>
public class NullableStructValueObjectMessagePackFormatter<T, TKey> : ValueObjectMessagePackFormatter<T?, TKey>
   where T : struct
   where TKey : notnull
{
   /// <inheritdoc />
   public NullableStructValueObjectMessagePackFormatter(Func<TKey, T> convertFromKey, Func<T, TKey> convertToKey)
      : base(key => convertFromKey(key), value => convertToKey(value!.Value))
   {
   }
}
