using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests.TestClasses;
using Xunit;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests
{
   public class ReadJson : JsonTestsBase
   {
      public static IEnumerable<object[]> DataForValueTypeWithMultipleProperties => new[]
                                                                                    {
                                                                                       new object[] { null, "null" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}", null, true }
                                                                                    };

      [Theory]
      [MemberData(nameof(DataForStringBasedEnumTest))]
      public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
      {
         var value = Deserialize<TestEnum, TestEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForExtensibleEnumTest))]
      public void Should_deserialize_ExtensibleTestEnum(ExtensibleTestEnum expectedValue, string json)
      {
         var value = Deserialize<ExtensibleTestEnum, ExtensibleTestEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForExtendedEnumTest))]
      public void Should_deserialize_ExtendedTestEnum(ExtendedTestEnum expectedValue, string json)
      {
         var value = Deserialize<ExtendedTestEnum, ExtendedTestEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForDifferentAssemblyExtendedTestEnumTest))]
      public void Should_deserialize_DifferentAssemblyExtendedTestEnum(DifferentAssemblyExtendedTestEnum expectedValue, string json)
      {
         var value = Deserialize<DifferentAssemblyExtendedTestEnum, DifferentAssemblyExtendedTestEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
      {
         var value = Deserialize<IntegerEnum, IntegerEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
      public void Should_deserialize_class_containing_string_based_enum(ClassWithStringBasedEnum expectedValue, string json, bool ignoreNullValues = false)
      {
         var value = Deserialize<ClassWithStringBasedEnum, TestEnum_ValueTypeJsonConverterFactory>(json, ignoreNullValues: ignoreNullValues);

         value.Should().BeEquivalentTo(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
      public void Should_deserialize_class_containing_int_based_enum(ClassWithIntBasedEnum expectedValue, string json, bool ignoreNullValues = false)
      {
         var value = Deserialize<ClassWithIntBasedEnum, IntegerEnum_ValueTypeJsonConverterFactory>(json, ignoreNullValues: ignoreNullValues);

         value.Should().BeEquivalentTo(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForValueTypeWithMultipleProperties))]
      public void Should_deserialize_value_type_with_multiple_properties(
         ValueTypeWithMultipleProperties expectedValueType,
         string json,
         JsonNamingPolicy namingPolicy = null,
         bool propertyNameCaseInsensitive = false)
      {
         var value = Deserialize<ValueTypeWithMultipleProperties, ValueTypeWithMultipleProperties_ValueTypeJsonConverterFactory>(json, namingPolicy, propertyNameCaseInsensitive);

         value.Should().BeEquivalentTo(expectedValueType);
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
                          IgnoreNullValues = ignoreNullValues
                       };

         return JsonSerializer.Deserialize<T>(json, options);
      }
   }
}
