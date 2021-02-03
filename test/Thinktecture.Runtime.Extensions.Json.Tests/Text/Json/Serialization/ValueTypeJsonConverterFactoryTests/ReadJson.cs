using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests.TestClasses;
using Xunit;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests
{
   public class ReadJson
   {
      public static IEnumerable<object[]> DataForStringBasedEnumTest => new[]
                                                                        {
                                                                           new object[] { null, "null" },
                                                                           new object[] { StringBasedEnum.ValueA, "\"A\"" },
                                                                           new object[] { StringBasedEnum.ValueB, "\"B\"" }
                                                                        };

      public static IEnumerable<object[]> DataForClassWithStringBasedEnumTest => new[]
                                                                                 {
                                                                                    new object[] { null, "null" },
                                                                                    new object[] { new ClassWithStringBasedEnum(), "{ }" },
                                                                                    new object[] { new ClassWithStringBasedEnum(), "{ \"Enum\": null }" },
                                                                                    new object[] { new ClassWithStringBasedEnum(StringBasedEnum.ValueA), "{ \"Enum\": \"A\" }" },
                                                                                    new object[] { new ClassWithStringBasedEnum(StringBasedEnum.ValueB), "{ \"Enum\": \"B\" }" }
                                                                                 };

      public static IEnumerable<object[]> DataForIntBasedEnumTest => new[]
                                                                     {
                                                                        new object[] { null, "null" },
                                                                        new object[] { IntBasedEnum.Value1, "1" },
                                                                        new object[] { IntBasedEnum.Value2, "2" }
                                                                     };

      public static IEnumerable<object[]> DataForClassWithIntBasedEnumTest => new[]
                                                                              {
                                                                                 new object[] { null, "null" },
                                                                                 new object[] { new ClassWithIntBasedEnum(), "{ }" },
                                                                                 new object[] { new ClassWithIntBasedEnum(), "{ \"Enum\": null }" },
                                                                                 new object[] { new ClassWithIntBasedEnum(IntBasedEnum.Value1), "{ \"Enum\": 1 }" },
                                                                                 new object[] { new ClassWithIntBasedEnum(IntBasedEnum.Value2), "{ \"Enum\": 2 }" }
                                                                              };

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
      public void Should_deserialize_string_based_enum(StringBasedEnum expectedValue, string json)
      {
         var value = Deserialize<StringBasedEnum, StringBasedEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_deserialize_int_based_enum(IntBasedEnum expectedValue, string json)
      {
         var value = Deserialize<IntBasedEnum, IntBasedEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
      public void Should_deserialize_class_containing_string_based_enum(ClassWithStringBasedEnum expectedValue, string json)
      {
         var value = Deserialize<ClassWithStringBasedEnum, StringBasedEnum_ValueTypeJsonConverterFactory>(json);

         value.Should().BeEquivalentTo(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
      public void Should_deserialize_class_containing_int_based_enum(ClassWithIntBasedEnum expectedValue, string json)
      {
         var value = Deserialize<ClassWithIntBasedEnum, IntBasedEnum_ValueTypeJsonConverterFactory>(json);

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
         bool propertyNameCaseInsensitive = false)
         where TConverterFactory : JsonConverterFactory, new()
      {
         var factory = new TConverterFactory();
         var options = new JsonSerializerOptions
                       {
                          Converters = { factory },
                          PropertyNamingPolicy = namingPolicy,
                          PropertyNameCaseInsensitive = propertyNameCaseInsensitive
                       };

         return JsonSerializer.Deserialize<T>(json, options);
      }
   }
}
