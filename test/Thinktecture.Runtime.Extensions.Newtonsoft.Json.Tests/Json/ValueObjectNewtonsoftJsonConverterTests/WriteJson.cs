using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests
{
   public class WriteJson : JsonTestsBase
   {
      public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
                                                                                      {
                                                                                         new object[] { null, "null" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0.0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0.0}", null, NullValueHandling.Ignore },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0.0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1.0,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                         new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1.0,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", new CamelCaseNamingStrategy() }
                                                                                      };

      [Theory]
      [MemberData(nameof(DataForStringBasedEnumTest))]
      public void Should_serialize_string_based_enum(TestEnum enumValue, string expectedJson)
      {
         var json = Serialize<TestEnum, TestEnum.ValueObjectNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForExtensibleTestEnumTest))]
      public void Should_serialize_ExtensibleTestEnum(ExtensibleTestEnum enumValue, string expectedJson)
      {
         var json = Serialize<ExtensibleTestEnum, ExtensibleTestEnum.ValueObjectNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForExtendedTestEnumTest))]
      public void Should_serialize_ExtendedTestEnum(ExtendedTestEnum enumValue, string expectedJson)
      {
         var json = Serialize<ExtendedTestEnum, ExtendedTestEnum.ValueObjectNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForDifferentAssemblyExtendedTestEnumTest))]
      public void Should_serialize_DifferentAssemblyExtendedTestEnum(DifferentAssemblyExtendedTestEnum enumValue, string expectedJson)
      {
         var json = Serialize<DifferentAssemblyExtendedTestEnum, DifferentAssemblyExtendedTestEnum.ValueObjectNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_serialize_int_based_enum(IntegerEnum enumValue, string expectedJson)
      {
         var json = Serialize<IntegerEnum, IntegerEnum.ValueObjectNewtonsoftJsonConverter>(enumValue);

         json.Should().Be(expectedJson);
      }

      [Theory]
      [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
      public void Should_serialize_value_type_with_multiple_properties(
         ValueObjectWithMultipleProperties value,
         string expectedJson,
         NamingStrategy namingStrategy = null,
         NullValueHandling nullValueHandling = NullValueHandling.Include)
      {
         var json = Serialize<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectNewtonsoftJsonConverter>(value, namingStrategy, nullValueHandling);

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
