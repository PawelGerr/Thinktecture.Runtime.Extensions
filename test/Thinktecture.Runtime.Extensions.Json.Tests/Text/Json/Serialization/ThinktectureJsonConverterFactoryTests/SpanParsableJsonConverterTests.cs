#if NET9_0_OR_GREATER
#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

/// <summary>
/// Tests for zero-allocation JSON deserialization using <see cref="ThinktectureSpanParsableJsonConverter{T,TValidationError}"/>.
/// These tests verify that types with <see cref="ObjectFactoryAttribute{TInput}"/> for <see cref="System.ReadOnlySpan{T}"/> of char
/// correctly use the span-based converter on NET9+.
/// </summary>
public class SpanParsableJsonConverterTests : JsonTestsBase
{
   [Fact]
   public void Should_deserialize_string_based_value_object_with_span_factory()
   {
      var value = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>("\"test-value\"");

      value.Should().NotBeNull();
      value.Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("test-value"));
   }

   [Fact]
   public void Should_deserialize_null_for_string_based_value_object_with_span_factory()
   {
      var value = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>("null");

      value.Should().BeNull();
   }

   [Fact]
   public void Should_serialize_string_based_value_object_with_span_factory()
   {
      var valueObject = StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("test-value");

      var json = Serialize(valueObject);

      json.Should().Be("\"test-value\"");
   }

   [Fact]
   public void Should_roundtrip_string_based_value_object_with_span_factory()
   {
      var original = StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("roundtrip-test");

      var json = Serialize(original);
      var deserialized = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>(json);

      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_deserialize_string_based_value_object_with_only_span_factory()
   {
      var value = Deserialize<StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory>("\"test-value\"");

      value.Should().NotBeNull();
      value.Should().Be(StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Create("test-value"));
   }

   [Fact]
   public void Should_deserialize_null_for_string_based_value_object_with_only_span_factory()
   {
      var value = Deserialize<StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory>("null");

      value.Should().BeNull();
   }

   [Fact]
   public void Should_serialize_string_based_value_object_with_only_span_factory()
   {
      var valueObject = StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Create("test-value");

      var json = Serialize(valueObject);

      json.Should().Be("\"test-value\"");
   }

   [Fact]
   public void Should_roundtrip_string_based_value_object_with_only_span_factory()
   {
      var original = StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory.Create("roundtrip-test");

      var json = Serialize(original);
      var deserialized = Deserialize<StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory>(json);

      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_deserialize_value_object_with_case_insensitive_comparison()
   {
      // These types use StringOrdinalIgnoreCase comparer
      var value1 = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>("\"TEST\"");
      var value2 = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>("\"test\"");

      // Values should be equal due to case-insensitive comparison
      value1.Should().Be(value2);
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_span_converter_for_string_based_smart_enum()
   {
      // Use skipObjectsWithJsonConverterAttribute: false to test converter selection
      // (types have [JsonConverterAttribute] from source generator)
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(SmartEnum_StringBased));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(SmartEnum_StringBased), options);

      // The converter should be ThinktectureSpanParsableJsonConverter on NET9+
      converter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<SmartEnum_StringBased, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_span_converter_for_string_based_value_object_with_span_factory()
   {
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory), options);

      // The converter should be ThinktectureSpanParsableJsonConverter on NET9+
      converter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_regular_converter_for_string_based_value_object_without_span_factory()
   {
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(StringBasedReferenceValueObject));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(StringBasedReferenceValueObject), options);

      // Without [ObjectFactory<ReadOnlySpan<char>>], should use regular converter
      converter.Should().BeOfType<ThinktectureJsonConverter<StringBasedReferenceValueObject, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_regular_converter_for_int_based_smart_enum()
   {
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(SmartEnum_IntBased));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(SmartEnum_IntBased), options);

      // Int-based enums don't use span converter (they use ThinktectureJsonConverter<T, TKey, TValidationError>)
      converter.Should().BeOfType<ThinktectureJsonConverter<SmartEnum_IntBased, int, ValidationError>>();
   }

   [Fact]
   public void Should_use_string_based_value_object_with_span_factory_as_dictionary_key()
   {
      var key = StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("my-key");
      var dictionary = new Dictionary<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory, int>
                       {
                          [key] = 42
                       };

      var json = Serialize(dictionary);

      json.Should().Be("{\"my-key\":42}");
   }

   [Fact]
   public void Should_deserialize_dictionary_with_string_based_value_object_with_span_factory_as_key()
   {
      var json = "{\"my-key\":42}";

      var dictionary = Deserialize<Dictionary<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory, int>>(json);

      var expectedKey = StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("my-key");
      dictionary.Should().ContainKey(expectedKey);
      dictionary![expectedKey].Should().Be(42);
   }

   [Fact]
   public void Should_deserialize_string_based_smart_enum()
   {
      var value = Deserialize<SmartEnum_StringBased>("\"Item1\"");

      value.Should().NotBeNull();
      value.Should().BeSameAs(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_deserialize_null_for_string_based_smart_enum()
   {
      var value = Deserialize<SmartEnum_StringBased>("null");

      value.Should().BeNull();
   }

   [Fact]
   public void Should_serialize_string_based_smart_enum()
   {
      var json = Serialize(SmartEnum_StringBased.Item1);

      json.Should().Be("\"Item1\"");
   }

   [Fact]
   public void Should_roundtrip_string_based_smart_enum()
   {
      var json = Serialize(SmartEnum_StringBased.Item2);
      var deserialized = Deserialize<SmartEnum_StringBased>(json);

      deserialized.Should().BeSameAs(SmartEnum_StringBased.Item2);
   }

   [Fact]
   public void Should_use_string_based_smart_enum_as_dictionary_key()
   {
      var dictionary = new Dictionary<SmartEnum_StringBased, int>
                       {
                          [SmartEnum_StringBased.Item1] = 1,
                          [SmartEnum_StringBased.Item2] = 2
                       };

      var json = Serialize(dictionary);

      json.Should().Be("{\"Item1\":1,\"Item2\":2}");
   }

   [Fact]
   public void Should_deserialize_dictionary_with_string_based_smart_enum_as_key()
   {
      var json = "{\"Item1\":1,\"Item2\":2}";

      var dictionary = Deserialize<Dictionary<SmartEnum_StringBased, int>>(json);

      dictionary.Should().ContainKey(SmartEnum_StringBased.Item1);
      dictionary.Should().ContainKey(SmartEnum_StringBased.Item2);
      dictionary![SmartEnum_StringBased.Item1].Should().Be(1);
      dictionary[SmartEnum_StringBased.Item2].Should().Be(2);
   }

   [Fact]
   public void Should_deserialize_string_based_struct_value_object_with_span_factory()
   {
      var value = Deserialize<StringBasedStructValueObject_With_ReadOnlyBasedObjectFactory>("\"test-value\"");

      value.Should().Be(StringBasedStructValueObject_With_ReadOnlyBasedObjectFactory.Create("test-value"));
   }

   [Fact]
   public void Should_serialize_string_based_struct_value_object_with_span_factory()
   {
      var valueObject = StringBasedStructValueObject_With_ReadOnlyBasedObjectFactory.Create("test-value");

      var json = Serialize(valueObject);

      json.Should().Be("\"test-value\"");
   }

   [Fact]
   public void Should_roundtrip_string_based_struct_value_object_with_span_factory()
   {
      var original = StringBasedStructValueObject_With_ReadOnlyBasedObjectFactory.Create("roundtrip-test");

      var json = Serialize(original);
      var deserialized = Deserialize<StringBasedStructValueObject_With_ReadOnlyBasedObjectFactory>(json);

      deserialized.Should().Be(original);
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_span_converter_for_struct_value_object_with_span_factory()
   {
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var converter = factory.CreateConverter(typeof(StringBasedStructValueObject_With_ReadOnlyBasedObjectFactory), options);

      converter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<StringBasedStructValueObject_With_ReadOnlyBasedObjectFactory, ValidationError>>();
   }

   [Fact]
   public void Should_deserialize_value_object_with_special_characters()
   {
      var value = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>("\"hello world!@#$%\"");

      value.Should().NotBeNull();
      value.Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("hello world!@#$%"));
   }

   [Fact]
   public void Should_deserialize_value_object_with_unicode_characters()
   {
      var value = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>("\"\\u00e4\\u00f6\\u00fc\"");

      value.Should().NotBeNull();
      value.Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("\u00e4\u00f6\u00fc"));
   }

   [Fact]
   public void Should_deserialize_value_object_with_escaped_characters()
   {
      var value = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>("\"line1\\nline2\"");

      value.Should().NotBeNull();
      value.Should().Be(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("line1\nline2"));
   }

   [Fact]
   public void Should_roundtrip_value_object_with_unicode()
   {
      var original = StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory.Create("\u00e4\u00f6\u00fc\u00df");

      var json = Serialize(original);
      var deserialized = Deserialize<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory>(json);

      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_throw_for_invalid_smart_enum_value()
   {
      var act = () => Deserialize<SmartEnum_StringBased>("\"NonExistentItem\"");

      act.Should().Throw<JsonException>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_regular_converter_for_int_based_smart_enum_with_span_factory()
   {
      // Int-based smart enums use ThinktectureJsonConverter<T, TKey, TValidationError> even when they have
      // ObjectFactory<ReadOnlySpan<char>>, because the metadata reports KeyType = int (not string).
      // The span converter only applies to types where the key/conversion type is string.
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory), options);

      converter.Should().BeOfType<ThinktectureJsonConverter<SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory, int, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_regular_converter_when_skipSpanBasedDeserialization_returns_true()
   {
      var factory = new ThinktectureJsonConverterFactory(
         skipObjectsWithJsonConverterAttribute: false,
         skipSpanBasedDeserialization: _ => true);
      var options = new JsonSerializerOptions();

      var converter = factory.CreateConverter(typeof(SmartEnum_StringBased), options);

      // When callback returns true, should use regular key-type-based converter
      converter.Should().BeOfType<ThinktectureJsonConverter<SmartEnum_StringBased, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_span_converter_when_skipSpanBasedDeserialization_returns_false()
   {
      var factory = new ThinktectureJsonConverterFactory(
         skipObjectsWithJsonConverterAttribute: false,
         skipSpanBasedDeserialization: _ => false);
      var options = new JsonSerializerOptions();

      var converter = factory.CreateConverter(typeof(SmartEnum_StringBased), options);

      // When callback returns false, should use span-based converter
      converter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<SmartEnum_StringBased, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_span_converter_when_skipSpanBasedDeserialization_is_null()
   {
      var factory = new ThinktectureJsonConverterFactory(
         skipObjectsWithJsonConverterAttribute: false,
         skipSpanBasedDeserialization: null);
      var options = new JsonSerializerOptions();

      var converter = factory.CreateConverter(typeof(SmartEnum_StringBased), options);

      // When callback is null (default), should use span-based converter
      converter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<SmartEnum_StringBased, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_skipSpanBasedDeserialization_should_receive_correct_type()
   {
      Type? receivedType = null;
      var factory = new ThinktectureJsonConverterFactory(
         skipObjectsWithJsonConverterAttribute: false,
         skipSpanBasedDeserialization: type =>
         {
            receivedType = type;
            return false;
         });
      var options = new JsonSerializerOptions();

      factory.CreateConverter(typeof(SmartEnum_StringBased), options);

      receivedType.Should().Be(typeof(SmartEnum_StringBased));
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_regular_converter_for_smart_enum_with_disabled_span_conversion()
   {
      // SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion has DisableSpanBasedJsonConversion = true,
      // so the source generator emits ThinktectureJsonConverterFactory<T, TValidationError> (non-span).
      // The runtime factory must make the same decision.
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion), options);

      // Must use regular converter (same as source generator decision)
      converter.Should().BeOfType<ThinktectureJsonConverter<SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_span_converter_for_value_object_with_only_span_factory()
   {
      // StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory has [ObjectFactory<ReadOnlySpan<char>>]
      // so the source generator emits ThinktectureSpanParsableJsonConverterFactory (span).
      // The runtime factory must make the same decision.
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory), options);

      // Must use span converter (same as source generator decision)
      converter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory, ValidationError>>();
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_should_use_span_converter_for_complex_value_object_with_span_factory()
   {
      // ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson has [ObjectFactory<ReadOnlySpan<char>>]
      // so the source generator emits ThinktectureSpanParsableJsonConverterFactory (span).
      // The runtime factory must make the same decision.
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var options = new JsonSerializerOptions();

      var canConvert = factory.CanConvert(typeof(ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson));
      canConvert.Should().BeTrue();

      var converter = factory.CreateConverter(typeof(ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson), options);

      // Must use span converter (same as source generator decision)
      converter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson, ValidationError>>();
   }

   [Fact]
   public void Should_deserialize_generic_string_based_smart_enum_using_span_converter()
   {
      // SmartEnum_Generic_StringBased<T> uses a generated ValueObjectJsonConverterFactory (file class)
      // which should create ThinktectureSpanParsableJsonConverter on NET9+.
      var value = Deserialize<SmartEnum_Generic_StringBased<int>>("\"item1\"");

      value.Should().NotBeNull();
      value.Should().BeSameAs(SmartEnum_Generic_StringBased<int>.Item1);
   }

   [Fact]
   public void Should_serialize_generic_string_based_smart_enum_using_span_converter()
   {
      var json = Serialize(SmartEnum_Generic_StringBased<int>.Item1);

      json.Should().Be("\"item1\"");
   }

   [Fact]
   public void Should_roundtrip_generic_string_based_smart_enum_using_span_converter()
   {
      var json = Serialize(SmartEnum_Generic_StringBased<int>.Item2);
      var deserialized = Deserialize<SmartEnum_Generic_StringBased<int>>(json);

      deserialized.Should().BeSameAs(SmartEnum_Generic_StringBased<int>.Item2);
   }

   [Fact]
   public void ThinktectureJsonConverterFactory_skipSpanBasedDeserialization_should_allow_selective_opt_out()
   {
      // Opt out only for SmartEnum_StringBased
      var factory = new ThinktectureJsonConverterFactory(
         skipObjectsWithJsonConverterAttribute: false,
         skipSpanBasedDeserialization: type => type == typeof(SmartEnum_StringBased));
      var options = new JsonSerializerOptions();

      var smartEnumConverter = factory.CreateConverter(typeof(SmartEnum_StringBased), options);
      var valueObjectConverter = factory.CreateConverter(typeof(StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory), options);

      // SmartEnum_StringBased should use regular converter (opted out)
      smartEnumConverter.Should().BeOfType<ThinktectureJsonConverter<SmartEnum_StringBased, ValidationError>>();

      // Value object should still use span converter (not opted out)
      valueObjectConverter.Should().BeOfType<ThinktectureSpanParsableJsonConverter<StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory, ValidationError>>();
   }
}
#endif
