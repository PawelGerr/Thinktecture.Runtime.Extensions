using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests.TestClasses;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

public class ReadJson : JsonTestsBase
{
   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties =>
   [
      [null, "null"],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0,\"NullableStructProperty\":null,\"ReferenceProperty\":null}"],
      [ValueObjectWithMultipleProperties.Create(0, null, null!), "{\"StructProperty\":0}"],
      [ValueObjectWithMultipleProperties.Create(0, 0, String.Empty), "{\"StructProperty\":0,\"NullableStructProperty\":0,\"ReferenceProperty\":\"\"}"],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"StructProperty\":1,\"NullableStructProperty\":42,\"ReferenceProperty\":\"Value\"}"],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structProperty\":1,\"nullableStructProperty\":42,\"referenceProperty\":\"Value\"}", JsonNamingPolicy.CamelCase],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value"), "{\"structproperty\":1,\"NULLABLESTRUCTPROPERTY\":42,\"ReFeReNCePRoPeRTy\":\"Value\"}", null, true]
   ];

   [Fact]
   public void Should_try_deserialize_enum_when_value_is_null_or_default()
   {
      // class - int
      Deserialize<SmartEnum_IntBased>("null").Should().Be(null);
      FluentActions.Invoking(() => Deserialize<SmartEnum_IntBased>("0")).Should().Throw<JsonException>().WithMessage("There is no item of type 'SmartEnum_IntBased' with the identifier '0'.");

      // class - string
      Deserialize<SmartEnum_StringBased>("null").Should().Be(null);

      // class - class
      Deserialize<SmartEnum_ClassBased>("null").Should().Be(null);
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
                   .Should().Throw<JsonException>().WithMessage("The JSON value could not be converted to Thinktecture.Runtime.Tests.TestValueObjects.IntBasedStructValueObject. Path: $ | LineNumber: 0 | BytePositionInLine: 4.");

      Deserialize<IntBasedStructValueObjectDoesNotAllowDefaultStructs>("0").Should().Be(IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(0)); // AllowDefaultStructs = true

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
   public void Should_not_throw_if_AllowDefaultStructs_is_disabled_on_keyed_value_object_and_value_is_default()
   {
      FluentActions.Invoking(() => Deserialize<GenericClass<IntBasedStructValueObjectDoesNotAllowDefaultStructs>>("{\"Property\": 0 }"))
                   .Should().NotThrow();
   }

   [Fact]
   public void Should_throw_if_AllowDefaultStructs_is_disabled_on_keyed_value_object_and_value_is_null()
   {
      FluentActions.Invoking(() => Deserialize<GenericClass<StringBasedStructValueObject>>("{\"Property\": null }"))
                   .Should().Throw<JsonException>().WithMessage("Cannot convert null to type \"StringBasedStructValueObject\" because it doesn't allow default values.");
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
   public void Should_deserialize_string_based_enum(SmartEnum_StringBased expectedValue, string json)
   {
      var value = Deserialize<SmartEnum_StringBased>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForIntBasedEnumTest))]
   public void Should_deserialize_int_based_enum(SmartEnum_IntBased expectedValue, string json)
   {
      var value = Deserialize<SmartEnum_IntBased>(json);

      value.Should().Be(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithStringBasedEnumTest))]
   public void Should_deserialize_class_containing_string_based_enum(ClassWithStringBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = Deserialize<ClassWithStringBasedEnum>(json, ignoreNullValues: ignoreNullValues);

      value.Should().BeEquivalentTo(expectedValue);
   }

   [Theory]
   [MemberData(nameof(DataForClassWithIntBasedEnumTest))]
   public void Should_deserialize_class_containing_int_based_enum(ClassWithIntBasedEnum expectedValue, string json, bool ignoreNullValues = false)
   {
      var value = Deserialize<ClassWithIntBasedEnum>(json, ignoreNullValues: ignoreNullValues);

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
      var value = Deserialize<ValueObjectWithMultipleProperties>(json, namingPolicy, propertyNameCaseInsensitive);

      value.Should().BeEquivalentTo(expectedValueObject);
   }

   [Fact]
   public void Should_throw_JsonException_if_enum_parsing_throws_UnknownSmartEnumIdentifierException()
   {
      FluentActions.Invoking(() => Deserialize<SmartEnum_StringBased>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("There is no item of type 'SmartEnum_StringBased' with the identifier 'invalid'.");
   }

   [Fact]
   public void Should_throw_JsonException_if_complex_value_object_is_not_a_json_object()
   {
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class>("\"invalid\""))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"String\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class>("42"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"Number\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
      FluentActions.Invoking(() => Deserialize<TestValueObject_Complex_Class>("[]"))
                   .Should().Throw<JsonException>().WithMessage("Unexpected token \"StartArray\" when trying to deserialize \"TestValueObject_Complex_Class\". Expected token: \"StartObject\".");
   }

   [Fact]
   public void Should_deserialize_using_custom_factory_specified_by_ObjectFactoryAttribute()
   {
      var value = Deserialize<BoundaryWithFactories>("\"1:2\"");

      value.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public void Should_deserialize_enum_with_ValidationErrorAttribute()
   {
      var value = Deserialize<TestSmartEnum_CustomError>("\"item1\"");

      value.Should().BeEquivalentTo(TestSmartEnum_CustomError.Item1);
   }

   [Fact]
   public void Should_deserialize_simple_value_object_with_ValidationErrorAttribute()
   {
      var value = Deserialize<StringBasedReferenceValueObjectWithCustomError>("\"value\"");

      value.Should().BeEquivalentTo(StringBasedReferenceValueObjectWithCustomError.Create("value"));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_ValidationErrorAttribute()
   {
      var value = Deserialize<BoundaryWithCustomError>("{ \"lower\": 1, \"upper\": 2 }", JsonNamingPolicy.CamelCase);

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
      var value = Deserialize<BoundaryWithCustomFactoryNames>("{ \"lower\": 1, \"upper\": 2 }", JsonNamingPolicy.CamelCase);

      value.Should().BeEquivalentTo(BoundaryWithCustomFactoryNames.Get(1, 2));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_numbers_as_string()
   {
      var value = Deserialize<Boundary>("""{ "lower": "1", "upper": 2}""",
                                        namingPolicy: JsonNamingPolicy.CamelCase,
                                        numberHandling: JsonNumberHandling.AllowReadingFromString);

      value.Should().BeEquivalentTo(Boundary.Create(1, 2));
   }

   [Fact]
   public void Should_throw_if_non_string_based_enum_is_used_as_dictionary_key()
   {
      FluentActions.Invoking(() => Deserialize<Dictionary<SmartEnum_IntBased, int>>("""{ "1": 1 }"""))
                   .Should().Throw<NotSupportedException>();
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
   public void Should_not_throw_if_complex_value_object_property_has_AllowDefaultStructs_equals_to_false_and_value_is_default()
   {
      // property is default
      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithInt>("{ \"Property\": 0 }"))
                   .Should().NotThrow();

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct>("{ \"Property\": 0 }"))
                   .Should().NotThrow();
   }

   [Fact]
   public void Should_throw_if_complex_value_object_property_has_AllowDefaultStructs_equals_to_false_and_value_is_null_or_default()
   {
      // property is null
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
                   .Should().Throw<JsonException>().WithMessage("The member \"Property\" of type \"ComplexValueObjectWithNonNullProperty\" must not be null.");

      FluentActions.Invoking(() => Deserialize<ComplexValueObjectWithPropertyWithoutNullableAnnotation>("{ \"Property\": null }"))
                   .Should().NotThrow();
   }
}
