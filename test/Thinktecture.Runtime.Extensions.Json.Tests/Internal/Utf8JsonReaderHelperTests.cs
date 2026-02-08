#if NET9_0_OR_GREATER
#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thinktecture.Internal;

namespace Thinktecture.Runtime.Tests.Internal;

/// <summary>
/// Comprehensive tests for <see cref="Utf8JsonReaderHelper"/>.
/// Exercises all code paths: fast path (unescaped, contiguous), slow path (escaped/fragmented),
/// buffer boundary conditions, multi-byte UTF-8, and edge cases.
/// </summary>
public class Utf8JsonReaderHelperTests
{
   /// <summary>
   /// Test type that captures the validated char span as a string.
   /// Always succeeds validation (returns null error).
   /// </summary>
   private sealed class SpanCapture : IObjectFactory<SpanCapture, ReadOnlySpan<char>, ValidationError>
   {
      public string CapturedValue { get; }

      public SpanCapture(string value) => CapturedValue = value;

      public static ValidationError? Validate(
         ReadOnlySpan<char> value,
         IFormatProvider? provider,
         out SpanCapture? item)
      {
         item = new SpanCapture(value.ToString());
         return null;
      }
   }

   /// <summary>
   /// Test type that always returns a validation error.
   /// </summary>
   private sealed class AlwaysFailsValidation : IObjectFactory<AlwaysFailsValidation, ReadOnlySpan<char>, ValidationError>
   {
      public static ValidationError? Validate(
         ReadOnlySpan<char> value,
         IFormatProvider? provider,
         out AlwaysFailsValidation? item)
      {
         item = null;
         return ValidationError.Create($"Validation failed for: {value.ToString()}");
      }
   }

   /// <summary>
   /// Test type that captures the IFormatProvider passed to Validate.
   /// </summary>
   private sealed class ProviderCapture : IObjectFactory<ProviderCapture, ReadOnlySpan<char>, ValidationError>
   {
      public static IFormatProvider? LastProvider { get; private set; }
      public static string? LastValue { get; private set; }

      public static void Reset()
      {
         LastProvider = null;
         LastValue = null;
      }

      public static ValidationError? Validate(
         ReadOnlySpan<char> value,
         IFormatProvider? provider,
         out ProviderCapture? item)
      {
         LastProvider = provider;
         LastValue = value.ToString();
         item = new ProviderCapture();
         return null;
      }
   }

   /// <summary>
   /// Linked list segment for building multi-segment <see cref="ReadOnlySequence{T}"/>.
   /// Used to test the fragmented data path (HasValueSequence = true).
   /// </summary>
   private sealed class MemorySegment<T> : ReadOnlySequenceSegment<T>
   {
      public MemorySegment(ReadOnlyMemory<T> memory)
      {
         Memory = memory;
      }

      public MemorySegment<T> Append(ReadOnlyMemory<T> memory)
      {
         var next = new MemorySegment<T>(memory)
         {
            RunningIndex = RunningIndex + Memory.Length
         };
         Next = next;
         return next;
      }
   }

   #region Helper Methods

   /// <summary>
   /// Creates a Utf8JsonReader from raw UTF-8 bytes and advances to the first token.
   /// </summary>
   private static Utf8JsonReader CreateReader(byte[] jsonBytes)
   {
      var reader = new Utf8JsonReader(jsonBytes);
      reader.Read();
      return reader;
   }

   /// <summary>
   /// Creates a Utf8JsonReader from a JSON string (which is UTF-8 encoded).
   /// The input should be a valid JSON value (e.g., "\"hello\"").
   /// </summary>
   private static Utf8JsonReader CreateReader(string json)
   {
      return CreateReader(Encoding.UTF8.GetBytes(json));
   }

   /// <summary>
   /// Creates a Utf8JsonReader for a string value without JSON escaping non-ASCII characters.
   /// The value is wrapped in quotes and the raw UTF-8 bytes are used.
   /// This ensures the fast path is taken (ValueIsEscaped = false).
   /// WARNING: The value must not contain characters that require JSON escaping (", \, control chars).
   /// </summary>
   private static Utf8JsonReader CreateUnescapedReader(string value)
   {
      // Build JSON bytes manually: quote + UTF-8(value) + quote
      var valueBytes = Encoding.UTF8.GetBytes(value);
      var jsonBytes = new byte[valueBytes.Length + 2];
      jsonBytes[0] = (byte)'"';
      valueBytes.CopyTo(jsonBytes, 1);
      jsonBytes[^1] = (byte)'"';
      return CreateReader(jsonBytes);
   }

