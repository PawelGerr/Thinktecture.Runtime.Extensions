namespace Thinktecture.Internal;

/// <summary>
/// For internal use only.
/// </summary>
public static class ValidationErrorCreator
{
   /// <summary>
   /// Creates a validation error of type <typeparamref name="TError"/>.
   /// Is necessary if the method <see cref="IValidationError{T}.Create"/> is implemented explicitly.
   /// </summary>
   /// <param name="message">Error message.</param>
   /// <typeparam name="TError">Type of the validation error.</typeparam>
   /// <returns>An instance of <typeparamref name="TError"/>.</returns>
   [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
   public static TError CreateValidationError<TError>(string message)
      where TError : class, IValidationError<TError>
   {
      return TError.Create(message);
   }
}
