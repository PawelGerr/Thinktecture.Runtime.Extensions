using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Json.ValueTypeNewtonsoftJsonConverterTests.TestClasses;
using Xunit;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Thinktecture.Json.ValueTypeNewtonsoftJsonConverterTests
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
         var json = Serialize<StringBasedEnum, StringBasedEnum_ValueTypeNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_serialize_int_based_enum(IntBasedEnum enumValue, string expectedJson)
      {
         var json = Serialize<IntBasedEnum, IntBasedEnum_ValueTypeNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      public static IEnumerable<object[]> DataForValueTypeWithMultipleProperties => new[]
                                                                                    {
                                                                                       new object[] { null, "null" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0.0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0.0}", null, NullValueHandling.Ignore },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0.0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1.0,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1.0,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", new CamelCaseNamingStrategy() }
                                                                                    };

      [Theory]
      [MemberData(nameof(DataForValueTypeWithMultipleProperties))]
      public void Should_serialize_value_type_with_multiple_properties(
         ValueTypeWithMultipleProperties value,
         string expectedJson,
         NamingStrategy namingStrategy = null,
         NullValueHandling nullValueHandling = NullValueHandling.Include)
      {
         var json = Serialize<ValueTypeWithMultipleProperties, ValueTypeWithMultipleProperties_ValueTypeNewtonsoftJsonConverter>(value, namingStrategy, nullValueHandling);

         json.Should().Be(expectedJson);
      }

      private static string Serialize<T, TConverter>(
         T value,
         NamingStrategy namingStrategy = null,
         NullValueHandling nullValueHandling = NullValueHandling.Include)
         where TConverter : JsonConverter<T>, new()
      {
         using var writer = new StringWriter();
         using var jsonWriter = new JsonTextWriter(writer);

         var settings = new JsonSerializerSettings
                        {
                           Converters = { new TConverter() },
                           NullValueHandling = nullValueHandling
                        };

         if (namingStrategy is not null)
            settings.ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy };

         var serializer = JsonSerializer.CreateDefault(settings);

         serializer.Serialize(jsonWriter, value);

         return writer.GetStringBuilder().ToString();
      }
   }
}
