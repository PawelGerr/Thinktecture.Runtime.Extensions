using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Thinktecture.Text.Json.Serialization.EnumJsonConverterTests.TestClasses;
using Xunit;

namespace Thinktecture.Text.Json.Serialization.EnumJsonConverterTests
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
         var value = Deserialize<StringBasedEnum, string>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_deserialize_int_based_enum(IntBasedEnum expectedValue, string json)
      {
         var value = Deserialize<IntBasedEnum, int>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
      public void Should_deserialize_class_containing_string_based_enum(ClassWithStringBasedEnum expectedValue, string json)
      {
         var value = Deserialize<ClassWithStringBasedEnum, StringBasedEnum, string>(json);

         value.Should().BeEquivalentTo(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
      public void Should_deserialize_class_containing_int_based_enum(ClassWithIntBasedEnum expectedValue, string json)
      {
         var value = Deserialize<ClassWithIntBasedEnum, IntBasedEnum, int>(json);

         value.Should().BeEquivalentTo(expectedValue);
      }

      private static T Deserialize<T, TKey>(string json)
         where T : Enum<T, TKey>
      {
         return Deserialize<T, T, TKey>(json);
      }

      private static T Deserialize<T, TEnum, TKey>(string json)
         where TEnum : Enum<TEnum, TKey>
      {
         var sut = new EnumJsonConverter<TEnum, TKey>();
         var options = new JsonSerializerOptions { Converters = { sut } };

         return JsonSerializer.Deserialize<T>(json, options);
      }
   }
}
