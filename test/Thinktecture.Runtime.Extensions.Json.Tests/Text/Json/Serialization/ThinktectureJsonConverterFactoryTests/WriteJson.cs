using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
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
      SerializeWithConverter<TestSmartEnum_Class_IntBased, ThinktectureJsonConverterFactory<TestSmartEnum_Class_IntBased, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Class_StringBased, ThinktectureJsonConverterFactory<TestSmartEnum_Class_StringBased, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_IntBased_Validatable?, ThinktectureJsonConverterFactory<TestSmartEnum_Struct_IntBased_Validatable, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_StringBased_Validatable?, ThinktectureJsonConverterFactory<TestSmartEnum_Struct_StringBased_Validatable, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_IntBased_Validatable, ThinktectureJsonConverterFactory<TestSmartEnum_Struct_IntBased_Validatable, int, ValidationError>>(default).Should().Be("0");
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
      SerializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.JsonConverterFactory>(null).Should().Be("null");
      SerializeWithConverter<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.JsonConverterFactory>(null).Should().Be("null");
      SerializeWithConverter<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.JsonConverterFactory>(default).Should().Be("{\"Property1\":null,\"Property2\":null}");
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_serialize_string_based_enum(TestEnum enumValue, string expectedJson)
   {
      var json = SerializeWithConverter<TestEnum, ThinktectureJsonConverterFactory<TestEnum, string, ValidationError>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_serialize_int_based_enum(IntegerEnum enumValue, string expectedJson)
   {
      var json = SerializeWithConverter<IntegerEnum, ThinktectureJsonConverterFactory<IntegerEnum, int, ValidationError>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_serialize_class_containing_string_based_enum(ClassWithStringBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithStringBasedEnum, ThinktectureJsonConverterFactory<TestEnum, string, ValidationError>>(classWithEnum, null, ignoreNullValues);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_serialize_class_containing_int_based_enum(ClassWithIntBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithIntBasedEnum, ThinktectureJsonConverterFactory<IntegerEnum, int, ValidationError>>(classWithEnum, null, ignoreNullValues);

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
      var json = SerializeWithConverter<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.JsonConverterFactory>(valueObject, namingPolicy, jsonIgnoreCondition);

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
      var value = Serialize<TestEnumWithCustomError, string, TestEnumValidationError>(TestEnumWithCustomError.Item1);

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
      var value = SerializeWithConverter<BoundaryWithCustomError, BoundaryWithCustomError.JsonConverterFactory>(BoundaryWithCustomError.Create(1, 2), JsonNamingPolicy.CamelCase);

      value.Should().BeEquivalentTo("{\"lower\":1,\"upper\":2}");
   }

   [Fact]
   public void Should_throw_if_non_string_based_enum_is_used_as_dictionary_key()
   {
      var dictionary = new Dictionary<TestSmartEnum_Class_IntBased, int>
                       {
                          { TestSmartEnum_Class_IntBased.Value1, 1 }
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
      var value = SerializeWithConverter<ComplexValueObjectWithObjectProperty, ComplexValueObjectWithObjectProperty.JsonConverterFactory>(obj);

      value.Should().BeEquivalentTo("{\"Property\":{\"Test\":1}}");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_JsonIgnoreAttribute()
   {
      var obj = ComplexValueObjectWithJsonIgnore.Create(
         null, null, null, null, null, null,
         0, 0, 0, 0, 0,
         null, null, null, null, null, null);

       var value = SerializeWithConverter<ComplexValueObjectWithJsonIgnore, ComplexValueObjectWithJsonIgnore.JsonConverterFactory>(obj);

       value.Should().BeEquivalentTo("{\"StringProperty_Ignore_Never\":null,\"StringProperty\":null,\"IntProperty_Ignore_Never\":0,\"IntProperty\":0,\"NullableIntProperty_Ignore_Never\":null,\"NullableIntProperty\":null}");
   }
}
