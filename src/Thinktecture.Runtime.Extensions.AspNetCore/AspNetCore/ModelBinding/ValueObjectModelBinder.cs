namespace Thinktecture.AspNetCore.ModelBinding;

/// <summary>
/// Model binder for Smart Enums Value Objects with a key member.
/// </summary>
/// <typeparam name="T">Type of the value object.</typeparam>
/// <typeparam name="TKey">Type of the key member.</typeparam>
/// <typeparam name="TValidationError">Type of the validation error.</typeparam>
[ThinktectureRuntimeExtensionInternal]
public sealed class ThinktectureModelBinder<T, TKey, TValidationError> : ThinktectureModelBinderBase<T, TKey, TValidationError>
   where T : IObjectFactory<T, TKey, TValidationError>
   where TKey : notnull
   where TValidationError : class, IValidationError<TValidationError>;
