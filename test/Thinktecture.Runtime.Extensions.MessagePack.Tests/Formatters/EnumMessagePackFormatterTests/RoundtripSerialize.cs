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
         var option = StandardResolver.Options.WithResolver(EnumMessageFormatterResolver.Instance);

         var bytes = MessagePackSerializer.Serialize(StringBasedEnum.ValueA, option, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<StringBasedEnum>(bytes, option, CancellationToken.None);

         value.Should().Be(StringBasedEnum.ValueA);
      }

      [Fact]
      public void Should_roundtrip_serialize_int_based_enum_providing_resolver()
      {
         var option = StandardResolver.Options.WithResolver(EnumMessageFormatterResolver.Instance);

         var bytes = MessagePackSerializer.Serialize(IntBasedEnum.Value1, option, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<IntBasedEnum>(bytes, option, CancellationToken.None);

         value.Should().Be(IntBasedEnum.Value1);
      }

      [Fact]
      public void Should_roundtrip_serialize_class_with_string_based_enum_providing_resolver()
      {
         var option = StandardResolver.Options.WithResolver(EnumMessageFormatterResolver.Instance);

         var instance = new ClassWithStringBasedEnum(StringBasedEnum.ValueA);

         var bytes = MessagePackSerializer.Serialize(instance, option, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<ClassWithStringBasedEnum>(bytes, option, CancellationToken.None);

         value.Should().BeEquivalentTo(instance);
      }

      [Fact]
      public void Should_roundtrip_serialize_class_with_int_based_enum_providing_resolver()
      {
         var option = StandardResolver.Options.WithResolver(EnumMessageFormatterResolver.Instance);

         var instance = new ClassWithIntBasedEnum(IntBasedEnum.Value1);

         var bytes = MessagePackSerializer.Serialize(instance, option, CancellationToken.None);
         var value = MessagePackSerializer.Deserialize<ClassWithIntBasedEnum>(bytes, option, CancellationToken.None);

         value.Should().BeEquivalentTo(instance);
      }
   }
}
