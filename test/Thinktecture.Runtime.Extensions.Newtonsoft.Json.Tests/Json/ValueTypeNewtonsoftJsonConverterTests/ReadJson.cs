using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Thinktecture.Json.ValueTypeNewtonsoftJsonConverterTests
{
   public class ReadJson
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
      public void Should_deserialize_string_based_enum(StringBasedEnum expectedValue, string json)
      {
         var value = Deserialize<StringBasedEnum, string, StringBasedEnum_ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_deserialize_int_based_enum(IntBasedEnum expectedValue, string json)
      {
         var value = Deserialize<IntBasedEnum, int, IntBasedEnum_ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      private static T Deserialize<T, TKey, TConverter>(string json)
         where T : IEnum<TKey>
         where TConverter : ValueTypeNewtonsoftJsonConverter<T, TKey>, new()
      {
         var sut = new TConverter();

         using (var reader = new StringReader(json))
         using (var jsonWriter = new JsonTextReader(reader))
         {
            var serializer = JsonSerializer.CreateDefault();
            return (T)sut.ReadJson(jsonWriter, typeof(T), default, false, serializer);
         }
      }
   }
}
