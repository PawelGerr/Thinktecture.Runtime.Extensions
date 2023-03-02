using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class ComparisonOperators
{
   [Fact]
   public void Should_compare()
   {
      var obj_1 = DecimalBasedClassValueObject.Create(1);
      var obj_2 = DecimalBasedClassValueObject.Create(2);

      (obj_1 < obj_1).Should().BeFalse();
      (obj_1 <= obj_1).Should().BeTrue();
      (obj_1 > obj_1).Should().BeFalse();
      (obj_1 >= obj_1).Should().BeTrue();

      (obj_1 < obj_2).Should().BeTrue();
      (obj_1 <= obj_2).Should().BeTrue();
      (obj_1 > obj_2).Should().BeFalse();
      (obj_1 >= obj_2).Should().BeFalse();
   }

}
