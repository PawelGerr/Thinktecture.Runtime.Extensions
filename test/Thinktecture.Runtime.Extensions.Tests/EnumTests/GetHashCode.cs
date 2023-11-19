using System;
using System.Runtime.CompilerServices;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key()
   {
      var hashCode = TestEnum.Item1.GetHashCode();
      var keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(TestEnum.Item1.Key);
      hashCode.Should().Be(HashCode.Combine(typeof(TestEnum), keyHashCode));
   }

   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key_for_structs()
   {
      var hashCode = StructIntegerEnum.Item1.GetHashCode();

      var keyHashCode = StructIntegerEnum.Item1.Key.GetHashCode();
      hashCode.Should().Be(HashCode.Combine(typeof(StructIntegerEnum), keyHashCode));
   }

   [Fact]
   public void Should_return_hashcode_of_instance_of_keyless_smart_enum()
   {
      KeylessTestEnum.Item1.GetHashCode().Should().Be(RuntimeHelpers.GetHashCode(KeylessTestEnum.Item1));
   }
}
