using System;
using System.Runtime.CompilerServices;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key()
   {
      var expected = HashCode.Combine(typeof(TestEnum), StringComparer.OrdinalIgnoreCase.GetHashCode(TestEnum.Item1.Key));

      TestEnum.Item1.GetHashCode().Should().Be(expected);
   }

   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key_for_structs()
   {
      var expected = HashCode.Combine(typeof(StructIntegerEnum), StructIntegerEnum.Item1.Key.GetHashCode());

      StructIntegerEnum.Item1.GetHashCode().Should().Be(expected);
   }

   [Fact]
   public void Should_return_hashcode_of_instance_of_keyless_smart_enum()
   {
      var expected = RuntimeHelpers.GetHashCode(KeylessTestEnum.Item1);

      KeylessTestEnum.Item1.GetHashCode().Should().Be(expected);
   }

   [Fact]
   public void Should_return_hashcode_of_case_sensitive_enum()
   {
      var expected = HashCode.Combine(typeof(ValidatableTestEnumCaseSensitive), StringComparer.Ordinal.GetHashCode(ValidatableTestEnumCaseSensitive.LowerCased.Key));

      ValidatableTestEnumCaseSensitive.LowerCased.GetHashCode().Should().Be(expected);
   }
}
