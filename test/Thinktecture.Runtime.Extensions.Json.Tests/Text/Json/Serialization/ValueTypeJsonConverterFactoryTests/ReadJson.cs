using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Xunit;
using Thinktecture.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests.TestClasses;

namespace Thinktecture.Text.Json.Serialization.ValueTypeJsonConverterFactoryTests
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

      private static T Deserialize<T, TConverterFactory>(string json)
         where TConverterFactory : JsonConverterFactory, new()
      {
         var sut = new TConverterFactory();
         var options = new JsonSerializerOptions { Converters = { sut } };

         return JsonSerializer.Deserialize<T>(json, options);
      }
   }
}
