using Thinktecture.Collections;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Collections;

public class StringKeyedObjectComparerTests
{
   [Fact]
   public void Should_compare_smart_enums()
   {
      StringKeyedObjectComparer<SmartEnum_StringBased>.Ordinal.Equals(SmartEnum_StringBased.Item1, SmartEnum_StringBased.Item1).Should().BeTrue();
      StringKeyedObjectComparer<SmartEnum_CaseSensitive>.Ordinal.Equals(SmartEnum_CaseSensitive.LowerCased, SmartEnum_CaseSensitive.LowerCased).Should().BeTrue();
      StringKeyedObjectComparer<SmartEnum_CaseSensitive>.Ordinal.Equals(SmartEnum_CaseSensitive.LowerCased, SmartEnum_CaseSensitive.UpperCased).Should().BeFalse();

      StringKeyedObjectComparer<SmartEnum_StringBased>.OrdinalIgnoreCase.Equals(SmartEnum_StringBased.Item1, SmartEnum_StringBased.Item1).Should().BeTrue();
      StringKeyedObjectComparer<SmartEnum_CaseSensitive>.OrdinalIgnoreCase.Equals(SmartEnum_CaseSensitive.LowerCased, SmartEnum_CaseSensitive.LowerCased).Should().BeTrue();
      StringKeyedObjectComparer<SmartEnum_CaseSensitive>.OrdinalIgnoreCase.Equals(SmartEnum_CaseSensitive.LowerCased, SmartEnum_CaseSensitive.UpperCased).Should().BeTrue();
   }

   [Fact]
   public void Should_compare_value_objects()
   {
      StringKeyedObjectComparer<TestValueObjectCaseInsensitive>.Ordinal.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("Item")).Should().BeTrue();
      StringKeyedObjectComparer<TestValueObjectCaseInsensitive>.Ordinal.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("item")).Should().BeFalse();
      StringKeyedObjectComparer<TestValueObjectCaseSensitive>.Ordinal.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("Item")).Should().BeTrue();
      StringKeyedObjectComparer<TestValueObjectCaseSensitive>.Ordinal.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("item")).Should().BeFalse();

      StringKeyedObjectComparer<TestValueObjectCaseInsensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("Item")).Should().BeTrue();
      StringKeyedObjectComparer<TestValueObjectCaseInsensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseInsensitive.Create("Item"), TestValueObjectCaseInsensitive.Create("item")).Should().BeTrue();
      StringKeyedObjectComparer<TestValueObjectCaseSensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("Item")).Should().BeTrue();
      StringKeyedObjectComparer<TestValueObjectCaseSensitive>.OrdinalIgnoreCase.Equals(TestValueObjectCaseSensitive.Create("Item"), TestValueObjectCaseSensitive.Create("item")).Should().BeTrue();
   }
}
