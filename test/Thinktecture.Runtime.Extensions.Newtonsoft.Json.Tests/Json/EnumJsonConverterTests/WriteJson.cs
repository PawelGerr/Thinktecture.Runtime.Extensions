using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Thinktecture.Json.EnumJsonConverterTests
{
   public class WriteJson
   {
      public static IEnumerable<object[]> DataForStringBasedEnumTest => new[]
                                                                        {
                                                                           new object[] { null, "null" },
                                                                           new object[] { StringBasedEnum.ValueA, "\"A\"" },
                                                                           new object[] { StringBasedEnum.ValueB, "\"B\"" }
                                                                        };

      public static IEnumerable<object[]> DataForIntBasedEnumTest => new[]
                                                                     {
                                                                        new object[] { null, "null" },
                                                                        new object[] { IntBasedEnum.Value1, "1" },
                                                                        new object[] { IntBasedEnum.Value2, "2" }
                                                                     };

      [Theory]
      [MemberData(nameof(DataForStringBasedEnumTest))]
      public void Should_serialize_string_based_enum(StringBasedEnum enumValue, string expectedJson)
      {
         var json = Serialize<StringBasedEnum, string, StringBasedEnum_EnumNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_serialize_int_based_enum(IntBasedEnum enumValue, string expectedJson)
      {
         var json = Serialize<IntBasedEnum, int, IntBasedEnum_EnumNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      private static string Serialize<T, TKey, TConverter>(T value)
         where T : IEnum<TKey>
         where TConverter : EnumJsonConverter<T, TKey>, new()
      {
         var sut = new TConverter();

         using (var writer = new StringWriter())
         {
            using (var jsonWriter = new JsonTextWriter(writer))
            {
               sut.WriteJson(jsonWriter, value, JsonSerializer.CreateDefault());
            }

            return writer.GetStringBuilder().ToString();
         }
      }
   }
}
