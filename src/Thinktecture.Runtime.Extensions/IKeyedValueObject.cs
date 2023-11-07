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
public interface IKeyedValueObject<out TKey> : IValueObjectConverter<TKey>, IKeyedValueObject
   where TKey : notnull
{
}

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
/// <remarks>
/// Don't implement this interface directly. It will be implemented by a source generator.
/// </remarks>
public interface IKeyedValueObject<T, TKey> : IKeyedValueObject<TKey>, IValueObjectFactory<T, TKey>
   where TKey : notnull
{
}
