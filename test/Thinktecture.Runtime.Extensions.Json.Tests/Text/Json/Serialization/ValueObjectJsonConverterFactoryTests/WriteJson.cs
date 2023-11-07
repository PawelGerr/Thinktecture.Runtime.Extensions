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
      SerializeWithConverter<TestSmartEnum_Class_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_IntBased, int>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Class_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_StringBased, string>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_IntBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_StringBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string>>(null).Should().Be("null");
      SerializeWithConverter<TestSmartEnum_Struct_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int>>(default).Should().Be("0");
      SerializeWithConverter<TestSmartEnum_Struct_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string>>(default).Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_if_null_and_default()
   {
      SerializeWithConverter<IntBasedReferenceValueObject, ValueObjectJsonConverterFactory<IntBasedReferenceValueObject, int>>(null).Should().Be("null");
      SerializeWithConverter<StringBasedReferenceValueObject, ValueObjectJsonConverterFactory<StringBasedReferenceValueObject, string>>(null).Should().Be("null");
      SerializeWithConverter<IntBasedStructValueObject?, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int>>(null).Should().Be("null");
      SerializeWithConverter<StringBasedStructValueObject?, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string>>(null).Should().Be("null");
      SerializeWithConverter<IntBasedStructValueObject, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int>>(default).Should().Be("0");
      SerializeWithConverter<StringBasedStructValueObject, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string>>(default).Should().Be("null");
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
      var json = SerializeWithConverter<TestEnum, ValueObjectJsonConverterFactory<TestEnum, string>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_serialize_int_based_enum(IntegerEnum enumValue, string expectedJson)
   {
      var json = SerializeWithConverter<IntegerEnum, ValueObjectJsonConverterFactory<IntegerEnum, int>>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_serialize_class_containing_string_based_enum(ClassWithStringBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithStringBasedEnum, ValueObjectJsonConverterFactory<TestEnum, string>>(classWithEnum, null, ignoreNullValues);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_serialize_class_containing_int_based_enum(ClassWithIntBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
   {
      var json = SerializeWithConverter<ClassWithIntBasedEnum, ValueObjectJsonConverterFactory<IntegerEnum, int>>(classWithEnum, null, ignoreNullValues);

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

   private static string Serialize<T, TKey>(
      T value,
      JsonNamingPolicy namingStrategy = null,
      bool ignoreNullValues = false)
      where T : IValueObjectFactory<T, TKey>, IValueObjectConverter<TKey>
   {
      return SerializeWithConverter<T, ValueObjectJsonConverterFactory>(value, namingStrategy, ignoreNullValues);
   }

   private static string SerializeWithConverter<T, TConverterFactory>(
      T value,
      JsonNamingPolicy namingPolicy = null,
      bool ignoreNullValues = false)
      where TConverterFactory : JsonConverterFactory, new()
   {
      return SerializeWithConverter<T, TConverterFactory>(value, namingPolicy, ignoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never);
   }

   private static string SerializeWithConverter<T, TConverterFactory>(
      T value,
      JsonNamingPolicy namingPolicy,
      JsonIgnoreCondition jsonIgnoreCondition)
      where TConverterFactory : JsonConverterFactory, new()
   {
      var factory = new TConverterFactory();
      var options = new JsonSerializerOptions
                    {
                       Converters = { factory },
                       PropertyNamingPolicy = namingPolicy,
                       DefaultIgnoreCondition = jsonIgnoreCondition
                    };

      return JsonSerializer.Serialize(value, options);
   }
}
