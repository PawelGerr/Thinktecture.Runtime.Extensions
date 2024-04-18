using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using MessagePack;
using MessagePack.Resolvers;
using Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests;

public class Serialize
{
   private static readonly MethodInfo _serializeRoundTripMethodInfo = typeof(Serialize).GetMethod(nameof(RoundTripSerializeWithCustomOptions), BindingFlags.Instance | BindingFlags.NonPublic)
                                                                      ?? throw new Exception($"Method '{nameof(RoundTripSerializeWithCustomOptions)}' not found.");

   private readonly MessagePackSerializerOptions _options;

   public Serialize()
   {
      var resolver = CompositeResolver.Create(ValueObjectMessageFormatterResolver.Instance, StandardResolver.Instance);
      _options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
   }

   [Fact]
   public void Should_deserialize_enum_if_null_and_default()
   {
      RoundTripSerializeWithCustomOptions<TestSmartEnum_Class_IntBased>(null);
      RoundTripSerializeWithCustomOptions<TestSmartEnum_Class_StringBased>(null);
      RoundTripSerializeWithCustomOptions<TestSmartEnum_Struct_IntBased?>(null);
      RoundTripSerializeWithCustomOptions<TestSmartEnum_Struct_StringBased?>(null);
      RoundTripSerializeWithCustomOptions<TestSmartEnum_Struct_IntBased>(default);
      RoundTripSerializeWithCustomOptions<TestSmartEnum_Struct_StringBased>(default);
   }

   [Fact]
   public void Should_deserialize_keyed_value_object_when_null_and_default_unless_enum_and_underlying_are_both_null()
   {
      RoundTripSerializeWithCustomOptions<IntBasedReferenceValueObject>(null);
      RoundTripSerializeWithCustomOptions<StringBasedReferenceValueObject>(null);
      RoundTripSerializeWithCustomOptions<IntBasedStructValueObject?>(null);
      RoundTripSerializeWithCustomOptions<StringBasedStructValueObject?>(null);
      RoundTripSerializeWithCustomOptions<IntBasedStructValueObject>(default);
      RoundTripSerializeWithCustomOptions<StringBasedStructValueObject>(default);
   }

   [Fact]
   public void Should_deserialize_value_object_if_null_and_default()
   {
      RoundTripSerializeWithCustomOptions<TestValueObject_Complex_Class_WithFormatter>(null);
      RoundTripSerializeWithCustomOptions<TestValueObject_Complex_Struct_WithFormatter?>(null);
      RoundTripSerializeWithCustomOptions<TestValueObject_Complex_Struct_WithFormatter>(default);
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

   public static IEnumerable<object[]> DataForValueObject => new[]
   {
      new object[] { new ClassWithIntBasedEnum(IntegerEnum.Item1) },
      new object[] { new ClassWithStringBasedEnum(TestEnum.Item1) },
      new object[] { TestEnum.Item1 },
      new object[] { IntegerEnum.Item1 }
   };

   [Theory]
   [MemberData(nameof(DataForValueObject))]
   public void Should_roundtrip_serialize(object value)
   {
      _serializeRoundTripMethodInfo.MakeGenericMethod(value.GetType()).Invoke(this, new[] { value });
   }

   private void RoundTripSerializeWithCustomOptions<T>(T value)
   {
      var bytes = MessagePackSerializer.Serialize(value, _options, CancellationToken.None);
      var deserializedValue = MessagePackSerializer.Deserialize<T>(bytes, _options, CancellationToken.None);

      deserializedValue.Should().Be(value);
   }

   public static IEnumerable<object[]> DataForValueObjectWithMultipleProperties => new[]
   {
      new object[] { null },
      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!) },
      new object[] { ValueObjectWithMultipleProperties.Create(0, null, null!) },
      new object[] { ValueObjectWithMultipleProperties.Create(0, 0, String.Empty) },
      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value") },
      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value") },
      new object[] { ValueObjectWithMultipleProperties.Create(1, 42, "Value") }
   };

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
   public void Should_roundtrip_serialize_using_factory_specified_by_ValueObjectFactoryAttribute(bool withCustomResolver)
   {
      var options = withCustomResolver ? _options : StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(BoundaryWithFactories.Create(1, 2), options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<BoundaryWithFactories>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(BoundaryWithFactories.Create(1, 2));
   }

   [Fact]
   public void Should_roundtrip_serialize_enum_with_ValueObjectValidationErrorAttribute()
   {
      var options = StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(TestEnumWithCustomError.Item1, options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<TestEnumWithCustomError>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(TestEnumWithCustomError.Item1);
   }

   [Fact]
   public void Should_deserialize_simple_value_object_with_ValueObjectValidationErrorAttribute()
   {
      var options = StandardResolver.Options;

      var bytes = MessagePackSerializer.Serialize(StringBasedReferenceValueObjectWithCustomError.Create("value"), options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<StringBasedReferenceValueObjectWithCustomError>(bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(StringBasedReferenceValueObjectWithCustomError.Create("value"));
   }

   [Fact]
   public void Should_deserialize_complex_value_object_with_ValueObjectValidationErrorAttribute()
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

   public static IEnumerable<object[]> ObjectWithStructTestData =
   [
      [new TestClass<IntBasedStructValueObject>(IntBasedStructValueObject.Create(42))],
      [new TestClass<IntBasedStructValueObject?>(IntBasedStructValueObject.Create(42))],
      [new TestClass<IntBasedReferenceValueObject>(IntBasedReferenceValueObject.Create(42))],
      [new TestStruct<IntBasedStructValueObject>(IntBasedStructValueObject.Create(42))],
      [new TestStruct<IntBasedStructValueObject?>(IntBasedStructValueObject.Create(42))],
      [new TestStruct<IntBasedReferenceValueObject>(IntBasedReferenceValueObject.Create(42))],
   ];

   [Theory]
   [MemberData(nameof(ObjectWithStructTestData))]
   public void Should_roundtrip_serialize_types_with_struct_properties_using_resolver(object obj)
   {
      Roundtrip_serialize_types_with_struct_properties_using_resolver(true, obj);
      Roundtrip_serialize_types_with_struct_properties_using_resolver(false, obj);
   }

   private static void Roundtrip_serialize_types_with_struct_properties_using_resolver(
      bool skipValueObjectsWithMessagePackFormatter,
      object obj)
   {
      var resolver = CompositeResolver.Create(new ValueObjectMessageFormatterResolver(skipValueObjectsWithMessagePackFormatter), StandardResolver.Instance);
      var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

      var bytes = MessagePackSerializer.Serialize(obj, options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize(obj.GetType(), bytes, options, CancellationToken.None);

      value.Should().BeEquivalentTo(obj);
   }

   [MessagePackObject]
   public struct TestClass<T>
   {
      [Key(0)]
      public T Prop { get; set; }

      public TestClass(T prop)
      {
         Prop = prop;
      }
   }

   [MessagePackObject]
   public struct TestStruct<T>
   {
      [Key(0)]
      public T Prop { get; set; }

      public TestStruct(T prop)
      {
         Prop = prop;
      }
   }
}
