using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests;

public class ReadJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
                                                                                   {
                                                                                      new object[] { null, "null" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}" },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase },
                                                                                      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}", null, true }
                                                                                   };

   [Fact]
   public void Should_deserialize_enum_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      DeserializeWithConverter<TestSmartEnum_Class_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_IntBased, int, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<TestSmartEnum_Class_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Class_StringBased, string, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<TestSmartEnum_Struct_IntBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<TestSmartEnum_Struct_StringBased?, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<TestSmartEnum_Struct_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int, ValidationError>>("0").Should().Be(default(TestSmartEnum_Struct_IntBased)); // default(int) is 0
      DeserializeWithConverter<TestSmartEnum_Struct_StringBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_StringBased, string, ValidationError>>("null").Should().Be(default(TestSmartEnum_Struct_StringBased));

      FluentActions.Invoking(() => DeserializeWithConverter<TestSmartEnum_Struct_IntBased, ValueObjectJsonConverterFactory<TestSmartEnum_Struct_IntBased, int, ValidationError>>("null")).Should()
                   .Throw<JsonException>()
                   .WithInnerException<InvalidOperationException>().WithMessage("Cannot get the value of a token type 'Null' as a number.");
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      DeserializeWithConverter<IntBasedReferenceValueObject, ValueObjectJsonConverterFactory<IntBasedReferenceValueObject, int, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<StringBasedReferenceValueObject, ValueObjectJsonConverterFactory<StringBasedReferenceValueObject, string, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<IntBasedStructValueObject?, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<StringBasedStructValueObject?, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<IntBasedStructValueObject, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int, ValidationError>>("0").Should().Be(default); // default(int) is 0
      DeserializeWithConverter<StringBasedStructValueObject, ValueObjectJsonConverterFactory<StringBasedStructValueObject, string, ValidationError>>("null").Should().Be(default);

      // NullInFactoryMethodsYieldsNull
      DeserializeWithConverter<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string, ValidationError>>("null").Should().Be(null);
      FluentActions.Invoking(() => DeserializeWithConverter<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string, ValidationError>>("\"\""))
                   .Should().Throw<JsonException>().WithMessage("Property cannot be empty.");
      FluentActions.Invoking(() => DeserializeWithConverter<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull, string, ValidationError>>("\" \""))
                   .Should().Throw<JsonException>().WithMessage("Property cannot be empty.");

      // EmptyStringInFactoryMethodsYieldsNull
      DeserializeWithConverter<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string, ValidationError>>("null").Should().Be(null);
      DeserializeWithConverter<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string, ValidationError>>("\"\"").Should().Be(null);
      DeserializeWithConverter<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, ValueObjectJsonConverterFactory<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull, string, ValidationError>>("\" \"").Should().Be(null);

      FluentActions.Invoking(() => DeserializeWithConverter<IntBasedStructValueObject, ValueObjectJsonConverterFactory<IntBasedStructValueObject, int, ValidationError>>("null")).Should()
                   .Throw<JsonException>()
                   .WithInnerException<InvalidOperationException>().WithMessage("Cannot get the value of a token type 'Null' as a number.");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("null").Should().Be(null);
      DeserializeWithConverter<TestValueObject_Complex_Struct?, TestValueObject_Complex_Struct.ValueObjectJsonConverterFactory>("null").Should().Be(null);
      DeserializeWithConverter<TestValueObject_Complex_Struct, TestValueObject_Complex_Struct.ValueObjectJsonConverterFactory>("null").Should().Be(default(TestValueObject_Complex_Struct));
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
   {
      var value = DeserializeWithConverter<TestEnum, ValueObjectJsonConverterFactory<TestEnum, string, ValidationError>>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
   {
      var value = DeserializeWithConverter<IntegerEnum, ValueObjectJsonConverterFactory<IntegerEnum, int, ValidationError>>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_deserialize_class_containing_string_based_enum(ClassWithStringBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = DeserializeWithConverter<ClassWithStringBasedEnum, ValueObjectJsonConverterFactory<TestEnum, string, ValidationError>>(json, ignoreNullValues: ignoreNullValues);

      value.Should().BeEquivalentTo(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_deserialize_class_containing_int_based_enum(ClassWithIntBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = DeserializeWithConverter<ClassWithIntBasedEnum, ValueObjectJsonConverterFactory<IntegerEnum, int, ValidationError>>(json, ignoreNullValues: ignoreNullValues);

      value.Should().BeEquivalentTo(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
   public void Should_deserialize_value_type_with_multiple_properties(
      ValueObjectWithMultipleProperties expectedValueObject,
      string json,
      JsonNamingPolicy namingPolicy = null,
      bool propertyNameCaseInsensitive = false)
   {
      var value = DeserializeWithConverter<ValueObjectWithMultipleProperties, ValueObjectWithMultipleProperties.ValueObjectJsonConverterFactory>(json, namingPolicy, propertyNameCaseInsensitive);

      value.Should().BeEquivalentTo(expectedValueObject);
   }

   [Fact]
   public void Should_throw_JsonException_if_enum_parsing_throws_UnknownEnumIdentifierException()
   {
      FluentActions.Invoking(() => DeserializeWithConverter<ValidTestEnum, ValueObjectJsonConverterFactory<ValidTestEnum, string, ValidationError>>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }

   [Fact]
   public void Should_throw_JsonException_if_complex_value_object_is_not_a_json_object()
   {
      FluentActions.Invoking(() => DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"String\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("42"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"Number\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => DeserializeWithConverter<TestValueObject_Complex_Class, TestValueObject_Complex_Class.ValueObjectJsonConverterFactory>("[]"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"StartArray\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
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
      var value = DeserializeWithConverter<BoundaryWithCustomError, BoundaryWithCustomError.ValueObjectJsonConverterFactory>("{ \"lower\": 1, \"upper\": 2 }", JsonNamingPolicy.CamelCase);

      value.Should().BeEquivalentTo(BoundaryWithCustomError.Create(1, 2));
   }

   private static T Deserialize<T, TKey>(
      string json,
      JsonNamingPolicy namingPolicy = null)
      where T : IValueObjectFactory<T, TKey, ValidationError>, IValueObjectConvertable<TKey>
   {
      return Deserialize<T, TKey, ValidationError>(json, namingPolicy);
   }

   private static T Deserialize<T, TKey, TValidationError>(
      string json,
      JsonNamingPolicy namingPolicy = null)
      where T : IValueObjectFactory<T, TKey, TValidationError>, IValueObjectConvertable<TKey>
      where TValidationError : class, IValidationError<TValidationError>
   {
      return DeserializeWithConverter<T, ValueObjectJsonConverterFactory>(json, namingPolicy);
   }

   private static T DeserializeWithConverter<T, TConverterFactory>(
      string json,
      JsonNamingPolicy namingPolicy = null,
      bool propertyNameCaseInsensitive = false,
      bool ignoreNullValues = false)
      where TConverterFactory : JsonConverterFactory, new()
   {
      var factory = new TConverterFactory();
      var options = new JsonSerializerOptions
                    {
                       Converters = { factory },
                       PropertyNamingPolicy = namingPolicy,
                       PropertyNameCaseInsensitive = propertyNameCaseInsensitive,
                       DefaultIgnoreCondition = ignoreNullValues ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never
                    };

      return JsonSerializer.Deserialize<T>(json, options);
   }
}
