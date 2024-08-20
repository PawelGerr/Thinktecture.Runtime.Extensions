namespace Thinktecture.ValueObjects;

[ComplexValueObject(SkipToString = true)]
public partial class ComplexValueObjectWithCustomEqualityComparison
{
   [ValueObjectMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
   public string Identifier { get; }

   public string Name { get; } // unused in comparisons

   public override string ToString()
   {
      return Name;
   }
}
