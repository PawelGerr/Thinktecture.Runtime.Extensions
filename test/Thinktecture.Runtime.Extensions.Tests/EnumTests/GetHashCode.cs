using System;
using System.Runtime.CompilerServices;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_hashcode_of_the_type_plus_key()
   {
      var expected = StringComparer.OrdinalIgnoreCase.GetHashCode(SmartEnum_StringBased.Item1.Key);

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
      var expected = StringComparer.Ordinal.GetHashCode(SmartEnum_CaseSensitive.LowerCased.Key);

      SmartEnum_CaseSensitive.LowerCased.GetHashCode().Should().Be(expected);
   }

   [Fact]
   public void Should_have_consistent_hashcode_for_generic_keyless_enum()
   {
      var item1 = SmartEnum_Generic_Keyless<string>.Item1;
      var item1Again = SmartEnum_Generic_Keyless<string>.Item1;

      item1.GetHashCode().Should().Be(item1Again.GetHashCode());
   }
}
