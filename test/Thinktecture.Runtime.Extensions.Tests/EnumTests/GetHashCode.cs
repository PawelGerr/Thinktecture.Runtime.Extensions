using System;
using System.Runtime.CompilerServices;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key()
   {
      var expected = HashCode.Combine(typeof(SmartEnum_StringBased), StringComparer.OrdinalIgnoreCase.GetHashCode(SmartEnum_StringBased.Item1.Key));

      SmartEnum_StringBased.Item1.GetHashCode().Should().Be(expected);
   }

   [Fact]
   public void Should_return_hashcode_of_instance_of_keyless_smart_enum()
   {
      var expected = RuntimeHelpers.GetHashCode(SmartEnum_Keyless.Item1);

      SmartEnum_Keyless.Item1.GetHashCode().Should().Be(expected);
   }

   [Fact]
   public void Should_return_hashcode_of_case_sensitive_enum()
   {
      var expected = HashCode.Combine(typeof(SmartEnum_CaseSensitive), StringComparer.Ordinal.GetHashCode(SmartEnum_CaseSensitive.LowerCased.Key));

      SmartEnum_CaseSensitive.LowerCased.GetHashCode().Should().Be(expected);
   }
}
