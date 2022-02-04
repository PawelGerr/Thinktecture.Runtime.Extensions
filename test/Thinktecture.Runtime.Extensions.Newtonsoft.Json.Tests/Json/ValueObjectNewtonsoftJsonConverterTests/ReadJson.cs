using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests;

public class ReadJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
                                                                                   {
                                                                                      new object[] { null, "null" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", new CamelCaseNamingStrategy() },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}" }
                                                                                   };

   [Fact]
   public void Should_deserialize_enum_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      Deserialize<TestSmartEnum_Class_IntBased, TestSmartEnum_Class_IntBased.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Class_StringBased, TestSmartEnum_Class_StringBased.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Struct_IntBased?, TestSmartEnum_Struct_IntBased.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Struct_StringBased?, TestSmartEnum_Struct_StringBased.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Struct_IntBased, TestSmartEnum_Struct_IntBased.ValueObjectNewtonsoftJsonConverter>("0").Should().Be(default(TestSmartEnum_Struct_IntBased)); // default(int) is 0
      Deserialize<TestSmartEnum_Struct_StringBased, TestSmartEnum_Struct_StringBased.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(default(TestSmartEnum_Struct_StringBased));

      FluentActions.Invoking(() => Deserialize<TestSmartEnum_Struct_IntBased, TestSmartEnum_Struct_IntBased.ValueObjectNewtonsoftJsonConverter>("null")).Should()
                   .Throw<JsonException>().WithMessage("Cannot convert 'Null' to a struct of type 'Int32'.");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      Deserialize<IntBasedReferenceValueObject, IntBasedReferenceValueObject.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<StringBasedReferenceValueObject, StringBasedReferenceValueObject.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<IntBasedStructValueObject?, IntBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<StringBasedStructValueObject?, StringBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<IntBasedStructValueObject, IntBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>("0").Should().Be(default(IntBasedStructValueObject)); // default(int) is 0
      Deserialize<StringBasedStructValueObject, StringBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(default(StringBasedStructValueObject));

      FluentActions.Invoking(() => Deserialize<IntBasedStructValueObject, IntBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>("null")).Should()
                   .Throw<JsonException>().WithMessage("Cannot convert 'Null' to a struct of type 'Int32'.");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      Deserialize<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(default(TestValueObject_Complex_Struct));
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
   {
      var value = Deserialize<TestEnum, TestEnum.ValueObjectNewtonsoftJsonConverter>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForExtensibleTestEnumTest))]
   public void Should_deserialize_ExtensibleTestEnum(ExtensibleTestEnum expectedValue, string json)
   {
      var value = Deserialize<ExtensibleTestEnum, ExtensibleTestEnum.ValueObjectNewtonsoftJsonConverter>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForExtendedTestEnumTest))]
   public void Should_deserialize_ExtendedTestEnum(ExtendedTestEnum expectedValue, string json)
   {
      var value = Deserialize<ExtendedTestEnum, ExtendedTestEnum.ValueObjectNewtonsoftJsonConverter>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForDifferentAssemblyExtendedTestEnumTest))]
   public void Should_deserialize_DifferentAssemblyExtendedTestEnum(DifferentAssemblyExtendedTestEnum expectedValue, string json)
   {
      var value = Deserialize<DifferentAssemblyExtendedTestEnum, DifferentAssemblyExtendedTestEnum.ValueObjectNewtonsoftJsonConverter>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
   {
      var value = Deserialize<IntegerEnum, IntegerEnum.ValueObjectNewtonsoftJsonConverter>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
   public void Should_deserialize_value_type_with_multiple_properties(
      ValueObjectWithMultipleProperties expectedValueObject,
      string json,
      NamingStrategy namingStrategy = null)
   {
      var value = Deserialize<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectNewtonsoftJsonConverter>(json, namingStrategy);

      value.Should().BeEquivalentTo(expectedValueObject);
   }

   [Fact]
   public void Should_throw_JsonException_if_enum_parsing_throws_UnknownEnumIdentifierException()
   {
      FluentActions.Invoking(() => Deserialize<ValidTestEnum, ValidTestEnum.ValueObjectNewtonsoftJsonConverter>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }

   [Fact]
   public void Should_throw_JsonException_if_complex_value_object_is_not_a_json_object()
   {
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"String\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("42"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"Integer\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("[]"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"StartArray\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
   }

   private static T Deserialize<T, TConverter>(
      string json,
      NamingStrategy namingStrategy = null)
      where TConverter : JsonConverter, new()
   {
      using var reader = new StringReader(json);
      using var jsonReader = new JsonTextReader(reader);

      var settings = new JsonSerializerSettings { Converters = { new TConverter() } };

      if (namingStrategy is not null)
         settings.ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy };

      var serializer = JsonSerializer.CreateDefault(settings);

      return serializer.Deserialize<T>(jsonReader);
   }
}
