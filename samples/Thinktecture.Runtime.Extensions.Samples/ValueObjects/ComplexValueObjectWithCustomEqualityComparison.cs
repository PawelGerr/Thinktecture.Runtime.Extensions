using System;

namespace Thinktecture.ValueObjects;

[ComplexValueObject(SkipToString = true, DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
public partial class ComplexValueObjectWithCustomEqualityComparison
{
   [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
   public string Identifier { get; }

   public string Name { get; } // unused in comparisons

   public override string ToString()
   {
      return Name;
   }
}
