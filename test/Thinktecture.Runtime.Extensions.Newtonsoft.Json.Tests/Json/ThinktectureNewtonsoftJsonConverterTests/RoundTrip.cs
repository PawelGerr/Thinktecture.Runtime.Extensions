using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Json;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestRegularUnions;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests;

public class RoundTrip : JsonTestsBase
{
   [Fact]
   public void Should_roundtrip_serialize_dictionary_with_string_based_enum_key()
   {
      var dictionary = new Dictionary<SmartEnum_StringBased, int>
                       {
                          { SmartEnum_StringBased.Item1, 1 },
                          { SmartEnum_StringBased.Item2, 2 }
                       };

      var json = JsonConvert.SerializeObject(dictionary);
      var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<SmartEnum_StringBased, int>>(json);

      dictionary.Should().BeEquivalentTo(deserializedDictionary);
   }

   [Fact]
   public void Should_roundtrip_serialize_dictionary_with_string_based_value_objects()
   {
      var dictionary = new Dictionary<StringBasedStructValueObject, int>
                       {
                          { (StringBasedStructValueObject)"key 1", 1 },
                          { (StringBasedStructValueObject)"key 2", 2 }
                       };

      var json = JsonConvert.SerializeObject(dictionary);
      var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<StringBasedStructValueObject, int>>(json);

      dictionary.Should().BeEquivalentTo(deserializedDictionary);
   }

   public static IEnumerable<object[]> ObjectWithStructTestData =
   [
      [new { Prop = IntBasedStructValueObject.Create(42) }, """{"Prop":42}"""],
      [new { Prop = (IntBasedStructValueObject?)IntBasedStructValueObject.Create(42) }, """{"Prop":42}"""],
      [new { Prop = IntBasedReferenceValueObject.Create(42) }, """{"Prop":42}"""],
      [new TestStruct<IntBasedStructValueObject>(IntBasedStructValueObject.Create(42)), """{"Prop":42}"""],
      [new TestStruct<IntBasedStructValueObject?>(IntBasedStructValueObject.Create(42)), """{"Prop":42}"""],
      [new TestStruct<IntBasedReferenceValueObject>(IntBasedReferenceValueObject.Create(42)), """{"Prop":42}"""],
   ];

   [Theory]
   [MemberData(nameof(ObjectWithStructTestData))]
   public void Should_roundtrip_serialize_types_with_struct_properties_using_non_generic_factory(
      object obj,
      string expectedJson)
   {
      var options = new JsonSerializerSettings { Converters = { new ThinktectureNewtonsoftJsonConverterFactory() } };

      var json = JsonConvert.SerializeObject(obj, options);
      json.Should().Be(expectedJson);

      var deserializedObj = JsonConvert.DeserializeObject(json, obj.GetType(), options);
      obj.Should().BeEquivalentTo(deserializedObj);
   }

   [Theory]
   [InlineData("2025", 2025, null, null)]
   [InlineData("2025-06", 2025, 6, null)]
   [InlineData("2025-06-19", 2025, 6, 19)]
   public void Should_roundtrip_PartiallyKnownDateSerializable(string value, int year, int? month, int? day)
   {
      var obj = value.Split('-').Length switch
      {
         1 => (PartiallyKnownDateSerializable)new PartiallyKnownDateSerializable.YearOnly(year),
         2 => new PartiallyKnownDateSerializable.YearMonth(year, month!.Value),
         3 => new PartiallyKnownDateSerializable.Date(year, month!.Value, day!.Value),
         _ => throw new System.Exception("Invalid test data")
      };

      var json = JsonConvert.SerializeObject(obj);
      json.Should().Be($"\"{value}\"");

      var deserialized = JsonConvert.DeserializeObject<PartiallyKnownDateSerializable>(json);
      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_regular_union_with_factory()
   {
      var json = JsonConvert.SerializeObject((PartiallyKnownDateSerializable)null);
      json.Should().Be("null");

      var deserialized = JsonConvert.DeserializeObject<PartiallyKnownDateSerializable>(json);
      deserialized.Should().BeNull();
   }

   [Fact]
   public void Should_roundtrip_using_custom_factory_specified_by_ObjectFactoryAttribute()
   {
      var original = BoundaryWithFactories.Create(1, 2);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"1:2\"");

      var deserialized = JsonConvert.DeserializeObject<BoundaryWithFactories>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_enum_with_ValidationErrorAttribute()
   {
      var original = TestSmartEnum_CustomError.Item1;
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"item1\"");

      var deserialized = JsonConvert.DeserializeObject<TestSmartEnum_CustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_simple_value_object_with_ValidationErrorAttribute()
   {
      var original = StringBasedReferenceValueObjectWithCustomError.Create("value");
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"value\"");

      var deserialized = JsonConvert.DeserializeObject<StringBasedReferenceValueObjectWithCustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_ValidationErrorAttribute()
   {
      var original = BoundaryWithCustomError.Create(1, 2);
      var json = JsonConvert.SerializeObject(original, new JsonSerializerSettings
                                                       {
                                                          ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
                                                       });

      json.Should().Be("{\"lower\":1.0,\"upper\":2.0}");

      var deserialized = JsonConvert.DeserializeObject<BoundaryWithCustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_keyed_value_object_having_custom_factory()
   {
      var original = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("1");

      var deserialized = JsonConvert.DeserializeObject<IntBasedReferenceValueObjectWithCustomFactoryNames>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_having_custom_factory()
   {
      var original = BoundaryWithCustomFactoryNames.Get(1, 2);
      var json = JsonConvert.SerializeObject(original);

      var deserialized = JsonConvert.DeserializeObject<BoundaryWithCustomFactoryNames>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   private struct TestStruct<T>
   {
      public T Prop { get; set; }

      public TestStruct(T prop)
      {
         Prop = prop;
      }
   }
}
