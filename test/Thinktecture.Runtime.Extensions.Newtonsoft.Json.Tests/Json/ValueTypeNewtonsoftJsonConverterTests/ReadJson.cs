using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.Json.ValueTypeNewtonsoftJsonConverterTests.TestClasses;
using Xunit;

namespace Thinktecture.Runtime.Tests.Json.ValueTypeNewtonsoftJsonConverterTests
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
         var value = Deserialize<StringBasedEnum, StringBasedEnum_ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_deserialize_int_based_enum(IntBasedEnum expectedValue, string json)
      {
         var value = Deserialize<IntBasedEnum, IntBasedEnum_ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      public static IEnumerable<object[]> DataForValueTypeWithMultipleProperties => new[]
                                                                                    {
                                                                                       new object[] { null, "null" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", new CamelCaseNamingStrategy() },
                                                                                       new object[] { ValueTypeWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}" }
                                                                                    };

      [Theory]
      [MemberData(nameof(DataForValueTypeWithMultipleProperties))]
      public void Should_deserialize_value_type_with_multiple_properties(
         ValueTypeWithMultipleProperties expectedValueType,
         string json,
         NamingStrategy namingStrategy = null)
      {
         var value = Deserialize<ValueTypeWithMultipleProperties, ValueTypeWithMultipleProperties_ValueTypeNewtonsoftJsonConverter>(json, namingStrategy);

         value.Should().BeEquivalentTo(expectedValueType);
      }

      private static T Deserialize<T, TConverter>(
         string json,
         NamingStrategy namingStrategy = null)
         where TConverter : JsonConverter<T>, new()
      {
         using var reader = new StringReader(json);
         using var jsonReader = new JsonTextReader(reader);

         var settings = new JsonSerializerSettings { Converters = { new TConverter() }};

         if (namingStrategy is not null)
            settings.ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy };

         var serializer = JsonSerializer.CreateDefault(settings);

         return serializer.Deserialize<T>(jsonReader);
      }
   }
}
