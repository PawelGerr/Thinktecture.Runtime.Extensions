using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class CompareTo
{
   [Fact]
   public void Should_return_0_if_items_are_equal()
   {
      DecimalBasedClassValueObject.Create(1).CompareTo(DecimalBasedClassValueObject.Create(1)).Should().Be(0);
   }

   [Fact]
   public void Should_return_1_if_items_is_bigger_than_other_item()
   {
      DecimalBasedClassValueObject.Create(2).CompareTo(DecimalBasedClassValueObject.Create(1)).Should().Be(1);
   }

   [Fact]
   public void Should_return_minus_1_if_items_is_smaller_than_other_item()
   {
      DecimalBasedClassValueObject.Create(1).CompareTo(DecimalBasedClassValueObject.Create(2)).Should().Be(-1);
   }
}
