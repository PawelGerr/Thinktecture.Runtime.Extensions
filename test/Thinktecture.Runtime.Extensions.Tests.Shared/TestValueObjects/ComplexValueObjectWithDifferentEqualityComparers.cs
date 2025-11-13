namespace Thinktecture.Runtime.Tests.TestValueObjects;

[ComplexValueObject]
public partial class ComplexValueObjectWithDifferentEqualityComparers
{
   [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
   public string Property1 { get; }

   [MemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
   public string Property2 { get; }

   [MemberEqualityComparer<ComparerAccessors.Default<int>, int>]
   public int Property3 { get; }
}
