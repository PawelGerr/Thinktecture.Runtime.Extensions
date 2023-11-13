namespace Thinktecture;

/// <summary>
/// Default implementation of <see cref="IValidationError{T}"/>.
/// </summary>
public sealed class ValidationError : IValidationError<ValidationError>, IEquatable<ValidationError>
{
   /// <summary>
   /// Error message.
   /// </summary>
   public string Message { get; }

   /// <summary>
   /// Initializes new instance of <see cref="ValidationError"/>.
   /// </summary>
   /// <param name="message">Error message.</param>
   public ValidationError(string message)
   {
      ArgumentNullException.ThrowIfNull(message);

      Message = message;
   }

   /// <inheritdoc />
   public static ValidationError Create(string message)
   {
      return new ValidationError(message);
   }

   /// <inheritdoc />
   public override bool Equals(object? obj)
   {
      if (ReferenceEquals(null, obj))
         return false;

      if (ReferenceEquals(this, obj))
         return true;

      return Equals((ValidationError)obj);
   }

   /// <inheritdoc />
   public bool Equals(ValidationError? other)
   {
      if (ReferenceEquals(null, other))
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return Message == other.Message;
   }

   /// <inheritdoc />
   public override int GetHashCode()
   {
      return Message.GetHashCode();
   }

   /// <inheritdoc />
   public override string ToString()
   {
      return Message;
   }
}
