namespace Thinktecture;

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
public interface IKeyedValueObject
{
}

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
/// <typeparam name="TKey">Type of the key member.</typeparam>
public interface IKeyedValueObject<TKey> : IKeyedValueObject, IValueObjectConvertable<TKey>
   where TKey : notnull
{
}