   /// <summary>
   /// Creates a fragmented Utf8JsonReader by splitting the JSON bytes across multiple memory segments.
   /// The split happens at the specified byte offset within the JSON content.
   /// This forces HasValueSequence = true when the value spans segments.
   /// </summary>
   private static Utf8JsonReader CreateFragmentedReader(byte[] jsonBytes, int splitAt)
   {
      var firstPart = jsonBytes.AsMemory(0, splitAt);
      var secondPart = jsonBytes.AsMemory(splitAt);

      var first = new MemorySegment<byte>(firstPart);
      var last = first.Append(secondPart);

      var sequence = new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
      var reader = new Utf8JsonReader(sequence);
      reader.Read();
      return reader;
   }

   /// <summary>
   /// Creates a fragmented Utf8JsonReader from a JSON string, splitting at the specified byte offset.
   /// </summary>
   private static Utf8JsonReader CreateFragmentedReader(string json, int splitAt)
   {
      return CreateFragmentedReader(Encoding.UTF8.GetBytes(json), splitAt);
   }

   /// <summary>
   /// Creates a fragmented Utf8JsonReader by splitting into many small segments (1-byte each).
   /// This is the most extreme fragmentation scenario.
   /// </summary>
   private static Utf8JsonReader CreateMaxFragmentedReader(byte[] jsonBytes)
   {
      if (jsonBytes.Length < 2)
         return CreateReader(jsonBytes);

      var first = new MemorySegment<byte>(jsonBytes.AsMemory(0, 1));
      var current = first;

      for (int i = 1; i < jsonBytes.Length; i++)
      {
         current = current.Append(jsonBytes.AsMemory(i, 1));
      }

      var sequence = new ReadOnlySequence<byte>(first, 0, current, current.Memory.Length);
      var reader = new Utf8JsonReader(sequence);
      reader.Read();
      return reader;
   }

   /// <summary>
   /// Creates a JSON-encoded string using System.Text.Json serialization.
   /// Non-ASCII characters will be escaped as \uXXXX by the default encoder.
   /// </summary>
   private static string ToEscapedJson(string value)
   {
      return JsonSerializer.Serialize(value);
   }

   /// <summary>
   /// Creates a string by repeating the given string a specified number of times.
   /// </summary>
   private static string Repeat(string s, int count)
   {
      return string.Concat(Enumerable.Repeat(s, count));
   }

   #endregion

   // ==========================================================================
   // BASIC FUNCTIONALITY TESTS
   // ==========================================================================

   [Fact]
   public void Should_validate_empty_string()
   {
      var reader = CreateReader("\"\"");

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result.Should().NotBeNull();
      result!.CapturedValue.Should().BeEmpty();
   }

   [Fact]
   public void Should_validate_simple_ascii_string()
   {
      var reader = CreateReader("\"hello\"");

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result.Should().NotBeNull();
      result!.CapturedValue.Should().Be("hello");
   }

   [Fact]
   public void Should_validate_single_character()
   {
      var reader = CreateReader("\"A\"");

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("A");
   }

   [Fact]
   public void Should_return_validation_error_from_factory()
   {
      var reader = CreateReader("\"test-value\"");

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<AlwaysFailsValidation, ValidationError>(
         ref reader, null, out var result);

      error.Should().NotBeNull();
      error!.Message.Should().Be("Validation failed for: test-value");
      result.Should().BeNull();
   }

   [Fact]
   public void Should_forward_format_provider_to_validate()
   {
      ProviderCapture.Reset();
      var reader = CreateReader("\"test\"");
      var provider = CultureInfo.GetCultureInfo("de-DE");

      Utf8JsonReaderHelper.ValidateFromUtf8<ProviderCapture, ValidationError>(
         ref reader, provider, out _);

      ProviderCapture.LastProvider.Should().BeSameAs(provider);
      ProviderCapture.LastValue.Should().Be("test");
   }

   [Fact]
   public void Should_forward_null_format_provider()
   {
      ProviderCapture.Reset();
      var reader = CreateReader("\"test\"");

      Utf8JsonReaderHelper.ValidateFromUtf8<ProviderCapture, ValidationError>(
         ref reader, null, out _);

      ProviderCapture.LastProvider.Should().BeNull();
   }

   // ==========================================================================
   // FAST PATH: BUFFER BOUNDARY TESTS
   // These test the two branches in ValidateFastPath:
   //   - Stackalloc: utf8Bytes.Length <= 128 → stackalloc char[128]
   //   - ArrayPool:  utf8Bytes.Length > 128  → ArrayPool<char>
   // ==========================================================================

