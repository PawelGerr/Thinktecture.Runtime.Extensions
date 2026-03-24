using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests;

/// <summary>
/// Regression tests for issue #19: verify ComplexValueObject with nullable keyed value object
/// properties serializes and deserializes correctly with Newtonsoft.Json.
/// </summary>
public class NullableKeyedPropertyTests
{
   [Fact]
   public void Should_serialize_complex_value_object_with_null_keyed_value_object_properties()
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
   public void Should_skip_null_keyed_properties_when_null_value_handling_is_ignore()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(null, null, null, 42);

      var json = Serialize(obj, nullValueHandling: NullValueHandling.Ignore);

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
