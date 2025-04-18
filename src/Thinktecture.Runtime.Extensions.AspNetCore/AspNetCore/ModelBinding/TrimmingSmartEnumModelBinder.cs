namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for implementations of string-based Smart Enums Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[Obsolete("Use 'mvcOptions.AddThinktectureModelBinding()' instead")]
public sealed class TrimmingSmartEnumModelBinder<T, TValidationError> : ThinktectureModelBinderBase<T, string, TValidationError>
   where T : IObjectFactory<T, string, TValidationError>
   where TValidationError : class, IValidationError<TValidationError>;
