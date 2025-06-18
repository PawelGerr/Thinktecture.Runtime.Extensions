using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Json;
using Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests;

public class WriteJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties =>
   [
      [null, "null"],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0.0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}"],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0.0}", null, NullValueHandling.Ignore],
      [ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0.0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}"],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1.0,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}"],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1.0,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", new CamelCaseNamingStrategy()]
   ];

   [Fact]
   public void Should_deserialize_enum_if_null_and_default()
   {
      Serialize<SmartEnum_IntBased, int>(null).Should().Be("null");
      Serialize<SmartEnum_StringBased, string>(null).Should().Be("null");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_if_null_and_default()
   {
      Serialize<IntBasedReferenceValueObject, int>(null).Should().Be("null");
      Serialize<StringBasedReferenceValueObject, string>(null).Should().Be("null");
      SerializeNullableStruct<IntBasedStructValueObject, int>(null).Should().Be("null");
      SerializeNullableStruct<StringBasedStructValueObject, string>(null).Should().Be("null");
      SerializeStruct<IntBasedStructValueObject, int>(default).Should().Be("0");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      SerializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      SerializeWithConverter<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>(null).Should().Be("null");
      SerializeWithConverter<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>(default).Should().Be("{\"Property1\":null,\"Property2\":null}");
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_serialize_string_based_enum(SmartEnum_StringBased enumValue, string expectedJson)
   {
      var json = Serialize<SmartEnum_StringBased, string>(enumValue);

      json.Should().Be(expectedJson);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_serialize_int_based_enum(SmartEnum_IntBased enumValue, string expectedJson)
   {
      var json = Serialize<SmartEnum_IntBased, int>(enumValue);

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
      var json = SerializeWithConverter<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectNewtonsoftJsonConverter>(value, namingStrategy, nullValueHandling);

      json.Should().Be(expectedJson);
   }

   [Fact]
   public void Should_serialize_using_custom_factory_specified_by_ObjectFactoryAttribute()
   {
      var json = Serialize<BoundaryWithFactories, string>(BoundaryWithFactories.Create(1, 2));

      json.Should().Be("\"1:2\"");
   }

   [Fact]
   public void Should_serialize_enum_with_ValidationErrorAttribute()
   {
      var value = Serialize<TestSmartEnum_CustomError, string, CustomValidationError>(TestSmartEnum_CustomError.Item1);

      value.Should().BeEquivalentTo("\"item1\"");
   }

   [Fact]
   public void Should_serialize_simple_value_object_with_ValidationErrorAttribute()
   {
      var value = Serialize<StringBasedReferenceValueObjectWithCustomError, string, StringBasedReferenceValidationError>(StringBasedReferenceValueObjectWithCustomError.Create("value"));

      value.Should().BeEquivalentTo("\"value\"");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_ValidationErrorAttribute()
   {
      var value = SerializeWithConverter<BoundaryWithCustomError, BoundaryWithCustomError.ValueObjectNewtonsoftJsonConverter>(BoundaryWithCustomError.Create(1, 2), new CamelCaseNamingStrategy());

      value.Should().BeEquivalentTo("{\"lower\":1.0,\"upper\":2.0}");
   }

   [Fact]
   public void Should_serialize_value_object_with_object_key()
   {
      var obj = ObjectBaseValueObject.Create(new { Test = 1 });
      var value = Serialize<ObjectBaseValueObject, object, ValidationError>(obj);

      value.Should().BeEquivalentTo("{\"Test\":1}");
   }

   [Fact]
   public void Should_serialize_complex_value_object_with_object_property()
   {
      var obj = ComplexValueObjectWithObjectProperty.Create(new { Test = 1 });
      var value = SerializeWithConverter<ComplexValueObjectWithObjectProperty, ComplexValueObjectWithObjectProperty.ValueObjectNewtonsoftJsonConverter>(obj);

      value.Should().BeEquivalentTo("{\"Property\":{\"Test\":1}}");
   }

   private static string Serialize<T, TKey>(
      T value,
      NamingStrategy namingStrategy = null,
      NullValueHandling nullValueHandling = NullValueHandling.Include)
      where T : IObjectFactory<T, TKey, ValidationError>, IConvertible<TKey>
   {
      return Serialize<T, TKey, ValidationError>(value, namingStrategy, nullValueHandling);
   }

   private static string Serialize<T, TKey, TValidationError>(
      T value,
      NamingStrategy namingStrategy = null,
      NullValueHandling nullValueHandling = NullValueHandling.Include)
      where T : IObjectFactory<T, TKey, TValidationError>, IConvertible<TKey>
      where TValidationError : class, IValidationError<TValidationError>
   {
      return SerializeWithConverter<T, ThinktectureNewtonsoftJsonConverter<T, TKey, TValidationError>>(value, namingStrategy, nullValueHandling);
   }

   private static string SerializeNullableStruct<T, TKey>(
      T? value,
      NamingStrategy namingStrategy = null,
      NullValueHandling nullValueHandling = NullValueHandling.Include)
      where T : struct, IObjectFactory<T, TKey, ValidationError>, IConvertible<TKey>
   {
      return SerializeWithConverter<T?, ThinktectureNewtonsoftJsonConverter<T, TKey, ValidationError>>(value, namingStrategy, nullValueHandling);
   }

   private static string SerializeStruct<T, TKey>(
      T value,
      NamingStrategy namingStrategy = null,
      NullValueHandling nullValueHandling = NullValueHandling.Include)
      where T : struct, IObjectFactory<T, TKey, ValidationError>, IConvertible<TKey>
   {
      return SerializeWithConverter<T, ThinktectureNewtonsoftJsonConverter<T, TKey, ValidationError>>(value, namingStrategy, nullValueHandling);
   }

   private static string SerializeWithConverter<T, TConverter>(
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
