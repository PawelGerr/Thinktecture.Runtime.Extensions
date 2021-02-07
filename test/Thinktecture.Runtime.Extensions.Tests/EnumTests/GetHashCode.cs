using System;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTests
{
   public class GetHashCode
   {
      [Fact]
      public void Should_return_hashcode_of_the_type_plus_key()
      {
         var hashCode = TestEnum.Item1.GetHashCode();
         var typeHashCode = typeof(TestEnum).GetHashCode();
         var keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(TestEnum.Item1.Key);
         hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);

         hashCode = ExtensibleTestEnum.Item1.GetHashCode();
         typeHashCode = typeof(ExtensibleTestEnum).GetHashCode();
         keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(ExtensibleTestEnum.Item1.Key);
         hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);

         hashCode = ExtendedTestEnum.Item1.GetHashCode();
         typeHashCode = typeof(ExtendedTestEnum).GetHashCode();
         keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(ExtendedTestEnum.Item1.Key);
         hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);

         hashCode = DifferentAssemblyExtendedTestEnum.Item1.GetHashCode();
         typeHashCode = typeof(DifferentAssemblyExtendedTestEnum).GetHashCode();
         keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(DifferentAssemblyExtendedTestEnum.Item1.Key);
         hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);
      }

      [Fact]
      public void Should_return_hashcode_of_the_type_plus_key_for_structs()
      {
         var hashCode = StructIntegerEnum.Item1.GetHashCode();

         var typeHashCode = typeof(StructIntegerEnum).GetHashCode();
         var keyHashCode = StructIntegerEnum.Item1.Key.GetHashCode();
         hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);
      }
   }
}
