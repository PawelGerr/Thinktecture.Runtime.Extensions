namespace Thinktecture.Internal;

/// <summary>
/// A  utility class for invoking members of types with static abstract members.
/// </summary>
public static class StaticAbstractInvoker
{
   /// <summary>
   /// Validates the provided key and determines whether it meets the required criteria.
   /// </summary>
   /// <param name="key">The key to be validated, represented as a non-null value of type <typeparamref name="TKey"/>.</param>
   /// <param name="provider">
   /// An optional format provider that can influence the validation logic and provide culture-specific formatting information.
   /// </param>
   /// <param name="result">
   /// If validation is successful, this output parameter contains an instance of <typeparamref name="T"/> created from the key.
   /// If validation fails, this output parameter is set to null.
   /// </param>
   /// <typeparam name="T">
   /// The type of the resulting object, which must implement the <see cref="IObjectFactory{T, TKey, TError}"/> interface.
   /// </typeparam>
   /// <typeparam name="TKey">
   /// The type of the key being validated, which must be non-null.
   /// </typeparam>
   /// <typeparam name="TValidationError">
   /// The type of the validation error, which must implement the <see cref="IValidationError{TError}"/> interface.
   /// </typeparam>
   /// <returns>
   /// Returns an instance of <typeparamref name="TValidationError"/> if validation fails; otherwise, returns null if validation succeeds.
   /// </returns>
   public static TValidationError? Validate<T, TKey, TValidationError>(TKey key, IFormatProvider? provider, out T? result)
      where T : IObjectFactory<T, TKey, TValidationError>
      where TKey : notnull
      where TValidationError : class, IValidationError<TValidationError>
   {
      return T.Validate(key, provider, out result);
   }

#if NET9_0_OR_GREATER
   /// <summary>
   /// Validates the provided key and determines whether it meets the required criteria.
   /// </summary>
   /// <param name="key">The key to be validated as a read-only sequence of characters.</param>
   /// <param name="provider">
   /// An optional format provider that may influence the validation logic and provide culture-specific formatting details.
   /// </param>
   /// <param name="result">
   /// If validation is successful, this output parameter contains an instance of type <typeparamref name="T"/> created from the key.
   /// If validation fails, this output parameter is set to null.
   /// </param>
   /// <typeparam name="T">
   /// The type of the resulting object, which must implement the <see cref="IObjectFactory{T, TValue, TValidationError}"/> interface.
   /// </typeparam>
   /// <typeparam name="TValidationError">
   /// The type of the validation error, which must implement the <see cref="IValidationError{TValidationError}"/> interface.
   /// </typeparam>
   /// <returns>
   /// Returns an instance of <typeparamref name="TValidationError"/> if validation fails; otherwise, returns null if validation succeeds.
   /// </returns>
   public static TValidationError? Validate<T, TValidationError>(ReadOnlySpan<char> key, IFormatProvider? provider, out T? result)
      where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      where TValidationError : class, IValidationError<TValidationError>
   {
      return T.Validate(key, provider, out result);
   }
#endif
}
