using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Xunit;
using Thinktecture.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests.TestClasses;

namespace Thinktecture.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests
{
   public class WriteJson
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
                                                                                    new object[] { new ClassWithStringBasedEnum(), "{\"Enum\":null}" },
                                                                                    new object[] { new ClassWithStringBasedEnum(), "{\"Enum\":null}" },
                                                                                    new object[] { new ClassWithStringBasedEnum(StringBasedEnum.ValueA), "{\"Enum\":\"A\"}" },
                                                                                    new object[] { new ClassWithStringBasedEnum(StringBasedEnum.ValueB), "{\"Enum\":\"B\"}" }
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
                                                                                 new object[] { new ClassWithIntBasedEnum(), "{\"Enum\":null}" },
                                                                                 new object[] { new ClassWithIntBasedEnum(), "{\"Enum\":null}" },
                                                                                 new object[] { new ClassWithIntBasedEnum(IntBasedEnum.Value1), "{\"Enum\":1}" },
                                                                                 new object[] { new ClassWithIntBasedEnum(IntBasedEnum.Value2), "{\"Enum\":2}" }
                                                                              };

      public static IEnumerable<object[]> DataForValueTypeWithMultipleProperties => new[]
                                                                                    {
                                                                                       new object[] { null, "null" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}", null, true },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase }
                                                                                    };

      [Theory]
      [MemberData(nameof(DataForStringBasedEnumTest))]
      public void Should_serialize_string_based_enum(StringBasedEnum enumValue, string expectedJson)
      {
         var json = Serialize<StringBasedEnum, StringBasedEnum_ValueTypeJsonConverterFactory>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_serialize_int_based_enum(IntBasedEnum enumValue, string expectedJson)
      {
         var json = Serialize<IntBasedEnum, IntBasedEnum_ValueTypeJsonConverterFactory>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
      public void Should_serialize_class_containing_string_based_enum(ClassWithStringBasedEnum classWithEnum, string expectedJson)
      {
         var json = Serialize<ClassWithStringBasedEnum, StringBasedEnum_ValueTypeJsonConverterFactory>(classWithEnum);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
      public void Should_serialize_class_containing_int_based_enum(ClassWithIntBasedEnum classWithEnum, string expectedJson)
      {
         var json = Serialize<ClassWithIntBasedEnum, IntBasedEnum_ValueTypeJsonConverterFactory>(classWithEnum);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForValueTypeWithMultipleProperties))]
      public void Should_serialize_value_type_with_3_properties(
         ValueTypeWithMultipleProperties valueType,
         string expectedJson,
         JsonNamingPolicy namingPolicy = null,
         bool ignoreNullValues = false)
      {
         var json = Serialize<ValueTypeWithMultipleProperties, ValueTypeWithMultipleProperties_ValueTypeJsonConverterFactory>(valueType, namingPolicy, ignoreNullValues);

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
