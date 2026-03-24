using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

// ReSharper disable once InconsistentNaming
/// <summary>
/// Tests for issue #19: ComplexValueObject with nullable SmartEnum property throws
/// ArgumentNullException during JSON serialization when the property value is null.
/// </summary>
public class NullableSmartEnumPropertyTests : JsonTestsBase
{
   [Fact]
   public void Should_serialize_complex_value_object_with_null_string_based_smart_enum_property()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedSmartEnum\":null,\"NullableIntBasedSmartEnum\":null,\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_null_smart_enum_properties()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var json = Serialize(obj);
      var deserialized = Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(json);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_null_string_based_smart_enum_and_non_null_int_based()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, SmartEnum_IntBased.Item1, 42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedSmartEnum\":null,\"NullableIntBasedSmartEnum\":1,\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_non_null_string_based_and_null_int_based_smart_enum()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(SmartEnum_StringBased.Item1, null, 42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedSmartEnum\":\"Item1\",\"NullableIntBasedSmartEnum\":null,\"OtherProperty\":42}");
   }

   // ── Regression tests (expected to pass) ──

   [Fact]
   public void Should_serialize_complex_value_object_with_non_null_smart_enum_properties()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(SmartEnum_StringBased.Item1, SmartEnum_IntBased.Item2, 42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedSmartEnum\":\"Item1\",\"NullableIntBasedSmartEnum\":2,\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_non_null_smart_enum_properties()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(SmartEnum_StringBased.Item1, SmartEnum_IntBased.Item2, 42);

      var json = Serialize(obj);
      var deserialized = Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(json);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_skip_null_smart_enum_properties_when_ignore_condition_is_WhenWritingNull()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var json = Serialize(obj, null, JsonIgnoreCondition.WhenWritingNull);

      json.Should().Be("{\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_skip_null_smart_enum_properties_when_ignore_condition_is_WhenWritingDefault()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var json = Serialize(obj, null, JsonIgnoreCondition.WhenWritingDefault);

      json.Should().Be("{\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_serialize_null_complex_value_object_with_nullable_smart_enums_as_null()
   {
      var json = Serialize<ComplexValueObjectWithNullableSmartEnumProperty>(null);

      json.Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_null_smart_enum_properties_from_json()
   {
      var json = "{\"NullableStringBasedSmartEnum\":null,\"NullableIntBasedSmartEnum\":null,\"OtherProperty\":42}";

      var deserialized = Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(json);

      deserialized.NullableStringBasedSmartEnum.Should().BeNull();
      deserialized.NullableIntBasedSmartEnum.Should().BeNull();
      deserialized.OtherProperty.Should().Be(42);
   }

   [Fact]
   public void Should_serialize_with_camel_case_naming_policy()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var json = Serialize(obj, JsonNamingPolicy.CamelCase);

      json.Should().Be("{\"nullableStringBasedSmartEnum\":null,\"nullableIntBasedSmartEnum\":null,\"otherProperty\":42}");
   }
}
