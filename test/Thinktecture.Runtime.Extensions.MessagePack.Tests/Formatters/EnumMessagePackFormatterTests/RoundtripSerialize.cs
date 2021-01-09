using System.Threading;
using FluentAssertions;
using MessagePack;
using MessagePack.Resolvers;
using Thinktecture.Formatters.EnumMessagePackFormatterTests.TestClasses;
using Xunit;

namespace Thinktecture.Formatters.EnumMessagePackFormatterTests
{
   public class Serialize
   {
      private readonly MessagePackSerializerOptions _options;

      public Serialize()
      {
         var resolver = CompositeResolver.Create(ValueTypeMessageFormatterResolver.Instance, StandardResolver.Instance);
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
         var bytes = MessagePackSerializer.Serialize(StringBasedEnum.ValueA, _options, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<StringBasedEnum>(bytes, _options, CancellationToken.None);

         value.Should().Be(StringBasedEnum.ValueA);
      }

      [Fact]
      public void Should_roundtrip_serialize_int_based_enum_providing_resolver()
      {
         var bytes = MessagePackSerializer.Serialize(IntBasedEnum.Value1, _options, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<IntBasedEnum>(bytes, _options, CancellationToken.None);

         value.Should().Be(IntBasedEnum.Value1);
      }

      [Fact]
      public void Should_roundtrip_serialize_class_with_string_based_enum_providing_resolver()
      {
         var instance = new ClassWithStringBasedEnum(StringBasedEnum.ValueA);

         var bytes = MessagePackSerializer.Serialize(instance, _options, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<ClassWithStringBasedEnum>(bytes, _options, CancellationToken.None);

         value.Should().BeEquivalentTo(instance);
      }

      [Fact]
      public void Should_roundtrip_serialize_class_with_int_based_enum_providing_resolver()
      {
         var instance = new ClassWithIntBasedEnum(IntBasedEnum.Value1);

         var bytes = MessagePackSerializer.Serialize(instance, _options, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<ClassWithIntBasedEnum>(bytes, _options, CancellationToken.None);

         value.Should().BeEquivalentTo(instance);
      }
   }
}
