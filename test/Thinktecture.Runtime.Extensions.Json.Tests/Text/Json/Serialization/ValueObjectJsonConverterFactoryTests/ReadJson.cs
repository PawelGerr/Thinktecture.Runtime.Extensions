using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;
using Xunit;

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

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
   {
      var value = Deserialize<TestEnum, TestEnum.ValueObjectJsonConverterFactory>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForExtensibleEnumTest))]
   public void Should_deserialize_ExtensibleTestEnum(ExtensibleTestEnum expectedValue, string json)
   {
      var value = Deserialize<ExtensibleTestEnum, ExtensibleTestEnum.ValueObjectJsonConverterFactory>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForExtendedEnumTest))]
   public void Should_deserialize_ExtendedTestEnum(ExtendedTestEnum expectedValue, string json)
   {
      var value = Deserialize<ExtendedTestEnum, ExtendedTestEnum.ValueObjectJsonConverterFactory>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForDifferentAssemblyExtendedTestEnumTest))]
   public void Should_deserialize_DifferentAssemblyExtendedTestEnum(DifferentAssemblyExtendedTestEnum expectedValue, string json)
   {
      var value = Deserialize<DifferentAssemblyExtendedTestEnum, DifferentAssemblyExtendedTestEnum.ValueObjectJsonConverterFactory>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
   {
      var value = Deserialize<IntegerEnum, IntegerEnum.ValueObjectJsonConverterFactory>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_deserialize_class_containing_string_based_enum(ClassWithStringBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = Deserialize<ClassWithStringBasedEnum, TestEnum.ValueObjectJsonConverterFactory>(json, ignoreNullValues: ignoreNullValues);

      value.Should().BeEquivalentTo(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_deserialize_class_containing_int_based_enum(ClassWithIntBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = Deserialize<ClassWithIntBasedEnum, IntegerEnum.ValueObjectJsonConverterFactory>(json, ignoreNullValues: ignoreNullValues);

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
      Action action = () => Deserialize<ValidTestEnum, ValidTestEnum.ValueObjectJsonConverterFactory>("\"invalid\"");

      action.Should().Throw<JsonException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
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