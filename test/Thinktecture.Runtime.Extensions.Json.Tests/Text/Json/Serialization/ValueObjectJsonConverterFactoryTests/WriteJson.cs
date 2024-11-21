using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests;

public class WriteJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
                                                                                   {
                                                                                      new object[] { null, "null" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}", null, JsonIgnoreCondition.Never },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}", null, JsonIgnoreCondition.WhenWritingNull },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, null, null!), "{\"StructProperty\":1}", null, JsonIgnoreCondition.WhenWritingDefault },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{}", null, JsonIgnoreCondition.WhenWritingDefault },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}", null, JsonIgnoreCondition.Never },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}", null, JsonIgnoreCondition.WhenWritingNull },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}", null, JsonIgnoreCondition.WhenWritingDefault },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}", null, JsonIgnoreCondition.Never },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}", null, JsonIgnoreCondition.WhenWritingNull },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}", null, JsonIgnoreCondition.WhenWritingDefault },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase, JsonIgnoreCondition.Never }
                                                                                   };

   [Fact]
   public void Should_deserialize_enum_if_null_and_default()
   {
      SerializeWithConverter<TestSmartEnum_Class_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_IntBased, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Class_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_StringBased, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_IntBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_StringBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int, ValidationError>>(default).Should().Be("0");
      SerializeWithConverter<TestSmartEnum_Struct_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string, ValidationError>>(default).Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_if_null_and_default()
   {
      SerializeWithConverter<IntBasedReferenceValueObject, ValueObjectJsonConverterFactory<IntBasedReferenceValueObject, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<StringBasedReferenceValueObject, ValueObjectJsonConverterFactory<StringBasedReferenceValueObject, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<IntBasedStructValueObject?, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<StringBasedStructValueObject?, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string, ValidationError>>(null).Should().Be("null");
      SerializeWithConverter<IntBasedStructValueObject, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int, ValidationError>>(default).Should().Be("0");
      SerializeWithConverter<StringBasedStructValueObject, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string, ValidationError>>(default).Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      SerializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>(null).Should().Be("null");
      SerializeWithConverter<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.ValueObjectJsonConverterFactory>(null).Should().Be("null");
      SerializeWithConverter<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.ValueObjectJsonConverterFactory>(default).Should().Be("{\"Property1\":null,\"Property2\":null}");
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_serialize_string_based_enum(TestEnum enumValue, string expectedJson)
   {
      var json = SerializeWithConverter<TestEnum, ValueObjectJsonConverterFactory<TestEnum, string, ValidationError>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_serialize_int_based_enum(IntegerEnum enumValue, string expectedJson)
   {
      var json = SerializeWithConverter<IntegerEnum, ValueObjectJsonConverterFactory<IntegerEnum, int, ValidationError>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_serialize_class_containing_string_based_enum(ClassWithStringBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithStringBasedEnum, ValueObjectJsonConverterFactory<TestEnum, string, ValidationError>>(classWithEnum, null, ignoreNullValues);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_serialize_class_containing_int_based_enum(ClassWithIntBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithIntBasedEnum, ValueObjectJsonConverterFactory<IntegerEnum, int, ValidationError>>(classWithEnum, null, ignoreNullValues);

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
      var json = SerializeWithConverter<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectJsonConverterFactory>(valueObject, namingPolicy, jsonIgnoreCondition);

      json.Should().Be(expectedJson);
   }

   [Fact]
   public void Should_deserialize_using_custom_factory_specified_by_ValueObjectFactoryAttribute()
   {
      var json = Serialize<BoundaryWithFactories, string>(BoundaryWithFactories.Create(1, 2));

      json.Should().Be("\"1:2\"");
   }

   [Fact]
   public void Should_serialize_enum_with_ValueObjectValidationErrorAttribute()
   {
      var value = Serialize<TestEnumWithCustomError, string, TestEnumValidationError>(TestEnumWithCustomError.Item1);

      value.Should().BeEquivalentTo("\"item1\"");
   }

   [Fact]
   public void Should_serialize_simple_value_object_with_ValueObjectValidationErrorAttribute()
   {
      var value = Serialize<StringBasedReferenceValueObjectWithCustomError, string, StringBasedReferenceValueObjectValidationError>(StringBasedReferenceValueObjectWithCustomError.Create("value"));

      value.Should().BeEquivalentTo("\"value\"");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_ValueObjectValidationErrorAttribute()
   {
      var value = SerializeWithConverter<BoundaryWithCustomError, BoundaryWithCustomError.ValueObjectJsonConverterFactory>(BoundaryWithCustomError.Create(1, 2), JsonNamingPolicy.CamelCase);

      value.Should().BeEquivalentTo("{\"lower\":1,\"upper\":2}");
   }

   [Fact]
   public void Should_throw_if_non_string_based_enum_is_used_as_dictionary_key()
   {
      var dictionary = new Dictionary<TestSmartEnum_Class_IntBased, int>
                       {
                          { TestSmartEnum_Class_IntBased.Value1, 1 }
                       };

      var options = new JsonSerializerOptions { Converters = { new ValueObjectJsonConverterFactory() } };

      FluentActions.Invoking(() => JsonSerializer.Serialize(dictionary, options))
                   .Should().Throw<NotSupportedException>();
   }
}
