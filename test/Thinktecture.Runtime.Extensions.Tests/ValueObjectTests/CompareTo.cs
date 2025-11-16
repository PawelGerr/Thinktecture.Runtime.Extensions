using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class CompareTo
{
   [Fact]
   public void Should_return_0_if_items_are_equal()
   {
      DecimalBasedClassValueObject.Create(1).CompareTo(DecimalBasedClassValueObject.Create(1)).Should().Be(0);
      DecimalBasedClassValueObject.Create(1).CompareTo((object)DecimalBasedClassValueObject.Create(1)).Should().Be(0);
   }

   [Fact]
   public void Should_return_1_if_items_is_bigger_than_other_item()
   {
      DecimalBasedClassValueObject.Create(2).CompareTo(DecimalBasedClassValueObject.Create(1)).Should().Be(1);
      DecimalBasedClassValueObject.Create(2).CompareTo((object)DecimalBasedClassValueObject.Create(1)).Should().Be(1);
   }

   [Fact]
   public void Should_return_minus_1_if_items_is_smaller_than_other_item()
   {
      DecimalBasedClassValueObject.Create(1).CompareTo(DecimalBasedClassValueObject.Create(2)).Should().Be(-1);
      DecimalBasedClassValueObject.Create(1).CompareTo((object)DecimalBasedClassValueObject.Create(2)).Should().Be(-1);
   }

   [Fact]
   public void Should_support_CompareTo_for_generic_int_based_value_objects()
   {
      var obj1 = ValueObject_Generic_IntBased<string>.Create(42);
      var obj2 = ValueObject_Generic_IntBased<string>.Create(43);
      var obj3 = ValueObject_Generic_IntBased<string>.Create(42);

      obj1.CompareTo(obj2).Should().BeLessThan(0);
      obj2.CompareTo(obj1).Should().BeGreaterThan(0);
      obj1.CompareTo(obj3).Should().Be(0);
   }

   [Fact]
   public void Should_support_CompareTo_for_generic_string_based_value_objects()
   {
      var obj1 = ValueObject_Generic_StringBased<object>.Create("a");
      var obj2 = ValueObject_Generic_StringBased<object>.Create("b");
      var obj3 = ValueObject_Generic_StringBased<object>.Create("a");

      obj1.CompareTo(obj2).Should().BeLessThan(0);
      obj2.CompareTo(obj1).Should().BeGreaterThan(0);
      obj1.CompareTo(obj3).Should().Be(0);
   }
}
