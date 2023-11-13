namespace Thinktecture;

/// <summary>
/// Defines custom type to be used as a validation error.
/// </summary>
public abstract class ValueObjectValidationErrorAttribute : Attribute
{
   /// <summary>
   /// Type of the validation error.
   /// </summary>
   public Type Type { get; }

   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectValidationErrorAttribute{TValidationError}"/>.
   /// </summary>
   protected ValueObjectValidationErrorAttribute(Type type)
   {
      Type = type;
   }
}

/// <summary>
/// Defines custom type to be used as a validation error.
/// </summary>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ValueObjectValidationErrorAttribute<TValidationError> : ValueObjectValidationErrorAttribute
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Initializes new instance of <see cref="ValueObjectValidationErrorAttribute{TValidationError}"/>.
   /// </summary>
   public ValueObjectValidationErrorAttribute()
      : base(typeof(TValidationError))
   {
   }
}
