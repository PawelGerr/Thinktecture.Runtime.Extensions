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
      return obj is ValidationError other && Equals(other);
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

   /// <summary>
   /// Compares two instances of <see cref="ValidationError"/>.
   /// </summary>
   /// <param name="left">Instance to compare.</param>
   /// <param name="right">Another instance to compare.</param>
   /// <returns><c>true</c> if both instances are equal; otherwise <c>false</c>.</returns>
   public static bool operator ==(ValidationError? left, ValidationError? right)
   {
      if (left is null)
         return right is null;

      return left.Equals(right);
   }

   /// <summary>
   /// Compares two instances of <see cref="ValidationError"/>.
   /// </summary>
   /// <param name="left">Instance to compare.</param>
   /// <param name="right">Another instance to compare.</param>
   /// <returns><c>false</c> if both instances are equal; otherwise <c>true</c>.</returns>
   public static bool operator !=(ValidationError? left, ValidationError? right)
   {
      return !(left == right);
   }

   /// <inheritdoc />
   public override string ToString()
   {
      return Message;
   }
}
