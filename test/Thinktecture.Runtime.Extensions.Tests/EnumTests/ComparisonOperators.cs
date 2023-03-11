using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable InconsistentNaming
public class ComparisonOperators
{
   [Fact]
   public void Should_compare_enums()
   {
      var item_1 = IntegerEnum.Item1;
      var item_2 = IntegerEnum.Item2;

      // ReSharper disable EqualExpressionComparison
      (item_1 < item_1).Should().BeFalse();
      (item_1 <= item_1).Should().BeTrue();
      (item_1 > item_1).Should().BeFalse();
      (item_1 >= item_1).Should().BeTrue();
      // ReSharper restore EqualExpressionComparison

      (item_1 < item_2).Should().BeTrue();
      (item_1 <= item_2).Should().BeTrue();
      (item_1 > item_2).Should().BeFalse();
      (item_1 >= item_2).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_enum_with_underlying_type()
   {
      var item_1 = IntegerEnum.Item1;
      var item_1_key = IntegerEnum.Item1.Key;
      var item_2 = IntegerEnum.Item2;
      var item_2_key = IntegerEnum.Item2.Key;

      (item_1 < item_1_key).Should().BeFalse();
      (item_1_key < item_1).Should().BeFalse();

      (item_1 <= item_1_key).Should().BeTrue();
      (item_1_key <= item_1).Should().BeTrue();

      (item_1 > item_1_key).Should().BeFalse();
      (item_1_key > item_1).Should().BeFalse();

      (item_1 >= item_1_key).Should().BeTrue();
      (item_1_key >= item_1).Should().BeTrue();

      (item_1_key < item_2).Should().BeTrue();
      (item_1 < item_2_key).Should().BeTrue();

      (item_1_key <= item_2).Should().BeTrue();
      (item_1 <= item_2_key).Should().BeTrue();

      (item_1_key > item_2).Should().BeFalse();
      (item_1 > item_2_key).Should().BeFalse();

      (item_1_key >= item_2).Should().BeFalse();
      (item_1 >= item_2_key).Should().BeFalse();
   }
}
