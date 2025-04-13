using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Json.ValueObjectNewtonsoftJsonConverterTests;

public class ReadJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties =>
   [
      [null, "null"],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}"],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}"],
      [ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}"],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}"],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", new CamelCaseNamingStrategy()],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}"]
   ];

   [Fact]
   public void Should_try_deserialize_enum_when_value_is_null_or_default()
   {
      // class - int
      Deserialize<TestSmartEnum_Class_IntBased>("null").Should().Be(null);
      FluentActions.Invoking(() => Deserialize<TestSmartEnum_Class_IntBased>("0")).Should().Throw<JsonException>().WithMessage("There is no item of type 'TestSmartEnum_Class_IntBased' with the identifier '0'.");

      // [validateable] class - int
      Deserialize<TestSmartEnum_Class_IntBased_Validatable>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Class_IntBased_Validatable>("0").Should().Be(TestSmartEnum_Class_IntBased_Validatable.Get(0)); // invalid item "0"

      // class - string
      Deserialize<TestSmartEnum_Class_StringBased>("null").Should().Be(null);

      // [validateable] class - string
      Deserialize<TestSmartEnum_Class_StringBased_Validatable>("null").Should().Be(null);

      // class - class
      Deserialize<TestSmartEnum_Class_ClassBased>("null").Should().Be(null);

      // [validateable] nullable struct - int
      Deserialize<TestSmartEnum_Struct_IntBased_Validatable?>("null").Should().Be(null);
      Deserialize<TestSmartEnum_Struct_IntBased_Validatable?>("0").Should().Be(TestSmartEnum_Struct_IntBased_Validatable.Get(0));

      // [validateable] struct - int
      FluentActions.Invoking(() => Deserialize<TestSmartEnum_Struct_IntBased_Validatable>("null"))
                   .Should().Throw<JsonException>().WithMessage("Error converting value {null} to type 'System.Int32'. Path '', line 1, position 4.");
      Deserialize<TestSmartEnum_Struct_IntBased_Validatable>("0").Should().Be(TestSmartEnum_Struct_IntBased_Validatable.Get(0));

      // [validateable] nullable struct - string
      Deserialize<TestSmartEnum_Struct_StringBased_Validatable?>("null").Should().Be(null);

      // [validateable] struct - string
      FluentActions.Invoking(() => Deserialize<TestSmartEnum_Struct_StringBased_Validatable>("null")) // AllowDefaultStructs = false
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"TestSmartEnum_Struct_StringBased_Validatable\" because it doesn't allow default values.");

      // [validateable] struct - class
      FluentActions.Invoking(() => Deserialize<TestSmartEnum_Struct_ClassBased>("null")) // AllowDefaultStructs = false
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"TestSmartEnum_Struct_ClassBased\" because it doesn't allow default values.");
   }

   [Fact]
   public void Should_try_deserialize_keyed_value_object_when_value_is_null_or_default()
   {
      // class - int
      Deserialize<IntBasedReferenceValueObject>("null").Should().Be(null);

      // class - string
      Deserialize<StringBasedReferenceValueObject>("null").Should().Be(null);

      // class - class
      Deserialize<ClassBasedReferenceValueObject>("null").Should().Be(null);

      // nullable struct - int
      Deserialize<IntBasedStructValueObject?>("null").Should().Be(null);
      Deserialize<IntBasedStructValueObject?>("0").Should().Be(IntBasedStructValueObject.Create(0));

      // struct - int
      Deserialize<IntBasedStructValueObject>("0").Should().Be(IntBasedStructValueObject.Create(0)); // AllowDefaultStructs = true
      FluentActions.Invoking(() => Deserialize<IntBasedStructValueObject>("null"))
                   .Should().Throw<JsonException>().WithMessage("Error converting value {null} to type 'System.Int32'. Path '', line 1, position 4.");

      FluentActions.Invoking(() => Deserialize<IntBasedStructValueObjectDoesNotAllowDefaultStructs>("0")) // AllowDefaultStructs = true
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"IntBasedStructValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      // nullable struct - string
      Deserialize<StringBasedStructValueObject?>("null").Should().Be(null);

      // struct - string
      FluentActions.Invoking(() => Deserialize<StringBasedStructValueObject>("null")) // AllowDefaultStructs = false
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"StringBasedStructValueObject\" because it doesn't allow default values.");

      // struct - class
      FluentActions.Invoking(() => Deserialize<ReferenceTypeBasedStructValueObjectDoesNotAllowDefaultStructs>("null")) // AllowDefaultStructs = false
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"ReferenceTypeBasedStructValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");
   }

   [Fact]
   public void Should_deserialize_value_objects_with_NullInFactoryMethodsYieldsNull()
   {
      Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull>("null").Should().Be(null);

      FluentActions.Invoking(() => Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull>("\"\""))
                   .Should().Throw<JsonException>().WithMessage("Property cannot be empty.");

      FluentActions.Invoking(() => Deserialize<StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull>("\" \""))
                   .Should().Throw<JsonException>().WithMessage("Property cannot be empty.");
   }

   [Fact]
   public void Should_deserialize_value_objects_with_EmptyStringInFactoryMethodsYieldsNull()
   {
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull>("null").Should().Be(null);
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull>("\"\"").Should().Be(null);
      Deserialize<StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull>("\" \"").Should().Be(null);
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      Deserialize<TestValueObject_Complex_Class>("null").Should().Be(null);
      Deserialize<TestValueObject_Complex_Struct?>("null").Should().Be(null);
      Deserialize<TestValueObject_Complex_Struct>("null").Should().Be(default(TestValueObject_Complex_Struct));
   }

   [Theory]
   [MemberData(nameof(DataForStringBasedEnumTest))]
   public void Should_deserialize_string_based_enum(TestEnum expectedValue, string json)
   {
      var value = Deserialize<TestEnum>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_deserialize_int_based_enum(IntegerEnum expectedValue, string json)
   {
      var value = Deserialize<IntegerEnum>(json);

      value.Should().Be(expectedValue);
   }

   [Fact]
   public void Should_deserialize_using_custom_factory_specified_by_ValueObjectFactoryAttribute()
   {
      var value = Deserialize<BoundaryWithFactories>("\"1:2\"");

      value.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public void Should_deserialize_enum_with_ValueObjectValidationErrorAttribute()
   {
      var value = Deserialize<TestEnumWithCustomError>("\"item1\"");

      value.Should().BeEquivalentTo(TestEnumWithCustomError.Item1);
   }

   [Fact]
   public void Should_deserialize_simple_value_object_with_ValueObjectValidationErrorAttribute()
   {
      var value = Deserialize<StringBasedReferenceValueObjectWithCustomError>("\"value\"");

      value.Should().BeEquivalentTo(StringBasedReferenceValueObjectWithCustomError.Create("value"));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_ValueObjectValidationErrorAttribute()
   {
      var value = Deserialize<BoundaryWithCustomError>("{ \"lower\": 1, \"upper\": 2 }");

      value.Should().BeEquivalentTo(BoundaryWithCustomError.Create(1, 2));
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_having_custom_factory()
   {
      var value = Deserialize<IntBasedReferenceValueObjectWithCustomFactoryNames>("1");

      value.Should().BeEquivalentTo(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_having_custom_factory()
   {
      var value = Deserialize<BoundaryWithCustomFactoryNames>("{ \"lower\": 1, \"upper\": 2 }");

      value.Should().BeEquivalentTo(BoundaryWithCustomFactoryNames.Get(1, 2));
   }

   [Theory]
   [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
   public void Should_deserialize_value_type_with_multiple_properties(
      ValueObjectWithMultipleProperties expectedValueObject,
      string json,
      NamingStrategy namingStrategy = null)
   {
      var value = Deserialize<ValueObjectWithMultipleProperties>(json, namingStrategy);

      value.Should().BeEquivalentTo(expectedValueObject);
   }

   [Fact]
   public void Should_throw_JsonException_if_enum_parsing_throws_UnknownEnumIdentifierException()
   {
      FluentActions.Invoking(() => Deserialize<ValidTestEnum>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }

   [Fact]
   public void Should_throw_JsonException_if_complex_value_object_is_not_a_json_object()
   {
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"String\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class>("42"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"Integer\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class>("[]"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"StartArray\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
   }

   [Fact]
   public void Should_deserialize_empty_complex_value_object()
   {
      Deserialize<EmptyComplexValueObjectDoesNotAllowDefaultStructs>("{}")
         .Should().Be(EmptyComplexValueObjectDoesNotAllowDefaultStructs.Create());
   }

   [Fact]
   public void Should_throw_if_AllowDefaultStructs_is_disabled_on_complex_value_object_and_value_is_null()
   {
      // null as root
      FluentActions.Invoking(() => Deserialize<EmptyComplexValueObjectDoesNotAllowDefaultStructs>("null"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"EmptyComplexValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithInt>("null"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithInt\" because it doesn't allow default values.");

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct>("null"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct\" because it doesn't allow default values.");

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct>("null"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct\" because it doesn't allow default values.");

      // null as property
      FluentActions.Invoking(() => Deserialize<GenericClass<EmptyComplexValueObjectDoesNotAllowDefaultStructs>>("{\"Property\": null }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"EmptyComplexValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      FluentActions.Invoking(() => Deserialize<GenericClass<ComplexValueObjectDoesNotAllowDefaultStructsWithInt>>("{\"Property\": null }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithInt\" because it doesn't allow default values.");

      FluentActions.Invoking(() => Deserialize<GenericClass<ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct>>("{\"Property\": null }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct\" because it doesn't allow default values.");

      FluentActions.Invoking(() => Deserialize<GenericClass<ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct>>("{\"Property\": null }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct\" because it doesn't allow default values.");

      // Impossible to handle missing properties if the parent type is not a complex value object but a POCO
   }

   [Fact]
   public void Should_throw_if_complex_value_object_property_has_AllowDefaultStructs_equals_to_false_and_value_is_null_or_default()
   {
      // property is null or default
      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithInt>("{ \"Property\": 0 }"))
                   .Should().NotThrow();

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct>("{ \"Property\": 0 }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"IntBasedStructValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct>("{ \"Property\": null }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"StringBasedStructValueObject\" because it doesn't allow default values.");

      // missing property
      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithInt>("{ }"))
                   .Should().NotThrow();

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct>("{ }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot deserialize type \"ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct\" because the member \"Property\" of type \"global::Thinktecture.Runtime.Tests.TestValueObjects.IntBasedStructValueObjectDoesNotAllowDefaultStructs\" is missing and does not allow default values.");

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct>("{ }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot deserialize type \"ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct\" because the member \"Property\" of type \"global::Thinktecture.Runtime.Tests.TestValueObjects.StringBasedStructValueObject\" is missing and does not allow default values.");
   }

   [Fact]
   public void Should_throw_if_complex_value_object_property_has_AllowDefaultStructs_but_the_property_non_nullable()
   {
      FluentActions.Invoking(() => Deserialize<ComplexValueObjectWithNonNullProperty>("{ \"Property\": null }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot deserialize type \"ComplexValueObjectWithNonNullProperty\" because the member \"Property\" of type \"string\" must not be null.");

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectWithPropertyWithoutNullableAnnotation>("{ \"Property\": null }"))
                   .Should().NotThrow();
   }

   private static T Deserialize<T>(
      string json,
      NamingStrategy namingStrategy)
   {
      using var reader = new StringReader(json);
      using var jsonReader = new JsonTextReader(reader);

      var settings = new JsonSerializerSettings
                     {
                        ContractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy }
                     };

      var serializer = JsonSerializer.CreateDefault(settings);

      return serializer.Deserialize<T>(jsonReader);
   }

   private static T Deserialize<T>(string json)
   {
      return JsonConvert.DeserializeObject<T>(json);
   }
}
