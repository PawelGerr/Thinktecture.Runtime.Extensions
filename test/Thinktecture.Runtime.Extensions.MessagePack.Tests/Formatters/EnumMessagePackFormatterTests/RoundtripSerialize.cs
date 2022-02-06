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
                                                                new object[] { IntegerEnum.Item1 },
                                                                new object[] { DifferentAssemblyExtendedTestEnum.Item1 },
                                                                new object[] { ExtendedTestEnum.Item1 },
                                                                new object[] { ExtensibleTestEnum.Item1 },
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
}
