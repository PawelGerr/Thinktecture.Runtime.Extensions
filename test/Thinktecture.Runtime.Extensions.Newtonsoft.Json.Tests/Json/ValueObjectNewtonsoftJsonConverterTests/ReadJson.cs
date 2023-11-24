using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Json;
using Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests;

public class ReadJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
                                                                                   {
                                                                                      new object[] { null, "null" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", new CamelCaseNamingStrategy() },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}" }
                                                                                   };

   [Fact]
   public void Should_deserialize_enum_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      Deserialize<TestSmartEnum_Class_IntBased, int>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Class_StringBased, string>("null").Should().Be(null);
      DeserializeNullableStruct<TestSmartEnum_Struct_IntBased, int>("null").Should().Be(null);
      DeserializeNullableStruct<TestSmartEnum_Struct_StringBased, string>("null").Should().Be(null);
      DeserializeStruct<TestSmartEnum_Struct_IntBased, int>("0").Should().Be(default(TestSmartEnum_Struct_IntBased)); // default(int) is 0
      DeserializeStruct<TestSmartEnum_Struct_StringBased, string>("null").Should().Be(default(TestSmartEnum_Struct_StringBased));

      FluentActions.Invoking(() => DeserializeStruct<TestSmartEnum_Struct_IntBased, int>("null")).Should()
                   .Throw<JsonException>().WithMessage("Cannot convert 'Null' to a struct of type 'Int32', which is the underlying type of 'Thinktecture.Runtime.Tests.TestEnums.TestSmartEnum_Struct_IntBased'.");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      Deserialize<IntBasedReferenceValueObject, int>("null").Should().Be(null);
      Deserialize<StringBasedReferenceValueObject, string>("null").Should().Be(null);
      DeserializeNullableStruct<IntBasedStructValueObject, int>("null").Should().Be(null);
      DeserializeNullableStruct<StringBasedStructValueObject, string>("null").Should().Be(null);
      DeserializeStruct<IntBasedStructValueObject, int>("0").Should().Be(default); // default(int) is 0
      DeserializeStruct<StringBasedStructValueObject, string>("null").Should().Be(default);

      // NullInFactoryMethodsYieldsNull
      Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>("null").Should().Be(null);
      FluentActions.Invoking(() => Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>("\"\""))
                   .Should().Throw<JsonSerializationException>().WithMessage("Property cannot be empty.");
      FluentActions.Invoking(() => Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string>("\" \""))
                   .Should().Throw<JsonSerializationException>().WithMessage("Property cannot be empty.");

      // EmptyStringInFactoryMethodsYieldsNull
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>("null").Should().Be(null);
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>("\"\"").Should().Be(null);
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string>("\" \"").Should().Be(null);

      FluentActions.Invoking(() => Deserialize<IntBasedStructValueObject, int>("null")).Should()
                   .Throw<JsonException>().WithMessage("Cannot convert 'Null' to a struct of type 'Int32', which is the underlying type of 'Thinktecture.Runtime.Tests.TestValueObjects.IntBasedStructValueObject'.");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      DeserializeWithConverter<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(null);
      DeserializeWithConverter<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.ValueObjectNewtonsoftJsonConverter>("null").Should().Be(default(TestValueObject_Complex_Struct));
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
   {
      var value = Deserialize<TestEnum, string>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
   {
      var value = Deserialize<IntegerEnum, int>(json);

      value.Should().Be(expectedValue);
   }

   [Fact]
   public void Should_deserialize_using_custom_factory_specified_by_ValueObjectFactoryAttribute()
   {
      var value = Deserialize<BoundaryWithFactories, string>("\"1:2\"");

      value.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public void Should_deserialize_enum_with_ValueObjectValidationErrorAttribute()
   {
      var value = Deserialize<TestEnumWithCustomError, string, TestEnumValidationError>("\"item1\"");

      value.Should().BeEquivalentTo(TestEnumWithCustomError.Item1);
   }

   [Fact]
   public void Should_deserialize_simple_value_object_with_ValueObjectValidationErrorAttribute()
   {
      var value = Deserialize<StringBasedReferenceValueObjectWithCustomError, string, StringBasedReferenceValueObjectValidationError>("\"value\"");

      value.Should().BeEquivalentTo(StringBasedReferenceValueObjectWithCustomError.Create("value"));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_ValueObjectValidationErrorAttribute()
   {
      var value = DeserializeWithConverter<BoundaryWithCustomError, BoundaryWithCustomError.ValueObjectNewtonsoftJsonConverter>("{ \"lower\": 1, \"upper\": 2 }");

      value.Should().BeEquivalentTo(BoundaryWithCustomError.Create(1, 2));
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_having_custom_factory()
   {
      var value = Deserialize<IntBasedReferenceValueObjectWithCustomFactoryNames, int>("1");

      value.Should().BeEquivalentTo(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_having_custom_factory()
   {
      var value = DeserializeWithConverter<BoundaryWithCustomFactoryNames, BoundaryWithCustomFactoryNames.ValueObjectNewtonsoftJsonConverter>("{ \"lower\": 1, \"upper\": 2 }");

      value.Should().BeEquivalentTo(BoundaryWithCustomFactoryNames.Get(1, 2));
   }

   [Theory]
   [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
   public void Should_deserialize_value_type_with_multiple_properties(
      ValueObjectWithMultipleProperties expectedValueObject,
      string json,
      NamingStrategy namingStrategy = null)
   {
      var value = DeserializeWithConverter<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectNewtonsoftJsonConverter>(json, namingStrategy);

      value.Should().BeEquivalentTo(expectedValueObject);
   }

   [Fact]
   public void Should_throw_JsonException_if_enum_parsing_throws_UnknownEnumIdentifierException()
   {
      FluentActions.Invoking(() => Deserialize<ValidTestEnum, string>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }

   [Fact]
   public void Should_throw_JsonException_if_complex_value_object_is_not_a_json_object()
   {
      FluentActions.Invoking(() => DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"String\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("42"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"Integer\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectNewtonsoftJsonConverter>("[]"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"StartArray\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
   }

   private static T Deserialize<T, TKey>(
      string json,
      NamingStrategy namingStrategy = null)
      where T : IValueObjectFactory<T, TKey, ValidationError>, IValueObjectConvertable<TKey>
   {
      return Deserialize<T, TKey, ValidationError>(json, namingStrategy);
   }

   private static T Deserialize<T, TKey, TValidationError>(
      string json,
      NamingStrategy namingStrategy = null)
      where T : IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
      where TValidationError : class, IValidationError<TValidationError>
   {
      return DeserializeWithConverter<T, ValueObjectNewtonsoftJsonConverter<T, TKey, TValidationError>>(json, namingStrategy);
   }

   private static T? DeserializeNullableStruct<T, TKey>(
      string json,
      NamingStrategy namingStrategy = null)
      where T : struct, IValueObjectFactory<T, TKey, ValidationError>, IValueObjectConvertable<TKey>
   {
      return DeserializeWithConverter<T?, ValueObjectNewtonsoftJsonConverter<T, TKey, ValidationError>>(json, namingStrategy);
   }

   private static T DeserializeStruct<T, TKey>(
      string json,
      NamingStrategy namingStrategy = null)
      where T : struct, IValueObjectFactory<T, TKey, ValidationError>, IValueObjectConvertable<TKey>
   {
      return DeserializeWithConverter<T, ValueObjectNewtonsoftJsonConverter<T, TKey, ValidationError>>(json, namingStrategy);
   }

   private static T DeserializeWithConverter<T, TConverter>(
      string json,
      NamingStrategy namingStrategy = null)
      where TConverter : JsonConverter, new()
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
