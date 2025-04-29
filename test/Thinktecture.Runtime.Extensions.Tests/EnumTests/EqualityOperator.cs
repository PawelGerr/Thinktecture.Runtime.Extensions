using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable EqualExpressionComparison
// ReSharper disable ConditionIsAlwaysTrueOrFalse
public class EqualityOperator
{
   [Fact]
   public void Should_return_false_if_item_is_null()
   {
      (SmartEnum_StringBased.Item1 == null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_item_is_of_different_type()
   {
      (SmartEnum_StringBased.Item1 == SmartEnum_CaseSensitive.LowerCased).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_on_reference_equality()
   {
      (SmartEnum_StringBased.Item1 == SmartEnum_StringBased.Item1).Should().BeTrue();
   }

   [Fact]
   public void Should_compare_keyless_smart_enum_via_reference_equality()
   {
      (SmartEnum_Keyless.Item1 == SmartEnum_Keyless.Item1).Should().BeTrue();
      (SmartEnum_Keyless.Item2 == SmartEnum_Keyless.Item2).Should().BeTrue();
      (SmartEnum_Keyless.Item1 == SmartEnum_Keyless.Item2).Should().BeFalse();
   }
}
