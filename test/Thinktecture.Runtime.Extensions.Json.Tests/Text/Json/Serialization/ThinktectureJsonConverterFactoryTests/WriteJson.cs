using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestRegularUnions;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests.TestClasses;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

public class WriteJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties =>
   [
      [null, "null"],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}", null, JsonIgnoreCondition.Never],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}", null, JsonIgnoreCondition.WhenWritingNull],
      [ValueObjectWithMultipleProperties.Create(1, null, null!), "{\"StructProperty\":1}", null, JsonIgnoreCondition.WhenWritingDefault],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{}", null, JsonIgnoreCondition.WhenWritingDefault],
      [ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}", null, JsonIgnoreCondition.Never],
      [ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}", null, JsonIgnoreCondition.WhenWritingNull],
      [ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}", null, JsonIgnoreCondition.WhenWritingDefault],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}", null, JsonIgnoreCondition.Never],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}", null, JsonIgnoreCondition.WhenWritingNull],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}", null, JsonIgnoreCondition.WhenWritingDefault],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase, JsonIgnoreCondition.Never]
   ];

   [Fact]
   public void Should_deserialize_enum_if_null_and_default()
   {
      SerializeWithConverter<SmartEnum_IntBased, ThinktectureJsonConverterFactory<SmartEnum_IntBased, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<SmartEnum_StringBased, ThinktectureJsonConverterFactory<SmartEnum_StringBased, string, ValidationError>>(null).Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_if_null_and_default()
   {
      SerializeWithConverter<IntBasedReferenceValueObject, ThinktectureJsonConverterFactory<IntBasedReferenceValueObject, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<StringBasedReferenceValueObject, ThinktectureJsonConverterFactory<StringBasedReferenceValueObject, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<IntBasedStructValueObject?, ThinktectureJsonConverterFactory<IntBasedStructValueObject, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<StringBasedStructValueObject?, ThinktectureJsonConverterFactory<StringBasedStructValueObject, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<IntBasedStructValueObject, ThinktectureJsonConverterFactory<IntBasedStructValueObject, int, ValidationError>>(default).Should().Be("0");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      Serialize<TestValueObject_Complex_Class>(null).Should().Be("null");
      Serialize<TestValueObject_Complex_Struct?>(null).Should().Be("null");
      Serialize<TestValueObject_Complex_Struct>(default).Should().Be("{\"Property1\":null,\"Property2\":null}");
      Serialize<GenericComplexValueObjectStruct<string, int, TimeSpan>?>(null).Should().Be("null");
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_serialize_string_based_enum(SmartEnum_StringBased enumValue, string expectedJson)
   {
      var json = SerializeWithConverter<SmartEnum_StringBased, ThinktectureJsonConverterFactory<SmartEnum_StringBased, string, ValidationError>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_serialize_int_based_enum(SmartEnum_IntBased enumValue, string expectedJson)
   {
      var json = SerializeWithConverter<SmartEnum_IntBased, ThinktectureJsonConverterFactory<SmartEnum_IntBased, int, ValidationError>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_serialize_class_containing_string_based_enum(ClassWithStringBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithStringBasedEnum, ThinktectureJsonConverterFactory<SmartEnum_StringBased, string, ValidationError>>(classWithEnum, null, ignoreNullValues);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_serialize_class_containing_int_based_enum(ClassWithIntBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithIntBasedEnum, ThinktectureJsonConverterFactory<SmartEnum_IntBased, int, ValidationError>>(classWithEnum, null, ignoreNullValues);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
   public void Should_serialize_value_type_with_3_properties(
      ValueObjectWithMultipleProperties valueObject,
      string expectedJson,
      JsonNamingPolicy namingPolicy = null,
      JsonIgnoreCondition jsonIgnoreCondition = JsonIgnoreCondition.Never)
   {
      var json = Serialize(valueObject, namingPolicy, jsonIgnoreCondition);

      json.Should().Be(expectedJson);
   }

   [Fact]
   public void Should_deserialize_using_custom_factory_specified_by_ObjectFactoryAttribute()
   {
      var json = Serialize<BoundaryWithFactories, string>(BoundaryWithFactories.Create(1, 2));

      json.Should().Be("\"1:2\"");
   }

   [Fact]
   public void Should_serialize_enum_with_ValidationErrorAttribute()
   {
      var value = Serialize<TestSmartEnum_CustomError, string, CustomValidationError>(TestSmartEnum_CustomError.Item1);

      value.Should().BeEquivalentTo("\"item1\"");
   }

   [Fact]
   public void Should_serialize_simple_value_object_with_ValidationErrorAttribute()
   {
      var value = Serialize<StringBasedReferenceValueObjectWithCustomError, string, StringBasedReferenceValidationError>(StringBasedReferenceValueObjectWithCustomError.Create("value"));

      value.Should().BeEquivalentTo("\"value\"");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_ValidationErrorAttribute()
   {
      var value = Serialize(BoundaryWithCustomError.Create(1, 2), JsonNamingPolicy.CamelCase);

      value.Should().BeEquivalentTo("{\"lower\":1,\"upper\":2}");
   }

   [Fact]
   public void Should_throw_if_non_string_based_enum_is_used_as_dictionary_key()
   {
      var dictionary = new Dictionary<SmartEnum_IntBased, int>
                       {
                          { SmartEnum_IntBased.Item1, 1 }
                       };

      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      FluentActions.Invoking(() => JsonSerializer.Serialize(dictionary, options))
                   .Should().Throw<NotSupportedException>();
   }

   [Fact]
   public void Should_serialize_value_object_with_object_key()
   {
      var obj = ObjectBaseValueObject.Create(new { Test = 1 });
      var value = Serialize<ObjectBaseValueObject, object, ValidationError>(obj);

      value.Should().BeEquivalentTo("{\"Test\":1}");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_object_property()
   {
      var obj = ComplexValueObjectWithObjectProperty.Create(new { Test = 1 });
      var value = Serialize(obj);

      value.Should().BeEquivalentTo("{\"Property\":{\"Test\":1}}");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_JsonIgnoreAttribute()
   {
      var obj = ComplexValueObjectWithJsonIgnore.Create(
         null, null, null, null, null, null,
         0, 0, 0, 0, 0,
         null, null, null, null, null, null);

      var value = Serialize(obj);

      value.Should().BeEquivalentTo("{\"StringProperty_Ignore_Never\":null,\"StringProperty\":null,\"IntProperty_Ignore_Never\":0,\"IntProperty\":0,\"NullableIntProperty_Ignore_Never\":null,\"NullableIntProperty\":null}");
   }

   [Theory]
   [InlineData("2025", 2025, null, null)]
   [InlineData("2025-06", 2025, 6, null)]
   [InlineData("2025-06-19", 2025, 6, 19)]
   public void Should_serialize_regular_union_with_factory(string expectedJson, int year, int? month, int? day)
   {
      PartiallyKnownDateSerializable obj = expectedJson.Split('-').Length switch
      {
         1 => new PartiallyKnownDateSerializable.YearOnly(year),
         2 => new PartiallyKnownDateSerializable.YearMonth(year, month!.Value),
         3 => new PartiallyKnownDateSerializable.Date(year, month!.Value, day!.Value),
         _ => throw new Exception("Invalid test data")
      };
      var json = Serialize<PartiallyKnownDateSerializable, string>(obj);
      json.Should().Be($"\"{expectedJson}\"");
   }

   [Fact]
   public void Should_serialize_PartiallyKnownDateSerializable_null()
   {
      var json = Serialize<PartiallyKnownDateSerializable, string>(null);
      json.Should().Be("null");
   }

   [Fact]
   public void Should_serialize_generic_class()
   {
      var obj = GenericComplexValueObject<string, int, TimeSpan>.Create(
         "text",
         "nullable-text",
         42,
         43,
         TimeSpan.FromSeconds(10),
         TimeSpan.FromSeconds(11)
      );

      var json = JsonSerializer.Serialize(obj);

      json.Should().Be("{\"ClassProperty\":\"text\",\"NullableClassProperty\":\"nullable-text\",\"StructProperty\":42,\"NullableStructProperty\":43,\"Property\":\"00:00:10\",\"NullableProperty\":\"00:00:11\"}");
   }

   [Fact]
   public void Should_serialize_generic_struct()
   {
      var obj = GenericComplexValueObjectStruct<string, int, TimeSpan>.Create(
         "text",
         "nullable-text",
         42,
         43,
         TimeSpan.FromSeconds(10),
         TimeSpan.FromSeconds(11)
      );

      var json = JsonSerializer.Serialize(obj);

      json.Should().Be("{\"ClassProperty\":\"text\",\"NullableClassProperty\":\"nullable-text\",\"StructProperty\":42,\"NullableStructProperty\":43,\"Property\":\"00:00:10\",\"NullableProperty\":\"00:00:11\"}");
   }

   [Fact]
   public void Should_serialize_nullable_generic_struct()
   {
      GenericComplexValueObjectStruct<string, int, TimeSpan>? obj = GenericComplexValueObjectStruct<string, int, TimeSpan>.Create(
         "text",
         "nullable-text",
         42,
         43,
         TimeSpan.FromSeconds(10),
         TimeSpan.FromSeconds(11)
      );

      var json = JsonSerializer.Serialize(obj);

      json.Should().Be("{\"ClassProperty\":\"text\",\"NullableClassProperty\":\"nullable-text\",\"StructProperty\":42,\"NullableStructProperty\":43,\"Property\":\"00:00:10\",\"NullableProperty\":\"00:00:11\"}");
   }

   [Fact]
   public void Should_serialize_int_based_generic_smart_enum()
   {
      var json = JsonSerializer.Serialize(SmartEnum_Generic_IntBased<string>.Item1);

      json.Should().Be("1");
   }

   [Fact]
   public void Should_serialize_null_int_based_generic_smart_enum_as_null()
   {
      var json = JsonSerializer.Serialize<SmartEnum_Generic_IntBased<string>>(null);

      json.Should().Be("null");
   }

   [Fact]
   public void Should_round_trip_all_int_based_generic_smart_enum_items_in_WriteJson()
   {
      foreach (var original in SmartEnum_Generic_IntBased<string>.Items)
      {
         var json = JsonSerializer.Serialize(original);
         var deserialized = JsonSerializer.Deserialize<SmartEnum_Generic_IntBased<string>>(json);

         deserialized.Should().BeSameAs(original);
      }
   }

   [Fact]
   public void Should_serialize_int_based_generic_smart_enum_with_different_type_arguments()
   {
      var stringItem = SmartEnum_Generic_IntBased<string>.Item1;
      var intItem = SmartEnum_Generic_IntBased<int>.Item1;

      var stringJson = JsonSerializer.Serialize(stringItem);
      var intJson = JsonSerializer.Serialize(intItem);

      stringJson.Should().Be("1");
      intJson.Should().Be("1");
   }

   [Fact]
   public void Should_serialize_string_based_generic_smart_enum()
   {
      var json = JsonSerializer.Serialize(SmartEnum_Generic_StringBased<int>.Item1);

      json.Should().Be("\"item1\"");
   }

   [Fact]
   public void Should_serialize_null_string_based_generic_smart_enum_as_null()
   {
      var json = JsonSerializer.Serialize<SmartEnum_Generic_StringBased<int>>(null);

      json.Should().Be("null");
   }

   [Fact]
   public void Should_round_trip_all_string_based_generic_smart_enum_items_in_WriteJson()
   {
      foreach (var original in SmartEnum_Generic_StringBased<int>.Items)
      {
         var json = JsonSerializer.Serialize(original);
         var deserialized = JsonSerializer.Deserialize<SmartEnum_Generic_StringBased<int>>(json);

         deserialized.Should().BeSameAs(original);
      }
   }

   [Fact]
   public void Should_serialize_string_based_generic_smart_enum_with_different_type_arguments()
   {
      var intItem = SmartEnum_Generic_StringBased<int>.Item1;
      var doubleItem = SmartEnum_Generic_StringBased<double>.Item1;

      var intJson = JsonSerializer.Serialize(intItem);
      var doubleJson = JsonSerializer.Serialize(doubleItem);

      intJson.Should().Be("\"item1\"");
      doubleJson.Should().Be("\"item1\"");
   }
}
