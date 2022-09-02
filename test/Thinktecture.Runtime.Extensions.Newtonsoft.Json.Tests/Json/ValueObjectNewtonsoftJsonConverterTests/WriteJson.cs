using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests;

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

   [Fact]
   public void Should_deserialize_enum_if_null_and_default()
   {
      Serialize<TestSmartEnum_Class_IntBased, TestSmartEnum_Class_IntBased.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<TestSmartEnum_Class_StringBased, TestSmartEnum_Class_StringBased.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<TestSmartEnum_Struct_IntBased?, TestSmartEnum_Struct_IntBased.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<TestSmartEnum_Struct_StringBased?, TestSmartEnum_Struct_StringBased.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<TestSmartEnum_Struct_IntBased, TestSmartEnum_Struct_IntBased.ValueObjectNewtonsoftJsonConverter>(default).Should().Be("0");
      Serialize<TestSmartEnum_Struct_StringBased, TestSmartEnum_Struct_StringBased.ValueObjectNewtonsoftJsonConverter>(default).Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_if_null_and_default()
   {
      Serialize<IntBasedReferenceValueObject, IntBasedReferenceValueObject.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<StringBasedReferenceValueObject, StringBasedReferenceValueObject.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<IntBasedStructValueObject?, IntBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<StringBasedStructValueObject?, StringBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<IntBasedStructValueObject, IntBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>(default).Should().Be("0");
      Serialize<StringBasedStructValueObject, StringBasedStructValueObject.ValueObjectNewtonsoftJsonConverter>(default).Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      Serialize<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      Serialize<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>(default).Should().Be("{\"Property1\":null,\"Property2\":null}");
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_serialize_string_based_enum(TestEnum enumValue, string expectedJson)
   {
      var json = Serialize<TestEnum, TestEnum.ValueObjectNewtonsoftJsonConverter>(enumValue);

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
      where TConverter : JsonConverter, new()
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
