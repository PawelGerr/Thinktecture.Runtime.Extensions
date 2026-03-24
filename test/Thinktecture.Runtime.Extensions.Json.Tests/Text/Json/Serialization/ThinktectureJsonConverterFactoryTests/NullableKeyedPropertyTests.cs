using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

/// <summary>
/// Tests for ComplexValueObject with nullable keyed value object properties.
/// Related to issue #19: the same root cause (null passed to converter.Write) affects
/// nullable keyed value object properties, not just nullable Smart Enums.
/// </summary>
public class NullableKeyedPropertyTests : JsonTestsBase
{
   [Fact]
   public void Should_serialize_complex_value_object_with_null_string_based_keyed_value_object_property()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(null, null, null, 42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedValueObject\":null,\"NullableIntBasedValueObject\":null,\"NullableIntBasedStructValueObject\":null,\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_null_keyed_value_object_properties()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(null, null, null, 42);

      var json = Serialize(obj);
      var deserialized = Deserialize<ComplexValueObjectWithNullableKeyedProperties>(json);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_partial_null_keyed_properties()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(
         StringBasedReferenceValueObject.Create("test"),
         null,
         null,
         42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedValueObject\":\"test\",\"NullableIntBasedValueObject\":null,\"NullableIntBasedStructValueObject\":null,\"OtherProperty\":42}");
   }

   // ── Regression tests (expected to pass) ──

   [Fact]
   public void Should_serialize_complex_value_object_with_non_null_keyed_value_object_properties()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(
         StringBasedReferenceValueObject.Create("test"),
         IntBasedReferenceValueObject.Create(1),
         IntBasedStructValueObject.Create(2),
         42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedValueObject\":\"test\",\"NullableIntBasedValueObject\":1,\"NullableIntBasedStructValueObject\":2,\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_non_null_keyed_value_object_properties()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(
         StringBasedReferenceValueObject.Create("test"),
         IntBasedReferenceValueObject.Create(1),
         IntBasedStructValueObject.Create(2),
         42);

      var json = Serialize(obj);
      var deserialized = Deserialize<ComplexValueObjectWithNullableKeyedProperties>(json);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_skip_null_keyed_properties_when_ignore_condition_is_WhenWritingNull()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(null, null, null, 42);

      var json = Serialize(obj, null, JsonIgnoreCondition.WhenWritingNull);

      json.Should().Be("{\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_skip_null_keyed_properties_when_ignore_condition_is_WhenWritingDefault()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(null, null, null, 42);

      var json = Serialize(obj, null, JsonIgnoreCondition.WhenWritingDefault);

      json.Should().Be("{\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_null_keyed_value_object_properties_from_json()
   {
      var json = "{\"NullableStringBasedValueObject\":null,\"NullableIntBasedValueObject\":null,\"NullableIntBasedStructValueObject\":null,\"OtherProperty\":42}";

      var deserialized = Deserialize<ComplexValueObjectWithNullableKeyedProperties>(json);

      deserialized.NullableStringBasedValueObject.Should().BeNull();
      deserialized.NullableIntBasedValueObject.Should().BeNull();
      deserialized.NullableIntBasedStructValueObject.Should().BeNull();
      deserialized.OtherProperty.Should().Be(42);
   }
}
