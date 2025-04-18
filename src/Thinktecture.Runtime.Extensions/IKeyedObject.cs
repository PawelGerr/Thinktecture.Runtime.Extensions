namespace Thinktecture;

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
/// <typeparam name="TKey">Type of the key member.</typeparam>
[Obsolete("Use 'IKeyedObject<TKey>' instead.")]
public interface IKeyedValueObject<TKey> : IConvertible<TKey>
   where TKey : notnull;

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
/// <typeparam name="TKey">Type of the key member.</typeparam>
public interface IKeyedObject<TKey> : IConvertible<TKey>
   where TKey : notnull;
