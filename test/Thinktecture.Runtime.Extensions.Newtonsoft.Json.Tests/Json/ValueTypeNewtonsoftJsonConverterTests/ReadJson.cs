using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.Json.ValueTypeNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.Json.ValueTypeNewtonsoftJsonConverterTests
{
   public class ReadJson : JsonTestsBase
   {
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
      [MemberData(nameof(DataForStringBasedEnumTest))]
      public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
      {
         var value = Deserialize<TestEnum, TestEnum.ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForExtensibleTestEnumTest))]
      public void Should_deserialize_ExtensibleTestEnum(ExtensibleTestEnum expectedValue, string json)
      {
         var value = Deserialize<ExtensibleTestEnum, ExtensibleTestEnum.ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForExtendedTestEnumTest))]
      public void Should_deserialize_ExtendedTestEnum(ExtendedTestEnum expectedValue, string json)
      {
         var value = Deserialize<ExtendedTestEnum, ExtendedTestEnum.ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForDifferentAssemblyExtendedTestEnumTest))]
      public void Should_deserialize_DifferentAssemblyExtendedTestEnum(DifferentAssemblyExtendedTestEnum expectedValue, string json)
      {
         var value = Deserialize<DifferentAssemblyExtendedTestEnum, DifferentAssemblyExtendedTestEnum.ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForIntBasedEnumTest))]
      public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
      {
         var value = Deserialize<IntegerEnum, IntegerEnum.ValueTypeNewtonsoftJsonConverter>(json);

         value.Should().Be(expectedValue);
      }

      [Theory]
      [MemberData(nameof(DataForValueTypeWithMultipleProperties))]
      public void Should_deserialize_value_type_with_multiple_properties(
         ValueTypeWithMultipleProperties expectedValueType,
         string json,
         NamingStrategy namingStrategy = null)
      {
         var value = Deserialize<ValueTypeWithMultipleProperties, ValueTypeWithMultipleProperties.ValueTypeNewtonsoftJsonConverter>(json, namingStrategy);

         value.Should().BeEquivalentTo(expectedValueType);
      }

      private static T Deserialize<T, TConverter>(
         string json,
         NamingStrategy namingStrategy = null)
         where TConverter : JsonConverter<T>, new()
      {
         using var reader = new StringReader(json);
         using var jsonReader = new JsonTextReader(reader);

         var settings = new JsonSerializerSettings { Converters = { new TConverter() } };

         if (namingStrategy is not null)
            settings.ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy };

         var serializer = JsonSerializer.CreateDefault(settings);

         return serializer.Deserialize<T>(jsonReader);
      }
   }
}
