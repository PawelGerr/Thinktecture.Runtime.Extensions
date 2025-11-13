namespace Thinktecture;

/// <summary>
/// Common interface of keyed value objects.
/// </summary>
/// <typeparam name="TKey">Type of the key member.</typeparam>
public interface IKeyedObject<TKey> : IConvertible<TKey>
   where TKey : notnull;
