using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using MessagePack;
using MessagePack.Resolvers;
using Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests;

public partial class RoundTripSerialize
{
   private static readonly MethodInfo _serializeRoundTripMethodInfo = typeof(RoundTripSerialize).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                                                                       .SingleOrDefault(m => m.Name == nameof(RoundTrip) && m.GetParameters().Length == 1)
                                                                      ?? throw new Exception($"Method '{nameof(RoundTrip)}' not found.");

   private readonly MessagePackSerializerOptions _options;

   public RoundTripSerialize()
   {
      var resolver = CompositeResolver.Create(ThinktectureMessageFormatterResolver.Instance, StandardResolver.Instance);
      _options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
   }

   [Fact]
   public void Should_try_deserialize_enum_when_value_is_null_or_default()
   {
      // class - int
      RoundTrip((object)null, (TestSmartEnum_Class_IntBased)null);
      FluentActions.Invoking(() => RoundTrip(0, (TestSmartEnum_Class_IntBased)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<ValidationException>().WithMessage("There is no item of type 'TestSmartEnum_Class_IntBased' with the identifier '0'.");

      // [validateable] class - int
      RoundTrip((object)null, (TestSmartEnum_Class_IntBased_Validatable)null);
      RoundTrip(0, TestSmartEnum_Class_IntBased_Validatable.Get(0)); // invalid item "0"

      // class - string
      RoundTrip((object)null, (TestSmartEnum_Class_StringBased)null);

      // [validateable] class - string
      RoundTrip((object)null, (TestSmartEnum_Class_StringBased_Validatable)null);

      // class - class
      RoundTrip((object)null, (TestSmartEnum_Class_ClassBased)null);

      // [validateable] nullable struct - int
      RoundTrip((object)null, (TestSmartEnum_Struct_IntBased_Validatable?)null);
      RoundTrip(0, (TestSmartEnum_Struct_IntBased_Validatable?)TestSmartEnum_Struct_IntBased_Validatable.Get(0));

      // [validateable] struct - int
      FluentActions.Invoking(() => RoundTrip((object)null, default(TestSmartEnum_Struct_IntBased_Validatable)))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Unexpected msgpack code 192 (nil) encountered.");
      RoundTrip(0, TestSmartEnum_Struct_IntBased_Validatable.Get(0));

      // [validateable] nullable struct - string
      RoundTrip((object)null, (TestSmartEnum_Struct_StringBased_Validatable?)null);

      // [validateable] struct - string
      FluentActions.Invoking(() => RoundTrip((object)null, (TestSmartEnum_Struct_StringBased_Validatable)null)) // AllowDefaultStructs = false
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"TestSmartEnum_Struct_StringBased_Validatable\" because it doesn't allow default values.");

      // [validateable] struct - class
      FluentActions.Invoking(() => RoundTrip((object)null, (TestSmartEnum_Struct_ClassBased)null)) // AllowDefaultStructs = false
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"TestSmartEnum_Struct_ClassBased\" because it doesn't allow default values.");
   }

   [Fact]
   public void Should_try_deserialize_keyed_value_object_when_value_is_null_or_default()
   {
      // class - int
      RoundTrip((object)null, (IntBasedReferenceValueObject)null);

      // class - string
      RoundTrip((object)null, (StringBasedReferenceValueObject)null);

      // class - class
      RoundTrip((object)null, (ClassBasedReferenceValueObject)null);

      // nullable struct - int
      RoundTrip((object)null, (IntBasedStructValueObject?)null);
      RoundTrip(0, (IntBasedStructValueObject?)IntBasedStructValueObject.Create(0));

      // struct - int
      RoundTrip(0, IntBasedStructValueObject.Create(0)); // AllowDefaultStructs = true
      FluentActions.Invoking(() => RoundTrip((object)null, default(IntBasedStructValueObject)))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Unexpected msgpack code 192 (nil) encountered.");

      FluentActions.Invoking(() => RoundTrip(0, IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(-1))) // AllowDefaultStructs = true
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert the value 0 to type \"IntBasedStructValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      // nullable struct - string
      RoundTrip((object)null, (StringBasedStructValueObject?)null);

      // struct - string
      FluentActions.Invoking(() => RoundTrip((object)null, StringBasedStructValueObject.Create(""))) // AllowDefaultStructs = false
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"StringBasedStructValueObject\" because it doesn't allow default values.");

