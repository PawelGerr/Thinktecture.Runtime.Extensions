using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ComparisonOperators
{
   [Fact]
   public void Should_compare()
   {
      var item_1 = IntegerEnum.Item1;
      var item_2 = IntegerEnum.Item2;

      (item_1 < item_1).Should().BeFalse();
      (item_1 <= item_1).Should().BeTrue();
      (item_1 > item_1).Should().BeFalse();
      (item_1 >= item_1).Should().BeTrue();

      (item_1 < item_2).Should().BeTrue();
      (item_1 <= item_2).Should().BeTrue();
      (item_1 > item_2).Should().BeFalse();
      (item_1 >= item_2).Should().BeFalse();
   }
}
