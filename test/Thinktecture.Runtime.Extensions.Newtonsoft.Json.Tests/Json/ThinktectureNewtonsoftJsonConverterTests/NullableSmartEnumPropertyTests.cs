using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests;

/// <summary>
/// Regression tests for issue #19: verify ComplexValueObject with nullable SmartEnum properties
/// serializes and deserializes correctly with Newtonsoft.Json.
/// </summary>
public class NullableSmartEnumPropertyTests
{
   [Fact]
   public void Should_serialize_complex_value_object_with_null_smart_enum_properties()
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
   public void Should_serialize_complex_value_object_with_null_string_based_and_non_null_int_based()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, SmartEnum_IntBased.Item1, 42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedSmartEnum\":null,\"NullableIntBasedSmartEnum\":1,\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_non_null_string_based_and_null_int_based()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(SmartEnum_StringBased.Item1, null, 42);

      var json = Serialize(obj);

      json.Should().Be("{\"NullableStringBasedSmartEnum\":\"Item1\",\"NullableIntBasedSmartEnum\":null,\"OtherProperty\":42}");
   }

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
   public void Should_skip_null_smart_enum_properties_when_null_value_handling_is_ignore()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var json = Serialize(obj, nullValueHandling: NullValueHandling.Ignore);

      json.Should().Be("{\"OtherProperty\":42}");
   }

   [Fact]
   public void Should_serialize_null_complex_value_object_as_null()
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
   public void Should_serialize_with_camel_case_naming_strategy()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var json = Serialize(obj, new CamelCaseNamingStrategy());

      json.Should().Be("{\"nullableStringBasedSmartEnum\":null,\"nullableIntBasedSmartEnum\":null,\"otherProperty\":42}");
   }

   private static string Serialize<T>(
      T value,
      NamingStrategy namingStrategy = null,
      NullValueHandling nullValueHandling = NullValueHandling.Include)
   {
      using var writer = new StringWriter();
      using var jsonWriter = new JsonTextWriter(writer);

      var settings = new JsonSerializerSettings
                     {
                        NullValueHandling = nullValueHandling
                     };

      if (namingStrategy is not null)
         settings.ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy };

      var serializer = JsonSerializer.CreateDefault(settings);

      serializer.Serialize(jsonWriter, value);

      return writer.GetStringBuilder().ToString();
   }

   private static T Deserialize<T>(string json)
   {
      return JsonConvert.DeserializeObject<T>(json);
   }
}
