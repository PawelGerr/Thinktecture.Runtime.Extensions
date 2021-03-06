using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;
using Xunit;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests
{
   public class WriteJson : JsonTestsBase
   {
      public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
                                                                                      {
                                                                                         new object[] { null, "null" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}", null, true },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase }
                                                                                      };

      [Theory]
      [MemberData(nameof(DataForStringBasedEnumTest))]
      public void Should_serialize_string_based_enum(TestEnum enumValue, string expectedJson)
      {
         var json = Serialize<TestEnum, TestEnum.ValueObjectJsonConverterFactory>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForExtensibleEnumTest))]
      public void Should_serialize_ExtensibleTestEnum(ExtensibleTestEnum enumValue, string expectedJson)
      {
         var json = Serialize<ExtensibleTestEnum, ExtensibleTestEnum.ValueObjectJsonConverterFactory>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForExtendedEnumTest))]
      public void Should_serialize_ExtendedTestEnum(ExtendedTestEnum enumValue, string expectedJson)
      {
         var json = Serialize<ExtendedTestEnum, ExtendedTestEnum.ValueObjectJsonConverterFactory>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForDifferentAssemblyExtendedTestEnumTest))]
      public void Should_serialize_DifferentAssemblyExtendedTestEnum(DifferentAssemblyExtendedTestEnum enumValue, string expectedJson)
      {
         var json = Serialize<DifferentAssemblyExtendedTestEnum, DifferentAssemblyExtendedTestEnum.ValueObjectJsonConverterFactory>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_serialize_int_based_enum(IntegerEnum enumValue, string expectedJson)
      {
         var json = Serialize<IntegerEnum, IntegerEnum.ValueObjectJsonConverterFactory>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
      public void Should_serialize_class_containing_string_based_enum(ClassWithStringBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
      {
         var json = Serialize<ClassWithStringBasedEnum, TestEnum.ValueObjectJsonConverterFactory>(classWithEnum, null, ignoreNullValues);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
      public void Should_serialize_class_containing_int_based_enum(ClassWithIntBasedEnum classWithEnum, string expectedJson, bool ignoreNullValues = false)
      {
         var json = Serialize<ClassWithIntBasedEnum, IntegerEnum.ValueObjectJsonConverterFactory>(classWithEnum, null, ignoreNullValues);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
      public void Should_serialize_value_type_with_3_properties(
         ValueObjectWithMultipleProperties valueObject,
         string expectedJson,
         JsonNamingPolicy namingPolicy = null,
         bool ignoreNullValues = false)
      {
         var json = Serialize<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectJsonConverterFactory>(valueObject, namingPolicy, ignoreNullValues);

         json.Should().Be(expectedJson);
      }

      private static string Serialize<T, TConverterFactory>(
         T value,
         JsonNamingPolicy namingPolicy = null,
         bool ignoreNullValues = false)
         where TConverterFactory : JsonConverterFactory, new()
      {
         var factory = new TConverterFactory();
         var options = new JsonSerializerOptions
                       {
                          Converters = { factory },
                          PropertyNamingPolicy = namingPolicy,
                          IgnoreNullValues = ignoreNullValues
                       };

         return JsonSerializer.Serialize(value, options);
      }
   }
}
