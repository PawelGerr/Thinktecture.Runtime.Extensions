using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

// ReSharper disable InconsistentNaming
public class ComparisonOperators
{
   [Fact]
   public void Should_compare_value_objects()
   {
      var obj_1 = DecimalBasedClassValueObject.Create(1);
      var obj_2 = DecimalBasedClassValueObject.Create(2);

      // ReSharper disable EqualExpressionComparison
      (obj_1 < obj_1).Should().BeFalse();
      (obj_1 <= obj_1).Should().BeTrue();
      (obj_1 > obj_1).Should().BeFalse();
      (obj_1 >= obj_1).Should().BeTrue();
      // ReSharper restore EqualExpressionComparison

      (obj_1 < obj_2).Should().BeTrue();
      (obj_1 <= obj_2).Should().BeTrue();
      (obj_1 > obj_2).Should().BeFalse();
      (obj_1 >= obj_2).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_value_object_with_key_type()
   {
      var obj_1 = DecimalBasedClassValueObject.Create(1);
      var obj_1_key = 1m;
      var obj_2 = DecimalBasedClassValueObject.Create(2);
      var obj_2_key = 2m;

      (obj_1 < obj_1_key).Should().BeFalse();
      (obj_1_key < obj_1).Should().BeFalse();

      (obj_1 <= obj_1_key).Should().BeTrue();
      (obj_1_key <= obj_1).Should().BeTrue();

      (obj_1 > obj_1_key).Should().BeFalse();
      (obj_1_key > obj_1).Should().BeFalse();

      (obj_1 >= obj_1_key).Should().BeTrue();
      (obj_1_key >= obj_1).Should().BeTrue();

      (obj_1_key < obj_2).Should().BeTrue();
      (obj_1 < obj_2_key).Should().BeTrue();

      (obj_1_key <= obj_2).Should().BeTrue();
      (obj_1 <= obj_2_key).Should().BeTrue();

      (obj_1_key > obj_2).Should().BeFalse();
      (obj_1 > obj_2_key).Should().BeFalse();

      (obj_1_key >= obj_2).Should().BeFalse();
      (obj_1 >= obj_2_key).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_value_objects_having_custom_factory_name()
   {
      var obj_1 = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);
      var obj_2 = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);

      // ReSharper disable EqualExpressionComparison
      (obj_1 < obj_1).Should().BeFalse();
      (obj_1 <= obj_1).Should().BeTrue();
      (obj_1 > obj_1).Should().BeFalse();
      (obj_1 >= obj_1).Should().BeTrue();
      // ReSharper restore EqualExpressionComparison

      (obj_1 < obj_2).Should().BeTrue();
      (obj_1 <= obj_2).Should().BeTrue();
      (obj_1 > obj_2).Should().BeFalse();
      (obj_1 >= obj_2).Should().BeFalse();
   }

   [Fact]
   public void Should_compare_value_object_with_key_type_having_custom_factory_name()
   {
      var obj_1 = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);
      var obj_1_key = 1;
      var obj_2 = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(2);
      var obj_2_key = 2;

      (obj_1 < obj_1_key).Should().BeFalse();
      (obj_1_key < obj_1).Should().BeFalse();

      (obj_1 <= obj_1_key).Should().BeTrue();
      (obj_1_key <= obj_1).Should().BeTrue();

      (obj_1 > obj_1_key).Should().BeFalse();
      (obj_1_key > obj_1).Should().BeFalse();

      (obj_1 >= obj_1_key).Should().BeTrue();
      (obj_1_key >= obj_1).Should().BeTrue();

      (obj_1_key < obj_2).Should().BeTrue();
      (obj_1 < obj_2_key).Should().BeTrue();

      (obj_1_key <= obj_2).Should().BeTrue();
      (obj_1 <= obj_2_key).Should().BeTrue();

      (obj_1_key > obj_2).Should().BeFalse();
      (obj_1 > obj_2_key).Should().BeFalse();

      (obj_1_key >= obj_2).Should().BeFalse();
      (obj_1 >= obj_2_key).Should().BeFalse();
   }
}
