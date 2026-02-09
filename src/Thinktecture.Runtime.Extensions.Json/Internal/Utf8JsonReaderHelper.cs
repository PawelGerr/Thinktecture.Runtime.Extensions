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
[SkipLocalsInit]
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
      // Escaped values (rarest case): CopyString handles unescaping and reassembly
      if (reader.ValueIsEscaped)
         return ValidateEscaped<T, TValidationError>(ref reader, provider, out result);

      // Fragmented but not escaped: assemble bytes, then transcode
      if (reader.HasValueSequence)
         return ValidateFragmentedUnescaped<T, TValidationError>(ref reader, provider, out result);

      // Fast path: contiguous, unescaped, short value (most common case)
      var utf8Bytes = reader.ValueSpan;

      if (utf8Bytes.Length <= _STACKALLOC_CHAR_THRESHOLD)
      {
         // Constant size enables JIT to emit a simple stack bump instead of localloc
         Span<char> charBuf = stackalloc char[_STACKALLOC_CHAR_THRESHOLD];
         var charsWritten = Encoding.UTF8.GetChars(utf8Bytes, charBuf);
         return T.Validate(charBuf[..charsWritten], provider, out result);
      }

      // Large contiguous unescaped value
      return ValidateLargeContiguousValue<T, TValidationError>(utf8Bytes, provider, out result);
   }

   // NoInlining: keeps the try/finally and ArrayPool machinery out of ValidateFromUtf8's inlined body,
   // ensuring the JIT emits compact native code for the hot stackalloc path.
   [MethodImpl(MethodImplOptions.NoInlining)]
   private static TValidationError? ValidateLargeContiguousValue<T, TValidationError>(
      ReadOnlySpan<byte> utf8Bytes,
      IFormatProvider? provider,
      out T? result)
      where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      where TValidationError : class, IValidationError<TValidationError>
   {
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

   // NoInlining: fragmented values are rare; isolating this keeps ValidateFromUtf8's
   // inlined body compact and avoids polluting the caller with ArrayPool<byte> machinery.
   [MethodImpl(MethodImplOptions.NoInlining)]
   private static TValidationError? ValidateFragmentedUnescaped<T, TValidationError>(
      ref Utf8JsonReader reader,
      IFormatProvider? provider,
      out T? result)
      where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      where TValidationError : class, IValidationError<TValidationError>
   {
      var sequence = reader.ValueSequence;
      var byteLength = checked((int)sequence.Length);

      var rentedBytes = ArrayPool<byte>.Shared.Rent(byteLength);

      try
      {
         sequence.CopyTo(rentedBytes);
         var utf8Bytes = rentedBytes.AsSpan(0, byteLength);

         // Now we have contiguous bytes — same transcoding logic as the contiguous unescaped path
         if (byteLength <= _STACKALLOC_CHAR_THRESHOLD)
         {
            Span<char> charBuf = stackalloc char[_STACKALLOC_CHAR_THRESHOLD];
            var charsWritten = Encoding.UTF8.GetChars(utf8Bytes, charBuf);
            return T.Validate(charBuf[..charsWritten], provider, out result);
         }

         var rentedChars = ArrayPool<char>.Shared.Rent(byteLength);

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
      finally
      {
         ArrayPool<byte>.Shared.Return(rentedBytes);
      }
   }

   // NoInlining: escaped values are the rarest case; isolating this keeps the dispatch compact.
   [MethodImpl(MethodImplOptions.NoInlining)]
   private static TValidationError? ValidateEscaped<T, TValidationError>(
      ref Utf8JsonReader reader,
      IFormatProvider? provider,
      out T? result)
      where T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>
      where TValidationError : class, IValidationError<TValidationError>
   {
      // CopyString handles both unescaping and reassembly of fragmented sequences
      var byteLength = reader.HasValueSequence
                          ? checked((int)reader.ValueSequence.Length)
                          : reader.ValueSpan.Length;

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
