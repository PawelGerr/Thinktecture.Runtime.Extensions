using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable SuspiciousTypeConversion.Global
public class Equals
{
   [Fact]
   public void Should_return_false_if_item_is_null()
   {
      SmartEnum_StringBased.Item1.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_item_is_of_different_type()
   {
      SmartEnum_StringBased.Item1.Equals(SmartEnum_CaseSensitive.LowerCased).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_on_reference_equality()
   {
      SmartEnum_StringBased.Item1.Equals(SmartEnum_StringBased.Item1).Should().BeTrue();
   }

   [Fact]
   public void Should_compare_keyless_smart_enum_via_reference_equality()
   {
      SmartEnum_Keyless.Item1.Equals(SmartEnum_Keyless.Item1).Should().BeTrue();
      SmartEnum_Keyless.Item2.Equals(SmartEnum_Keyless.Item2).Should().BeTrue();
      SmartEnum_Keyless.Item1.Equals(SmartEnum_Keyless.Item2).Should().BeFalse();
   }
}
