using System.Collections.Generic;
using Newtonsoft.Json;
using Thinktecture.Json;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests;

public class RoundTrip : JsonTestsBase
{
   [Fact]
   public void Should_roundtrip_serialize_dictionary_with_string_based_enum_key()
   {
      var dictionary = new Dictionary<TestSmartEnum_Class_StringBased, int>
                       {
                          { TestSmartEnum_Class_StringBased.Value1, 1 },
                          { TestSmartEnum_Class_StringBased.Value2, 2 }
                       };

      var json = JsonConvert.SerializeObject(dictionary);
      var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<TestSmartEnum_Class_StringBased, int>>(json);

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
      var options = new JsonSerializerSettings { Converters = { new ValueObjectNewtonsoftJsonConverter() } };

      var json = JsonConvert.SerializeObject(obj, options);
      json.Should().Be(expectedJson);

      var deserializedObj = JsonConvert.DeserializeObject(json, obj.GetType(), options);
      obj.Should().BeEquivalentTo(deserializedObj);
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
