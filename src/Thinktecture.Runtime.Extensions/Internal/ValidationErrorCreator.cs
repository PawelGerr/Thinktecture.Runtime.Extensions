namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public static class ValidationErrorCreator
{
   /// <summary>
   /// Creates a validation error of type <typeparamref name="TError"/>.
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
