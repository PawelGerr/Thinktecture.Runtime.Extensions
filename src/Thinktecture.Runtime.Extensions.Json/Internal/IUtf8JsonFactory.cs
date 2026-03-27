#if NET9_0_OR_GREATER
namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public interface IUtf8JsonFactory<T, TValidationError>
   where T : notnull
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <summary>
   /// Validates the provided <paramref name="value"/> and attempts to convert it.
   /// </summary>
   /// <param name="value">The value to validate.</param>
   /// <param name="provider">An optional format provider for culture-specific formatting.</param>
   /// <param name="result">The converted object if validation succeeds; otherwise, <c>null</c>.</param>
   /// <returns>A validation error if the validation fails; otherwise, <c>null</c>.</returns>
   TValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out T? result);
}
#endif
