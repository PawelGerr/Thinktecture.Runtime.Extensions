using Thinktecture.Collections;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Collections;

public class ValueObjectComparerTests
{
   [Fact]
   public void Should_compare_enums()
   {
      StringValueObjectComparer<TestEnum>.Ordinal.Equals(TestEnum.Item1, TestEnum.Item1).Should().BeTrue();
      StringValueObjectComparer<ValidatableTestEnumCaseSensitive>.Ordinal.Equals(ValidatableTestEnumCaseSensitive.LowerCased, ValidatableTestEnumCaseSensitive.LowerCased).Should().BeTrue();
      StringValueObjectComparer<ValidatableTestEnumCaseSensitive>.Ordinal.Equals(ValidatableTestEnumCaseSensitive.Get("Item"), ValidatableTestEnumCaseSensitive.Get("Item")).Should().BeTrue();
      StringValueObjectComparer<ValidatableTestEnumCaseSensitive>.Ordinal.Equals(ValidatableTestEnumCaseSensitive.Get("Item"), ValidatableTestEnumCaseSensitive.Get("item")).Should().BeFalse();

      StringValueObjectComparer<TestEnum>.OrdinalIgnoreCase.Equals(TestEnum.Item1, TestEnum.Item1).Should().BeTrue();
      StringValueObjectComparer<ValidatableTestEnumCaseSensitive>.OrdinalIgnoreCase.Equals(ValidatableTestEnumCaseSensitive.LowerCased, ValidatableTestEnumCaseSensitive.LowerCased).Should().BeTrue();
      StringValueObjectComparer<ValidatableTestEnumCaseSensitive>.OrdinalIgnoreCase.Equals(ValidatableTestEnumCaseSensitive.Get("Item"), ValidatableTestEnumCaseSensitive.Get("Item")).Should().BeTrue();
      StringValueObjectComparer<ValidatableTestEnumCaseSensitive>.OrdinalIgnoreCase.Equals(ValidatableTestEnumCaseSensitive.Get("Item"), ValidatableTestEnumCaseSensitive.Get("item")).Should().BeTrue();
   }

   [Fact]
   public void Should_compare_value_objects()
   {
      StringValueObjectComparer<TestValueObjectCaseInsensitive>.Ordinal.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("Item")).Should().BeTrue();
      StringValueObjectComparer<TestValueObjectCaseInsensitive>.Ordinal.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("item")).Should().BeFalse();
      StringValueObjectComparer<TestValueObjectCaseSensitive>.Ordinal.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("Item")).Should().BeTrue();
      StringValueObjectComparer<TestValueObjectCaseSensitive>.Ordinal.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("item")).Should().BeFalse();

      StringValueObjectComparer<TestValueObjectCaseInsensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("Item")).Should().BeTrue();
      StringValueObjectComparer<TestValueObjectCaseInsensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("item")).Should().BeTrue();
      StringValueObjectComparer<TestValueObjectCaseSensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("Item")).Should().BeTrue();
      StringValueObjectComparer<TestValueObjectCaseSensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("item")).Should().BeTrue();
   }
}