   [Theory]
   [InlineData(1)]
   [InlineData(10)]
   [InlineData(32)]
   [InlineData(64)]
   [InlineData(100)]
   [InlineData(127)]
   [InlineData(128)]  // Exactly _STACKALLOC_CHAR_THRESHOLD boundary
   public void Should_handle_ascii_in_stackalloc_path(int length)
   {
      var value = new string('X', length);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Theory]
   [InlineData(129)]   // Just over stackalloc threshold
   [InlineData(256)]
   [InlineData(512)]
   [InlineData(1024)]
   [InlineData(4096)]
   [InlineData(10000)]
   public void Should_handle_ascii_in_array_pool_path(int length)
   {
      var value = new string('Z', length);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   // ==========================================================================
   // FAST PATH: MULTI-BYTE UTF-8 TESTS
   // These exercise the fast path with non-ASCII characters that are encoded
   // as multi-byte UTF-8 sequences but do NOT use JSON escaping.
   // ==========================================================================

   [Fact]
   public void Should_handle_2_byte_utf8_characters()
   {
      // é (U+00E9) = 2 bytes in UTF-8, 1 char in UTF-16
      var reader = CreateUnescapedReader("café");

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("café");
   }

   [Fact]
   public void Should_handle_3_byte_utf8_characters()
   {
      // 日 (U+65E5) = 3 bytes in UTF-8, 1 char in UTF-16
      var reader = CreateUnescapedReader("日本語");

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("日本語");
   }

   [Fact]
   public void Should_handle_4_byte_utf8_characters()
   {
      // 😀 (U+1F600) = 4 bytes in UTF-8, 2 chars in UTF-16 (surrogate pair)
      var reader = CreateUnescapedReader("😀");

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("😀");
   }

   [Fact]
   public void Should_handle_multiple_4_byte_utf8_characters()
   {
      var value = Repeat("😀", 5) + Repeat("🎉", 3) + Repeat("🚀", 2);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_mixed_ascii_and_multibyte_utf8()
   {
      var value = "hello 日本 café 😀 world";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_only_2_byte_utf8_characters()
   {
      // String of only 2-byte UTF-8 chars
      var value = "àáâãäåæçèéêëìíîï";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_only_3_byte_utf8_characters()
   {
      // String of only 3-byte UTF-8 chars (CJK)
      var value = "漢字仮名片仮名平仮名";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   // ==========================================================================
   // FAST PATH: MULTI-BYTE AT BUFFER BOUNDARIES
   // These test whether the buffer is correctly sized when multi-byte UTF-8
   // characters place the byte count at the 128-byte threshold boundary while
   // the char count is different from the byte count.
   // ==========================================================================

   [Fact]
   public void Should_handle_2_byte_chars_filling_exactly_128_bytes()
   {
      // 64 × é (2 bytes each) = 128 bytes UTF-8, 64 chars UTF-16
      // Uses stackalloc (128 chars): needs 64 chars → should fit
      var value = new string('é', 64);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_2_byte_chars_just_over_128_bytes()
   {
      // 65 × é (2 bytes each) = 130 bytes UTF-8, 65 chars UTF-16
      // Uses ArrayPool: needs 65 chars → should fit
      var value = new string('é', 65);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_3_byte_chars_just_under_128_bytes()
   {
      // 42 × 日 (3 bytes each) = 126 bytes UTF-8, 42 chars UTF-16
      // Uses stackalloc (128 chars): needs 42 chars → should fit
      var value = new string('日', 42);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_3_byte_chars_just_over_128_bytes()
   {
      // 43 × 日 (3 bytes each) = 129 bytes UTF-8, 43 chars UTF-16
      // Uses ArrayPool: needs 43 chars → should fit
      var value = new string('日', 43);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_4_byte_chars_filling_exactly_128_bytes()
   {
      // 32 × 😀 (4 bytes each) = 128 bytes UTF-8, 64 chars UTF-16 (surrogate pairs)
      // Uses stackalloc (128 chars): needs 64 chars → should fit
      var value = Repeat("😀", 32);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_4_byte_chars_just_over_128_bytes()
   {
      // 33 × 😀 (4 bytes each) = 132 bytes UTF-8, 66 chars UTF-16
      // Uses ArrayPool: needs 66 chars → should fit
      var value = Repeat("😀", 33);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_mixed_byte_widths_crossing_128_byte_boundary()
   {
      // 40 ASCII (40 bytes) + 20 × é (40 bytes) + 16 × 日 (48 bytes) = 128 bytes exactly
      // Chars: 40 + 20 + 16 = 76 chars in UTF-16, buffer is 128 chars → should fit
      var value = new string('A', 40) + new string('é', 20) + new string('日', 16);
      var utf8Len = Encoding.UTF8.GetByteCount(value);
      // Verify our math: should be 128 bytes
      utf8Len.Should().Be(128);

      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   // ==========================================================================
   // SLOW PATH: ESCAPED CHARACTER TESTS
   // When the JSON value contains escape sequences, the reader sets
   // ValueIsEscaped = true, routing to ValidateSlowPath.
   // ==========================================================================

   [Fact]
   public void Should_handle_escaped_newline()
   {
      // JSON: "line1\nline2" (with literal backslash-n in JSON text)
      var jsonBytes = Encoding.UTF8.GetBytes("\"line1\\nline2\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("line1\nline2");
   }

   [Fact]
   public void Should_handle_escaped_tab()
   {
      var jsonBytes = Encoding.UTF8.GetBytes("\"col1\\tcol2\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("col1\tcol2");
   }

   [Fact]
   public void Should_handle_escaped_backslash()
   {
      var jsonBytes = Encoding.UTF8.GetBytes("\"path\\\\to\\\\file\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("path\\to\\file");
   }

   [Fact]
   public void Should_handle_escaped_quote()
   {
      var jsonBytes = Encoding.UTF8.GetBytes("\"say \\\"hello\\\"\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("say \"hello\"");
   }

   [Fact]
   public void Should_handle_escaped_forward_slash()
   {
      var jsonBytes = Encoding.UTF8.GetBytes("\"a\\/b\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("a/b");
   }

   [Fact]
   public void Should_handle_all_json_escape_types()
   {
      // All standard JSON escapes: \" \\ \/ \b \f \n \r \t
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\\"\\\\\\b\\f\\n\\r\\t\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("\"\\\b\f\n\r\t");
   }

   [Fact]
   public void Should_handle_unicode_escape_for_bmp_character()
   {
      // \u00E9 = é (U+00E9, Basic Multilingual Plane)
      var jsonBytes = Encoding.UTF8.GetBytes("\"caf\\u00E9\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("café");
   }

   [Fact]
   public void Should_handle_unicode_escape_for_cjk_character()
   {
      // \u65E5 = 日 (U+65E5)
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\u65E5\\u672C\\u8A9E\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("日本語");
   }

   [Fact]
   public void Should_handle_surrogate_pair_unicode_escape()
   {
      // 😀 (U+1F600) as surrogate pair: \uD83D\uDE00
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\uD83D\\uDE00\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("😀");
   }

   [Fact]
   public void Should_handle_many_unicode_escapes()
   {
      // 50 × \u0041 (= "A") = 300 raw bytes, 50 chars after unescaping
      // Raw byte length = 300, which is > 128 → uses ArrayPool in slow path
      var jsonBuilder = new StringBuilder("\"");
      for (int i = 0; i < 50; i++)
         jsonBuilder.Append("\\u0041");
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(new string('A', 50));
   }

   [Fact]
   public void Should_handle_mixed_escaped_and_unescaped()
   {
      // Mix of plain ASCII, escaped chars, and unicode escapes
      var jsonBytes = Encoding.UTF8.GetBytes("\"hello\\nworld\\u0021\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("hello\nworld!");
   }

   [Fact]
   public void Should_handle_escaped_string_at_slow_path_boundary()
   {
      // Create an escaped string whose raw byte length is exactly 128
      // (at _STACKALLOC_CHAR_THRESHOLD boundary in slow path)
      // Use 64 × \n (each is 2 raw bytes) = 128 raw bytes
      var jsonBuilder = new StringBuilder("\"");
      for (int i = 0; i < 64; i++)
         jsonBuilder.Append("\\n");
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(new string('\n', 64));
   }

   [Fact]
   public void Should_handle_escaped_string_just_over_slow_path_boundary()
   {
      // 65 × \n = 130 raw bytes, just over 128 → ArrayPool in slow path
      var jsonBuilder = new StringBuilder("\"");
      for (int i = 0; i < 65; i++)
         jsonBuilder.Append("\\n");
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(new string('\n', 65));
   }

   // ==========================================================================
   // SLOW PATH: FRAGMENTED SEQUENCE TESTS
   // When the reader is created from a ReadOnlySequence<byte> and the value
   // spans multiple segments, HasValueSequence = true.
   // ==========================================================================

   [Fact]
   public void Should_handle_simple_fragmented_sequence()
   {
      // Split "hello" across two segments
      var json = "\"hello\"";
      // Split in the middle of the value: "hel | lo"
      var reader = CreateFragmentedReader(json, 4); // After the opening quote + "hel"

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("hello");
   }

   [Fact]
   public void Should_handle_fragmented_at_different_split_points()
   {
      var json = "\"abcdefgh\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      // Test splitting at every possible point within the value
      for (int splitAt = 2; splitAt < jsonBytes.Length - 1; splitAt++)
      {
         var reader = CreateFragmentedReader(jsonBytes, splitAt);

         var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
            ref reader, null, out var result);

         error.Should().BeNull($"Split at byte {splitAt} should succeed");
         result!.CapturedValue.Should().Be("abcdefgh", $"Split at byte {splitAt} should produce correct value");
      }
   }

   [Fact]
   public void Should_handle_maximally_fragmented_sequence()
   {
      // Split into individual bytes (1-byte segments)
      var json = "\"hello world\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);
      var reader = CreateMaxFragmentedReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("hello world");
   }

   [Fact]
   public void Should_handle_fragmented_multibyte_utf8()
   {
      // Fragment a string with multi-byte UTF-8 chars
      var value = "café";
      var json = "\"" + value + "\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      // Split in the middle of the é character (which is 2 bytes: 0xC3 0xA9)
      // The 'é' starts at byte offset 4 (after quote + "caf")
      // Split between the two bytes of é: after 0xC3
      var reader = CreateFragmentedReader(jsonBytes, 5); // After quote + "caf" + first byte of é

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("café");
   }

   [Fact]
   public void Should_handle_fragmented_with_escape_sequences()
   {
      // Fragment a JSON string that contains escape sequences
      var json = "\"hello\\nworld\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      // Split within the escape sequence: between \ and n
      // The \ is at byte 6 (after quote + "hello"), n is at byte 7
      var reader = CreateFragmentedReader(jsonBytes, 7);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("hello\nworld");
   }

   [Fact]
   public void Should_handle_fragmented_with_unicode_escape()
   {
      // Fragment within a \uXXXX escape sequence
      var json = "\"caf\\u00E9\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      // Split within \u00E9: between 'u' and '0'
      var reader = CreateFragmentedReader(jsonBytes, 6); // After "caf\" + "u"

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("café");
   }

   [Fact]
   public void Should_handle_large_fragmented_sequence()
   {
      // Large string that requires ArrayPool in slow path
      var value = new string('Q', 500);
      var json = "\"" + value + "\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      // Split roughly in the middle
      var reader = CreateFragmentedReader(jsonBytes, 250);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_fragmented_empty_string()
   {
      // Empty string fragmented: opening and closing quotes in different segments
      var json = "\"\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      var reader = CreateFragmentedReader(jsonBytes, 1); // Split between the two quotes

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().BeEmpty();
   }

   // ==========================================================================
   // CONSISTENCY TESTS: Compare with reader.GetString()
   // Verify that ValidateFromUtf8 produces the same result as GetString()
   // for various inputs.
   // ==========================================================================

   public static IEnumerable<object[]> ConsistencyTestData()
   {
      yield return ["simple ascii", "hello world"];
      yield return ["empty", ""];
      yield return ["single char", "X"];
      yield return ["digits", "1234567890"];
      yield return ["2-byte utf8", "café résumé naïve"];
      yield return ["3-byte utf8", "日本語テスト"];
      yield return ["4-byte utf8", Repeat("😀", 3) + " hello"];
      yield return ["mixed encodings", "Hello 世界 café 🚀!"];
      yield return ["spaces only", "     "];
      yield return ["128 chars", new string('A', 128)];
      yield return ["129 chars", new string('B', 129)];
      yield return ["256 chars", new string('C', 256)];
   }

   [Theory]
   [MemberData(nameof(ConsistencyTestData))]
   public void Should_produce_same_result_as_GetString(string label, string value)
   {
      _ = label; // Used for test identification

      // Use JsonSerializer to properly encode the value (may include escapes)
      var json = JsonSerializer.Serialize(value);
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      // Get the expected value from reader.GetString()
      var getStringReader = new Utf8JsonReader(jsonBytes);
      getStringReader.Read();
      var expected = getStringReader.GetString();

      // Get the actual value from ValidateFromUtf8
      var validateReader = new Utf8JsonReader(jsonBytes);
      validateReader.Read();
      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref validateReader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(expected, $"Consistency check failed for: {label}");
   }

   [Theory]
   [MemberData(nameof(ConsistencyTestData))]
   public void Should_produce_same_result_as_GetString_with_unescaped_utf8(string label, string value)
   {
      _ = label;

      // Build JSON bytes with raw UTF-8 (no JSON escaping of non-ASCII)
      var valueBytes = Encoding.UTF8.GetBytes(value);
      var jsonBytes = new byte[valueBytes.Length + 2];
      jsonBytes[0] = (byte)'"';
      valueBytes.CopyTo(jsonBytes, 1);
      jsonBytes[^1] = (byte)'"';

      // Get expected from GetString
      var getStringReader = new Utf8JsonReader(jsonBytes);
      getStringReader.Read();
      var expected = getStringReader.GetString();

      // Get actual from ValidateFromUtf8
      var validateReader = new Utf8JsonReader(jsonBytes);
      validateReader.Read();
      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref validateReader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(expected, $"Unescaped consistency check failed for: {label}");
   }

   // ==========================================================================
   // EDGE CASES
   // ==========================================================================

   [Fact]
   public void Should_handle_embedded_null_character_via_unicode_escape()
   {
      // \u0000 is a valid JSON escape for the null character
      var jsonBytes = Encoding.UTF8.GetBytes("\"hello\\u0000world\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("hello\0world");
      result.CapturedValue.Length.Should().Be(11);
   }

   [Fact]
   public void Should_handle_string_of_only_spaces()
   {
      var value = new string(' ', 100);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_very_long_ascii_string()
   {
      // 100K character string → uses ArrayPool path
      var value = new string('V', 100_000);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_very_long_multibyte_string()
   {
      // 50K × 3-byte char = 150K bytes → uses ArrayPool path
      var value = new string('日', 50_000);
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_very_long_escaped_string()
   {
      // 10K × \n = 20K raw bytes, 10K chars after unescaping
      var jsonBuilder = new StringBuilder("\"");
      for (int i = 0; i < 10_000; i++)
         jsonBuilder.Append("\\n");
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(new string('\n', 10_000));
   }

   [Fact]
   public void Should_handle_string_with_all_ascii_printable_characters()
   {
      // All printable ASCII characters (space through tilde, except quote and backslash)
      var sb = new StringBuilder();
      for (char c = ' '; c <= '~'; c++)
      {
         if (c != '"' && c != '\\')
            sb.Append(c);
      }

      var value = sb.ToString();
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_string_with_only_escaped_characters()
   {
      // String made entirely of characters that require JSON escaping
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\\"\\\\\\n\\r\\t\\b\\f\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("\"\\\n\r\t\b\f");
   }

   [Fact]
   public void Should_handle_repetitive_escape_sequences()
   {
      // Many consecutive escape sequences
      var jsonBuilder = new StringBuilder("\"");
      for (int i = 0; i < 20; i++)
         jsonBuilder.Append("\\n\\t\\r");
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(Repeat("\n\t\r", 20));
   }

   // ==========================================================================
   // POTENTIAL BREAKING SCENARIOS
   // These tests probe areas where the implementation MIGHT fail.
   // Tests that fail reveal limitations of the code.
   // ==========================================================================

   [Fact]
   public void Should_throw_for_lone_high_surrogate_in_unicode_escape()
   {
      // \uD800 is a lone high surrogate (not followed by a low surrogate).
      // Utf8JsonReader.CopyString() rejects invalid UTF-16 surrogate sequences,
      // which propagates as InvalidOperationException through ValidateSlowPath.
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\uD800\"");
      var reader = CreateReader(jsonBytes);

      InvalidOperationException? ex = null;

      try
      {
         Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
            ref reader, null, out _);
      }
      catch (InvalidOperationException e)
      {
         ex = e;
      }

      ex.Should().NotBeNull();
   }

   [Fact]
   public void Should_throw_for_lone_low_surrogate_in_unicode_escape()
   {
      // \uDC00 is a lone low surrogate — invalid without a preceding high surrogate.
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\uDC00\"");
      var reader = CreateReader(jsonBytes);

      InvalidOperationException? ex = null;

      try
      {
         Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
            ref reader, null, out _);
      }
      catch (InvalidOperationException e)
      {
         ex = e;
      }

      ex.Should().NotBeNull();
   }

   [Fact]
   public void Should_throw_for_reversed_surrogate_pair()
   {
      // Low surrogate followed by high surrogate (wrong order) — invalid UTF-16.
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\uDE00\\uD83D\"");
      var reader = CreateReader(jsonBytes);

      InvalidOperationException? ex = null;

      try
      {
         Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
            ref reader, null, out _);
      }
      catch (InvalidOperationException e)
      {
         ex = e;
      }

      ex.Should().NotBeNull();
   }

   [Fact]
   public void Should_throw_for_multiple_lone_surrogates()
   {
      // Multiple consecutive lone high surrogates — each one is invalid.
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\uD800\\uD801\\uD802\"");
      var reader = CreateReader(jsonBytes);

      InvalidOperationException? ex = null;

      try
      {
         Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
            ref reader, null, out _);
      }
      catch (InvalidOperationException e)
      {
         ex = e;
      }

      ex.Should().NotBeNull();
   }

   [Fact]
   public void Should_throw_for_surrogate_pair_followed_by_lone_surrogate()
   {
      // Valid surrogate pair (😀) followed by a lone high surrogate — the lone one is invalid.
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\uD83D\\uDE00\\uD800\"");
      var reader = CreateReader(jsonBytes);

      InvalidOperationException? ex = null;

      try
      {
         Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
            ref reader, null, out _);
      }
      catch (InvalidOperationException e)
      {
         ex = e;
      }

      ex.Should().NotBeNull();
   }

   [Fact]
   public void Should_handle_unicode_escape_for_null_character()
   {
      // \u0000 - the NUL character
      var jsonBytes = Encoding.UTF8.GetBytes("\"\\u0000\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result.Should().NotBeNull();
      result!.CapturedValue.Length.Should().Be(1);
      result.CapturedValue[0].Should().Be('\0');
   }

   [Fact]
   public void Should_handle_unicode_escapes_for_control_characters()
   {
      // Control characters U+0001 through U+001F (which MUST be escaped in JSON)
      var jsonBuilder = new StringBuilder("\"");
      for (int i = 1; i <= 0x1F; i++)
         jsonBuilder.Append($"\\u{i:X4}");
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result.Should().NotBeNull();
      result!.CapturedValue.Length.Should().Be(31); // U+0001 through U+001F

      for (int i = 0; i < 31; i++)
      {
         result.CapturedValue[i].Should().Be((char)(i + 1));
      }
   }

   [Fact]
   public void Should_handle_fragmented_sequence_with_surrogate_pair_escape()
   {
      // Fragment within a surrogate pair escape sequence
      // \uD83D\uDE00 = 😀
      var json = "\"\\uD83D\\uDE00\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      // Split between the two surrogate halves
      // "\" + "uD83D" = 6 bytes + opening quote = 7 bytes
      var reader = CreateFragmentedReader(jsonBytes, 7);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("😀");
   }

   [Fact]
   public void Should_handle_unicode_escape_at_boundary_size()
   {
      // Create a string where the escaped representation is exactly 128 bytes
      // Each \uXXXX is 6 bytes. 128/6 = 21.33, so 21 × 6 = 126 bytes + 2 ASCII chars = 128
      var jsonBuilder = new StringBuilder("\"AB");
      for (int i = 0; i < 21; i++)
         jsonBuilder.Append("\\u0041"); // Each \u0041 = "A"
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      // Verify: "AB" = 2 bytes + 21 × 6 = 126 bytes = 128 total value bytes
      // (excluding quotes)
      var valueByteLength = jsonBytes.Length - 2; // Subtract 2 for quotes
      valueByteLength.Should().Be(128);

      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("AB" + new string('A', 21)); // 2 plain + 21 escaped
   }

   [Fact]
   public void Should_handle_unicode_escape_just_over_boundary_size()
   {
      // 129 raw bytes of value: 3 ASCII + 21 × \u0041
      var jsonBuilder = new StringBuilder("\"ABC");
      for (int i = 0; i < 21; i++)
         jsonBuilder.Append("\\u0041");
      jsonBuilder.Append('"');

      var jsonBytes = Encoding.UTF8.GetBytes(jsonBuilder.ToString());
      var valueByteLength = jsonBytes.Length - 2;
      valueByteLength.Should().Be(129);

      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be("ABC" + new string('A', 21)); // 3 plain + 21 escaped
   }

   [Fact]
   public void Should_handle_large_fragmented_multibyte_string()
   {
      // Large string with multi-byte UTF-8 chars, fragmented into 1-byte segments
      var value = new string('日', 200); // 200 × 3 bytes = 600 bytes
      var json = "\"" + value + "\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      var reader = CreateMaxFragmentedReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_string_with_bom_characters()
   {
      // BOM (U+FEFF) is 3 bytes in UTF-8
      var value = "\uFEFF" + "test" + "\uFEFF";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_string_with_zero_width_characters()
   {
      // Zero-width joiner (U+200D, 3 bytes UTF-8) and zero-width space (U+200B, 3 bytes UTF-8)
      var value = "a\u200Db\u200Bc";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_emoji_with_zero_width_joiner_sequence()
   {
      // Family emoji: 👨‍👩‍👧‍👦 = U+1F468 U+200D U+1F469 U+200D U+1F467 U+200D U+1F466
      // This is 25 bytes in UTF-8, 11 chars in UTF-16
      var value = "👨\u200D👩\u200D👧\u200D👦";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_right_to_left_text()
   {
      // Arabic text (each char is 2 bytes in UTF-8)
      var value = "مرحبا بالعالم";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_bidirectional_text()
   {
      // Mixed LTR and RTL text
      var value = "Hello مرحبا World عالم";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_combining_characters()
   {
      // e + combining acute accent (U+0301) = é (but as 2 code points)
      // Different from the precomposed é (U+00E9) which is a single code point
      var value = "e\u0301"; // decomposed é
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
      result.CapturedValue.Length.Should().Be(2); // 2 code points (e + combining accent)
   }

   [Fact]
   public void Should_handle_musical_symbols()
   {
      // Musical symbols are in the Supplementary Multilingual Plane (4-byte UTF-8)
      // 𝄞 = U+1D11E (Musical Symbol G Clef)
      var value = "𝄞𝄢𝄡";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_max_bmp_character()
   {
      // U+FFFD (Replacement Character) is the highest commonly used BMP character
      // U+FFFF is the max BMP character
      var value = "\uFFFD\uFFFE\uFFFF";
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   [Fact]
   public void Should_handle_string_with_alternating_1_and_4_byte_chars()
   {
      // Alternating ASCII and emoji to test buffer handling with variable-width chars
      var sb = new StringBuilder();
      for (int i = 0; i < 20; i++)
      {
         sb.Append((char)('A' + (i % 26)));
         sb.Append("😀");
      }

      var value = sb.ToString();
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
         ref reader, null, out var result);

      error.Should().BeNull();
      result!.CapturedValue.Should().Be(value);
   }

   // ==========================================================================
   // CONCURRENT / STRESS TESTS
   // ==========================================================================

   [Fact]
   public async Task Should_handle_concurrent_calls_without_interference()
   {
      // Run multiple validations in parallel to verify thread safety
      // (ArrayPool is thread-safe, but let's verify no shared state issues)
      var tasks = Enumerable.Range(0, 100).Select(i =>
      {
         return Task.Run(() =>
         {
            var value = $"concurrent-test-{i}-" + new string((char)('A' + (i % 26)), 300);
            var reader = CreateUnescapedReader(value);

            var error = Utf8JsonReaderHelper.ValidateFromUtf8<SpanCapture, ValidationError>(
               ref reader, null, out var result);

            return (Error: error, Value: result?.CapturedValue, Expected: value);
         });
      }).ToArray();

      var results = await Task.WhenAll(tasks);

      foreach (var (error, actualValue, expected) in results)
      {
         error.Should().BeNull();
         actualValue.Should().Be(expected);
      }
   }

   // ==========================================================================
   // VALIDATION ERROR PATH TESTS
   // ==========================================================================

   [Fact]
   public void Should_propagate_validation_error_in_stackalloc_path()
   {
      var value = new string('A', 100); // Stackalloc path (≤ 128 bytes)
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<AlwaysFailsValidation, ValidationError>(
         ref reader, null, out var result);

      error.Should().NotBeNull();
      error!.Message.Should().Contain(value);
      result.Should().BeNull();
   }

   [Fact]
   public void Should_propagate_validation_error_in_array_pool_path()
   {
      var value = new string('C', 300); // ArrayPool path (> 128 bytes)
      var reader = CreateUnescapedReader(value);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<AlwaysFailsValidation, ValidationError>(
         ref reader, null, out var result);

      error.Should().NotBeNull();
      error!.Message.Should().Contain(value);
      result.Should().BeNull();
   }

   [Fact]
   public void Should_propagate_validation_error_in_slow_path()
   {
      // Escaped value triggers slow path
      var jsonBytes = Encoding.UTF8.GetBytes("\"test\\nvalue\"");
      var reader = CreateReader(jsonBytes);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<AlwaysFailsValidation, ValidationError>(
         ref reader, null, out var result);

      error.Should().NotBeNull();
      error!.Message.Should().Be("Validation failed for: test\nvalue");
      result.Should().BeNull();
   }

   [Fact]
   public void Should_propagate_validation_error_in_fragmented_path()
   {
      var json = "\"fragmented-test\"";
      var jsonBytes = Encoding.UTF8.GetBytes(json);
      var reader = CreateFragmentedReader(jsonBytes, 5);

      var error = Utf8JsonReaderHelper.ValidateFromUtf8<AlwaysFailsValidation, ValidationError>(
         ref reader, null, out var result);

      error.Should().NotBeNull();
      error!.Message.Should().Contain("fragmented-test");
      result.Should().BeNull();
   }
}
#endif
