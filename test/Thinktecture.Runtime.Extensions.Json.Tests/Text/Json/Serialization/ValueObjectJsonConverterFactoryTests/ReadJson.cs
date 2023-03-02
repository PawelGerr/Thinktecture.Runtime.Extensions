using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests;

public class ReadJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
                                                                                   {
                                                                                      new object[] { null, "null" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}", null, true }
                                                                                   };

   [Fact]
   public void Should_deserialize_enum_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      Deserialize<TestSmartEnum_Class_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_IntBased, int>>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Class_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_StringBased, string>>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Struct_IntBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int>>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Struct_StringBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string>>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Struct_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int>>("0").Should().Be(default(TestSmartEnum_Struct_IntBased)); // default(int) is 0
      Deserialize<TestSmartEnum_Struct_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string>>("null").Should().Be(default(TestSmartEnum_Struct_StringBased));

      FluentActions.Invoking(() => Deserialize<TestSmartEnum_Struct_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int>>("null")).Should()
                   .Throw<JsonException>()
                   .WithInnerException<InvalidOperationException>().WithMessage("Cannot get the value of a token type 'Null' as a number.");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      Deserialize<IntBasedReferenceValueObject, ValueObjectJsonConverterFactory<IntBasedReferenceValueObject, int>>("null").Should().Be(null);
      Deserialize<StringBasedReferenceValueObject, ValueObjectJsonConverterFactory<StringBasedReferenceValueObject, string>>("null").Should().Be(null);
      Deserialize<IntBasedStructValueObject?, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int>>("null").Should().Be(null);
      Deserialize<StringBasedStructValueObject?, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string>>("null").Should().Be(null);
      Deserialize<IntBasedStructValueObject, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int>>("0").Should().Be(default); // default(int) is 0
      Deserialize<StringBasedStructValueObject, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string>>("null").Should().Be(default);

      // NullInFactoryMethodsYieldsNull
      Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>>("null").Should().Be(null);
      FluentActions.Invoking(() => Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>>("\"\""))
                   .Should().Throw<JsonException>().WithMessage("Property cannot be empty.");
      FluentActions.Invoking(() => Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>>("\" \""))
                   .Should().Throw<JsonException>().WithMessage("Property cannot be empty.");

      // EmptyStringInFactoryMethodsYieldsNull
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>>("null").Should().Be(null);
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>>("\"\"").Should().Be(null);
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>>("\" \"").Should().Be(null);

      FluentActions.Invoking(() => Deserialize<IntBasedStructValueObject, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int>>("null")).Should()
                   .Throw<JsonException>()
                   .WithInnerException<InvalidOperationException>().WithMessage("Cannot get the value of a token type 'Null' as a number.");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("null").Should().Be(null);
      Deserialize<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.ValueObjectJsonConverterFactory>("null").Should().Be(null);
      Deserialize<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.ValueObjectJsonConverterFactory>("null").Should().Be(default(TestValueObject_Complex_Struct));
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
   {
      var value = Deserialize<TestEnum, ValueObjectJsonConverterFactory<TestEnum, string>>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
   {
      var value = Deserialize<IntegerEnum, ValueObjectJsonConverterFactory<IntegerEnum, int>>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_deserialize_class_containing_string_based_enum(ClassWithStringBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = Deserialize<ClassWithStringBasedEnum, ValueObjectJsonConverterFactory<TestEnum, string>>(json, ignoreNullValues: ignoreNullValues);

      value.Should().BeEquivalentTo(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_deserialize_class_containing_int_based_enum(ClassWithIntBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = Deserialize<ClassWithIntBasedEnum, ValueObjectJsonConverterFactory<IntegerEnum, int>>(json, ignoreNullValues: ignoreNullValues);

      value.Should().BeEquivalentTo(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
   public void Should_deserialize_value_type_with_multiple_properties(
      ValueObjectWithMultipleProperties expectedValueObject,
      string json,
      JsonNamingPolicy namingPolicy = null,
      bool propertyNameCaseInsensitive = false)
   {
      var value = Deserialize<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectJsonConverterFactory>(json, namingPolicy, propertyNameCaseInsensitive);

      value.Should().BeEquivalentTo(expectedValueObject);
   }

   [Fact]
   public void Should_throw_JsonException_if_enum_parsing_throws_UnknownEnumIdentifierException()
   {
      FluentActions.Invoking(() => Deserialize<ValidTestEnum, ValueObjectJsonConverterFactory<ValidTestEnum, string>>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }

   [Fact]
   public void Should_throw_JsonException_if_complex_value_object_is_not_a_json_object()
   {
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"String\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("42"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"Number\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("[]"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"StartArray\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
   }

   private static T Deserialize<T, TConverterFactory>(
      string json,
      JsonNamingPolicy namingPolicy = null,
      bool propertyNameCaseInsensitive = false,
      bool ignoreNullValues = false)
      where TConverterFactory : JsonConverterFactory, new()
   {
      var factory = new TConverterFactory();
      var options = new JsonSerializerOptions
                    {
                       Converters = { factory },
                       PropertyNamingPolicy = namingPolicy,
                       PropertyNameCaseInsensitive = propertyNameCaseInsensitive,
                       DefaultIgnoreCondition = ignoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never
                    };

      return JsonSerializer.Deserialize<T>(json, options);
   }
}
