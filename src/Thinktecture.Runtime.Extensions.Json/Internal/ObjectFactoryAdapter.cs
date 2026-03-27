#if NET9_0_OR_GREATER
using System.Runtime.CompilerServices;

namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public readonly struct ObjectFactoryAdapter<T, TValidationError> : IUtf8JsonFactory<T, TValidationError>
   where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
   where TValidationError : class, IValidationError<TValidationError>
{
   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out T? result)
      => T.Validate(value, provider, out result);
}
#endif
