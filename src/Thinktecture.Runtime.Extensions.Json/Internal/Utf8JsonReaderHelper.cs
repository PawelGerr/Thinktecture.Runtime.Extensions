#if NET9_0_OR_GREATER
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Thinktecture.Internal;

/// <summary>
/// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
/// the same compatibility standards as public APIs. It may be changed or removed without notice in
/// any release. You should only use it directly in your code with extreme caution and knowing that
/// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
/// </summary>
public static class Utf8JsonReaderHelper
{
   // Aligned with System.Text.Json's JsonConstants.StackallocCharThreshold (= StackallocByteThreshold / 2 = 128)
   // See: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/Common/JsonConstants.cs
   private const int _STACKALLOC_CHAR_THRESHOLD = 128;

   /// <summary>
   /// This is an internal API that supports the Thinktecture.Runtime.Extensions infrastructure and not subject to
   /// the same compatibility standards as public APIs. It may be changed or removed without notice in
   /// any release. You should only use it directly in your code with extreme caution and knowing that
   /// doing so can result in application failures when updating to a new Thinktecture.Runtime.Extensions release.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TValidationError? ValidateFromUtf8<T, TValidationError>(
      ref Utf8JsonReader reader,
      IFormatProvider? provider,
      out T? result)
      where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      where TValidationError : class, IValidationError<TValidationError>
   {
      // Fast path: contiguous, unescaped value (most common case)
      if (!reader.HasValueSequence && !reader.ValueIsEscaped)
      {
         return ValidateFastPath<T, TValidationError>(reader.ValueSpan, provider, out result);
      }

      // Slow path: escaped or fragmented value
      return ValidateSlowPath<T, TValidationError>(ref reader, provider, out result);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static TValidationError? ValidateFastPath<T, TValidationError>(
      ReadOnlySpan<byte> utf8Bytes,
      IFormatProvider? provider,
      out T? result)
      where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      where TValidationError : class, IValidationError<TValidationError>
   {
      // UTF-16 char count is always <= UTF-8 byte count, so this comparison is safe
      if (utf8Bytes.Length <= _STACKALLOC_CHAR_THRESHOLD)
      {
         // Constant size enables JIT to emit a simple stack bump instead of localloc
         Span<char> charBuf = stackalloc char[_STACKALLOC_CHAR_THRESHOLD];
         var charsWritten = Encoding.UTF8.GetChars(utf8Bytes, charBuf);
         return T.Validate(charBuf[..charsWritten], provider, out result);
      }

      // Large values: use array pool
      var rentedChars = ArrayPool<char>.Shared.Rent(utf8Bytes.Length);

      try
      {
         var charsWritten = Encoding.UTF8.GetChars(utf8Bytes, rentedChars);
         return T.Validate(rentedChars.AsSpan(0, charsWritten), provider, out result);
      }
      finally
      {
         ArrayPool<char>.Shared.Return(rentedChars);
      }
   }

   private static TValidationError? ValidateSlowPath<T, TValidationError>(
      ref Utf8JsonReader reader,
      IFormatProvider? provider,
      out T? result)
      where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      where TValidationError : class, IValidationError<TValidationError>
   {
      // Get the byte length for buffer sizing
      var byteLength = reader.HasValueSequence
                          ? checked((int)reader.ValueSequence.Length)
                          : reader.ValueSpan.Length;

      // Use CopyString for escaped/fragmented values
      if (byteLength <= _STACKALLOC_CHAR_THRESHOLD)
      {
         Span<char> charBuf = stackalloc char[_STACKALLOC_CHAR_THRESHOLD];
         var charsWritten = reader.CopyString(charBuf);
         return T.Validate(charBuf[..charsWritten], provider, out result);
      }

      var rentedChars = ArrayPool<char>.Shared.Rent(byteLength);

      try
      {
         var charsWritten = reader.CopyString(rentedChars);
         return T.Validate(rentedChars.AsSpan(0, charsWritten), provider, out result);
      }
      finally
      {
         ArrayPool<char>.Shared.Return(rentedChars);
      }
   }
}
#endif
