using System;
using System.Collections.Generic;
using FluentAssertions;
using MessagePack;
using MessagePack.Resolvers;
using Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.Formatters.EnumMessagePackFormatterTests;

public class Serialize
{
   private readonly MessagePackSerializerOptions _options;

   public Serialize()
   {
      var resolver = CompositeResolver.Create(ValueObjectMessageFormatterResolver.Instance, StandardResolver.Instance);
      _options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
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

   [Fact]
   public void Should_roundtrip_serialize_string_based_enum_providing_resolver()
   {
      var bytes = MessagePackSerializer.Serialize(TestEnum.Item1, _options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<TestEnum>(bytes, _options, CancellationToken.None);

      value.Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_roundtrip_serialize_ExtensibleTestEnum()
   {
      var bytes = MessagePackSerializer.Serialize(ExtensibleTestEnum.Item1, _options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<ExtensibleTestEnum>(bytes, _options, CancellationToken.None);

      value.Should().Be(ExtensibleTestEnum.Item1);
   }

   [Fact]
   public void Should_roundtrip_serialize_ExtendedTestEnum()
   {
      var bytes = MessagePackSerializer.Serialize(ExtendedTestEnum.Item1, _options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<ExtendedTestEnum>(bytes, _options, CancellationToken.None);

      value.Should().Be(ExtendedTestEnum.Item1);
   }

   [Fact]
   public void Should_roundtrip_serialize_DifferentAssemblyExtendedTestEnum()
   {
      var bytes = MessagePackSerializer.Serialize(DifferentAssemblyExtendedTestEnum.Item1, _options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<DifferentAssemblyExtendedTestEnum>(bytes, _options, CancellationToken.None);

      value.Should().Be(DifferentAssemblyExtendedTestEnum.Item1);
   }

   [Fact]
   public void Should_roundtrip_serialize_int_based_enum_providing_resolver()
   {
      var bytes = MessagePackSerializer.Serialize(IntegerEnum.Item1, _options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<IntegerEnum>(bytes, _options, CancellationToken.None);

      value.Should().Be(IntegerEnum.Item1);
   }

   [Fact]
   public void Should_roundtrip_serialize_class_with_string_based_enum_providing_resolver()
   {
      var instance = new ClassWithStringBasedEnum(TestEnum.Item1);

      var bytes = MessagePackSerializer.Serialize(instance, _options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<ClassWithStringBasedEnum>(bytes, _options, CancellationToken.None);

      value.Should().BeEquivalentTo(instance);
   }

   [Fact]
   public void Should_roundtrip_serialize_class_with_int_based_enum_providing_resolver()
   {
      var instance = new ClassWithIntBasedEnum(IntegerEnum.Item1);

      var bytes = MessagePackSerializer.Serialize(instance, _options, CancellationToken.None);
      var value = MessagePackSerializer.Deserialize<ClassWithIntBasedEnum>(bytes, _options, CancellationToken.None);

      value.Should().BeEquivalentTo(instance);
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