using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTests
{
   public class ImplicitConversionToKey
   {
      [Fact]
      public void Should_return_null_if_item_is_null()
      {
         string key = (TestEnum)null;

         key.Should().BeNull();
      }

      [Fact]
      public void Should_return_default_if_struct_is_default()
      {
         StructIntegerEnum item = default;
         int key = item;

         key.Should().Be(0);
      }

      [Fact]
      public void Should_return_key()
      {
         string key = TestEnum.Item1;
         key.Should().Be(TestEnum.Item1.Key);

         key = ExtensibleTestEnum.Item1;
         key.Should().Be(ExtensibleTestEnum.Item1.Key);

         key = ExtendedTestEnum.Item1;
         key.Should().Be(ExtendedTestEnum.Item1.Key);

         key = ExtendedTestEnum.Item2;
         key.Should().Be(ExtendedTestEnum.Item2.Key);

         key = DifferentAssemblyExtendedTestEnum.Item1;
         key.Should().Be(DifferentAssemblyExtendedTestEnum.Item1.Key);

         key = DifferentAssemblyExtendedTestEnum.Item2;
         key.Should().Be(DifferentAssemblyExtendedTestEnum.Item2.Key);
      }
   }
}
