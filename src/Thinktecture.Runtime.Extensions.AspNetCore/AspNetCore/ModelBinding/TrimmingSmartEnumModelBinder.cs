namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of string-based Smart Enums Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'ValueObjectModelBinder' instead")]
public sealed class TrimmingSmartEnumModelBinder<T, TValidationError> : ValueObjectModelBinderBase<T, string, TValidationError>
   where T : IValueObjectFactory<T, string, TValidationError>
   where TValidationError : class, IValidationError<TValidationError>;
