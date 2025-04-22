namespace Thinktecture;

/// <summary>
/// Defines custom type to be used as a validation error.
/// </summary>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'ValidationErrorAttribute<TValidationError>' instead.")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ValueObjectValidationErrorAttribute<TValidationError> : ValidationErrorAttribute<TValidationError>
   where TValidationError : class, IValidationError<TValidationError>;

/// <summary>
/// Defines a custom type to be used as a validation error.
/// </summary>
public abstract class ValidationErrorAttribute : Attribute
{
   /// <summary>
   /// Type of the validation error.
   /// </summary>
   public Type Type { get; }

   /// <summary>
   /// Initializes new instance of <see cref="ValidationErrorAttribute{TValidationError}"/>.
   /// </summary>
   private protected ValidationErrorAttribute(Type type)
   {
      Type = type;
   }
}

/// <summary>
/// Defines a custom type to be used as a validation error.
/// </summary>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class ValidationErrorAttribute<TValidationError> : ValidationErrorAttribute
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Initializes new instance of <see cref="ValidationErrorAttribute{TValidationError}"/>.
   /// </summary>
   public ValidationErrorAttribute()
      : base(typeof(TValidationError))
   {
   }
}