      // struct - class
      FluentActions.Invoking(() => RoundTrip((object)null, ReferenceTypeBasedStructValueObjectDoesNotAllowDefaultStructs.Create(new TestClass()))) // AllowDefaultStructs = false
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"ReferenceTypeBasedStructValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");
   }

   [Fact]
   public void Should_throw_if_AllowDefaultStructs_is_disabled_on_keyed_value_object_and_value_is_null_and_default()
   {
      FluentActions.Invoking(() => RoundTrip(new GenericClass<int>(0), (GenericClass<IntBasedStructValueObjectDoesNotAllowDefaultStructs>)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert the value 0 to type \"IntBasedStructValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), (GenericClass<StringBasedStructValueObject>)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"StringBasedStructValueObject\" because it doesn't allow default values.");
   }

   [Fact]
   public void Should_deserialize_value_objects_with_NullInFactoryMethodsYieldsNull()
   {
      RoundTrip((object)null, (StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull)null);

      FluentActions.Invoking(() => RoundTrip("", (StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<ValidationException>().WithMessage("Property cannot be empty.");

      FluentActions.Invoking(() => RoundTrip(" ", (StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<ValidationException>().WithMessage("Property cannot be empty.");
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      RoundTrip<TestValueObject_Complex_Class_WithFormatter>(null);
      RoundTrip<TestValueObject_Complex_Struct_WithFormatter?>(null);
      RoundTrip<TestValueObject_Complex_Struct_WithFormatter>(default);
   }

   [Fact]
   public void Should_roundtrip_serialize_string_based_enum_having_formatter()
   {
      var bytes = MessagePackSerializer.Serialize(StringBasedEnumWithFormatter.ValueA, StandardResolver.Options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<StringBasedEnumWithFormatter>(bytes, StandardResolver.Options, CancellationToken.None);

      value.Should().Be(StringBasedEnumWithFormatter.ValueA);
   }

   [Fact]
   public void Should_roundtrip_serialize_int_based_enum_having_formatter()
   {
      var bytes = MessagePackSerializer.Serialize(IntBasedEnumWithFormatter.Value1, StandardResolver.Options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<IntBasedEnumWithFormatter>(bytes, StandardResolver.Options, CancellationToken.None);

      value.Should().Be(IntBasedEnumWithFormatter.Value1);
   }

   public static IEnumerable<object[]> DataForValueObject =>
   [
      [new ClassWithIntBasedEnum(IntegerEnum.Item1)],
      [new ClassWithStringBasedEnum(TestEnum.Item1)],
      [TestEnum.Item1],
      [IntegerEnum.Item1]
   ];

   [Theory]
   [MemberData(nameof(DataForValueObject))]
   public void Should_roundtrip_serialize(object value)
   {
      _serializeRoundTripMethodInfo.MakeGenericMethod(value.GetType()).Invoke(this, [value]);
   }

   private void RoundTrip<T>(T value)
   {
      var bytes = MessagePackSerializer.Serialize(value, _options, CancellationToken.None);
      var deserializedValue = MessagePackSerializer.Deserialize<T>(bytes, _options, CancellationToken.None);

      deserializedValue.Should().Be(value);
   }

   private void RoundTrip<TIn, TOut>(TIn serialize, TOut expected)
   {
      var bytes = MessagePackSerializer.Serialize(serialize, _options, CancellationToken.None);
      var deserializedValue = MessagePackSerializer.Deserialize<TOut>(bytes, _options, CancellationToken.None);

      deserializedValue.Should().Be(expected);
   }

   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties =>
   [
      [null],
      [ValueObjectWithMultipleProperties.Create(0, null, null!)],
      [ValueObjectWithMultipleProperties.Create(0, null, null!)],
      [ValueObjectWithMultipleProperties.Create(0, 0, String.Empty)],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value")],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value")],
      [ValueObjectWithMultipleProperties.Create(1, 42, "Value")]
   ];

   [Theory]
   [MemberData(nameof(DataForValueObjectWithMultipleProperties))]
   public void Should_roundtrip_serialize_ValueObjectWithMultipleProperties(ValueObjectWithMultipleProperties expectedValueObject)
   {
      var bytes = MessagePackSerializer.Serialize(expectedValueObject);
      var value = MessagePackSerializer.Deserialize<ValueObjectWithMultipleProperties>(bytes);

      value.Should().BeEquivalentTo(expectedValueObject);
   }

   [Theory]
   [InlineData(false)]
   [InlineData(true)]
   public void Should_roundtrip_serialize_using_factory_specified_by_ObjectFactoryAttribute(bool withCustomResolver)
   {
      var options = withCustomResolver ? _options : StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(BoundaryWithFactories.Create(1, 2), options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<BoundaryWithFactories>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public void Should_roundtrip_serialize_enum_with_ValidationErrorAttribute()
   {
      var options = StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(TestEnumWithCustomError.Item1, options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<TestEnumWithCustomError>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(TestEnumWithCustomError.Item1);
   }

   [Fact]
   public void Should_deserialize_simple_value_object_with_ValidationErrorAttribute()
   {
      var options = StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(StringBasedReferenceValueObjectWithCustomError.Create("value"), options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<StringBasedReferenceValueObjectWithCustomError>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(StringBasedReferenceValueObjectWithCustomError.Create("value"));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_ValidationErrorAttribute()
   {
      var options = StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(BoundaryWithCustomError.Create(1, 2), options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<BoundaryWithCustomError>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(BoundaryWithCustomError.Create(1, 2));
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_having_custom_factory()
   {
      var options = StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1), options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<IntBasedReferenceValueObjectWithCustomFactoryNames>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_having_custom_factory()
   {
      var options = StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(BoundaryWithCustomFactoryNames.Get(1, 2), options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<BoundaryWithCustomFactoryNames>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(BoundaryWithCustomFactoryNames.Get(1, 2));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_having_keys()
   {
      RoundTrip(ComplexValueObjectWithTwoPropertiesAndKeys.Create(1, "Test"));
      RoundTrip(ComplexValueObjectWith3PropertiesAndKeys.Create(1, "Test", 2));
      RoundTrip(ComplexValueObjectWith3PropertiesAndKeys.Create(1, "Test", 2), ComplexValueObjectWithTwoPropertiesAndKeys.Create(1, "Test"));
      RoundTrip(ComplexValueObjectWithKeysAndGaps.Create(1, "Test", 2));
      RoundTrip(ComplexValueObjectWithPartialKeys.Create(1, "Test", 2, true));
   }

   public static IEnumerable<object[]> ObjectWithStructTestData =
   [
      [new GenericClass<IntBasedStructValueObject>(IntBasedStructValueObject.Create(42))],
      [new GenericClass<IntBasedStructValueObject?>(IntBasedStructValueObject.Create(42))],
      [new GenericClass<IntBasedReferenceValueObject>(IntBasedReferenceValueObject.Create(42))],
      [new GenericStruct<IntBasedStructValueObject>(IntBasedStructValueObject.Create(42))],
      [new GenericStruct<IntBasedStructValueObject?>(IntBasedStructValueObject.Create(42))],
      [new GenericStruct<IntBasedReferenceValueObject>(IntBasedReferenceValueObject.Create(42))],
   ];

   [Theory]
   [MemberData(nameof(ObjectWithStructTestData))]
   public void Should_roundtrip_serialize_types_with_struct_properties_using_resolver(object obj)
   {
      Roundtrip_serialize_types_with_struct_properties_using_resolver(true, obj);
      Roundtrip_serialize_types_with_struct_properties_using_resolver(false, obj);
   }

   [Fact]
   public void Should_deserialize_empty_complex_value_object()
   {
      RoundTrip(new TestClass(), EmptyComplexValueObjectDoesNotAllowDefaultStructs.Create());
   }

   [Fact]
   public void Should_throw_if_AllowDefaultStructs_is_disabled_on_complex_value_object_and_value_is_null()
   {
      // null as root
      FluentActions.Invoking(() => RoundTrip((object)null, EmptyComplexValueObjectDoesNotAllowDefaultStructs.Create()))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"EmptyComplexValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip((object)null, ComplexValueObjectDoesNotAllowDefaultStructsWithInt.Create(0)))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithInt\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip((object)null, ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct.Create(IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(0))))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip((object)null, ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct.Create(StringBasedStructValueObject.Create(""))))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct\" because it doesn't allow default values.");

      // null as property
      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), (GenericClass<EmptyComplexValueObjectDoesNotAllowDefaultStructs>)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"EmptyComplexValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), (GenericClass<ComplexValueObjectDoesNotAllowDefaultStructsWithInt>)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithInt\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), (GenericClass<ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct>)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), (GenericClass<ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct>)null))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct\" because it doesn't allow default values.");

      // Impossible to handle missing properties if the parent type is not a complex value object but a POCO
   }

   [Fact]
   public void Should_throw_if_complex_value_object_property_has_AllowDefaultStructs_equals_to_false_and_value_is_null_or_default()
   {
      // property is null or default
      FluentActions.Invoking(() => RoundTrip(new GenericClass<int>(0), ComplexValueObjectDoesNotAllowDefaultStructsWithInt.Create(0)))
                   .Should().NotThrow();

      FluentActions.Invoking(() => RoundTrip(new GenericClass<int>(0), ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct.Create(IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(0))))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert the value 0 to type \"IntBasedStructValueObjectDoesNotAllowDefaultStructs\" because it doesn't allow default values.");

      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct.Create(StringBasedStructValueObject.Create(""))))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot convert null to type \"StringBasedStructValueObject\" because it doesn't allow default values.");

      // missing property
      FluentActions.Invoking(() => RoundTrip(new TestClass(), ComplexValueObjectDoesNotAllowDefaultStructsWithInt.Create(0)))
                   .Should().NotThrow();

      FluentActions.Invoking(() => RoundTrip(new TestClass(), ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct.Create(IntBasedStructValueObjectDoesNotAllowDefaultStructs.Create(0))))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot deserialize type \"ComplexValueObjectDoesNotAllowDefaultStructsWithIntBasedStruct\" because the member \"Property\" of type \"global::Thinktecture.Runtime.Tests.TestValueObjects.IntBasedStructValueObjectDoesNotAllowDefaultStructs\" is missing and does not allow default values.");

      FluentActions.Invoking(() => RoundTrip(new TestClass(), ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct.Create(StringBasedStructValueObject.Create(""))))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("Cannot deserialize type \"ComplexValueObjectDoesNotAllowDefaultStructsWithStringBasedStruct\" because the member \"Property\" of type \"global::Thinktecture.Runtime.Tests.TestValueObjects.StringBasedStructValueObject\" is missing and does not allow default values.");
   }

   [Fact]
   public void Should_throw_if_complex_value_object_property_has_AllowDefaultStructs_but_the_property_non_nullable()
   {
      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), ComplexValueObjectWithNonNullProperty.Create("")))
                   .Should().Throw<MessagePackSerializationException>()
                   .WithInnerException<MessagePackSerializationException>().WithMessage("The member \"Property\" of type \"ComplexValueObjectWithNonNullProperty\" must not be null.");

      FluentActions.Invoking(() => RoundTrip(new GenericClass<object>(null), ComplexValueObjectWithPropertyWithoutNullableAnnotation.Create(null!)))
                   .Should().NotThrow();
   }

   private static void Roundtrip_serialize_types_with_struct_properties_using_resolver(
      bool skipValueObjectsWithMessagePackFormatter,
      object obj)
   {
      var resolver = CompositeResolver.Create(new ThinktectureMessageFormatterResolver(skipValueObjectsWithMessagePackFormatter), StandardResolver.Instance);
      var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

      var bytes = MessagePackSerializer.Serialize(obj, options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize(obj.GetType(), bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(obj);
   }

   [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
   public partial class ComplexValueObjectWithTwoPropertiesAndKeys
   {
      [MessagePack.Key(0)]
      public decimal Property1 { get; }

      [MessagePack.Key(1)]
      public string Property2 { get; }
   }

   [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
   public partial class ComplexValueObjectWith3PropertiesAndKeys
   {
      [MessagePack.Key(0)]
      public decimal Property1 { get; }

      [MessagePack.Key(1)]
      public string Property2 { get; }

      [MessagePack.Key(2)]
      public int Property3 { get; }
   }

   [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
   public partial class ComplexValueObjectWithKeysAndGaps
   {
      [MessagePack.Key(2)]
      public decimal Property1 { get; }

      [MessagePack.Key(5)]
      public string Property2 { get; }

      [MessagePack.Key(8)]
      public int Property3 { get; }
   }

   [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
   public partial class ComplexValueObjectWithPartialKeys
   {
      public decimal Property1 { get; }

      [MessagePack.Key(5)]
      public string Property2 { get; }

      public int Property3 { get; }

      [MessagePack.Key(7)]
      public bool Property4 { get; }
   }
}
