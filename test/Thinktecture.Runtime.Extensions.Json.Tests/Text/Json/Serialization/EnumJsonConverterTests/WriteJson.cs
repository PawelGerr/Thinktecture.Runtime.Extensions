using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses;
using Xunit;

namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests
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

      private static string Serialize<T, TConverterFactory>(T value)
         where TConverterFactory : JsonConverterFactory, new()
      {
         var factory = new TConverterFactory();
         var options = new JsonSerializerOptions { Converters = { factory } };

         return JsonSerializer.Serialize(value, options);
      }
   }
}
