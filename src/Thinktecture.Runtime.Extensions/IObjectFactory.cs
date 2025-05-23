namespace Thinktecture;

/// <summary>
/// Marker interface.
/// </summary>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
[Obsolete("Use 'IObjectFactory<TValue>' instead.")]
public interface IValueObjectFactory<TValue> : IObjectFactory<TValue>
   where TValue : notnull
#if NET9_0_OR_GREATER
               , allows ref struct
#endif
// ReSharper disable once RedundantTypeDeclarationBody
{
}

/// <summary>
/// Marker interface.
/// </summary>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
// ReSharper disable once UnusedTypeParameter
public interface IObjectFactory<TValue>
   where TValue : notnull
#if NET9_0_OR_GREATER
               , allows ref struct
#endif
// ReSharper disable once RedundantTypeDeclarationBody
{
}

/// <summary>
/// A factory for creation of instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValue">Type of the value to create the item from.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
[Obsolete("Use 'IObjectFactory<T, TValue, TValidationError>' instead.")]
public interface IValueObjectFactory<T, TValue, out TValidationError> : IObjectFactory<T, TValue, TValidationError>
   where TValue : notnull
#if NET9_0_OR_GREATER
               , allows ref struct
#endif
   where TValidationError : class, IValidationError<TValidationError>
{

}
/// <summary>
/// A factory for creation of instances of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValue">Type of the value to create the item from.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
/// <remarks>
/// Don't use this interface directly. It will be used by a source generator.
/// </remarks>
public interface IObjectFactory<T, TValue, out TValidationError> : IObjectFactory<TValue>
   where TValue : notnull
#if NET9_0_OR_GREATER
               , allows ref struct
#endif
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Validates the <paramref name="value"/> and returns an <paramref name="item"/> if the validation succeeded.
   /// </summary>
   /// <param name="value">The value to validate.</param>
   /// <param name="provider">An object that provides culture-specific formatting information.</param>
   /// <param name="item">Item with key property equals to the provided <paramref name="value"/>.</param>
   /// <returns>Validation error if validation failed; otherwise <c>null</c>.</returns>
   static abstract TValidationError? Validate(TValue? value, IFormatProvider? provider, out T? item);
}
