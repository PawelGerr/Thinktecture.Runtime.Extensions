namespace Thinktecture;

/// <summary>
/// Validation error.
/// </summary>
/// <typeparam name="T">Concrete type of the validation error.</typeparam>
public interface IValidationError<out T>
   where T : class
{
   /// <summary>
   /// Factory method for creation of the validation error with provided <paramref name="message"/>.
   /// </summary>
   /// <param name="message">Error message.</param>
   /// <returns>Validation error of type '<typeparamref name="T"/>'.</returns>
   static abstract T Create(string message);
}
