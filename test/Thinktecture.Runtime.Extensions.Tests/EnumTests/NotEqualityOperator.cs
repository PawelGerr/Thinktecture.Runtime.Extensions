using Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable EqualExpressionComparison

namespace Thinktecture.Runtime.Tests.EnumTests;

public class NotEqualityOperator
{
   [Fact]
   public void Should_return_true_if_item_is_null()
   {
      (SmartEnum_StringBased.Item1 is not null).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_item_is_of_different_type()
   {
      // ReSharper disable once SuspiciousTypeConversion.Global
      (SmartEnum_StringBased.Item1 != SmartEnum_CaseSensitive.LowerCased).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_on_reference_equality()
   {
      // ReSharper disable once EqualExpressionComparison
      (SmartEnum_StringBased.Item1 != SmartEnum_StringBased.Item1).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_keyless_smart_enum_via_reference_equality()
   {
      (SmartEnum_Keyless.Item1 != SmartEnum_Keyless.Item1).Should().BeFalse();
      (SmartEnum_Keyless.Item2 != SmartEnum_Keyless.Item2).Should().BeFalse();
      (SmartEnum_Keyless.Item1 != SmartEnum_Keyless.Item2).Should().BeTrue();
   }
}
