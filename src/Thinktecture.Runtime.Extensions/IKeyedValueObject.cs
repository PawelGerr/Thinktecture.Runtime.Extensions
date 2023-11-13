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
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
/// <remarks>
/// Don't implement this interface directly. It will be implemented by a source generator.
/// </remarks>
public interface IKeyedValueObject<T, TKey, out TValidationError> : IKeyedValueObject<TKey>, IValueObjectFactory<T, TKey, TValidationError>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
}
