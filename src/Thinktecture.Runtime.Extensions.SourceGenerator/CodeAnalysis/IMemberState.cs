namespace Thinktecture.CodeAnalysis;

public interface IMemberState : IEquatable<IMemberState>, IMemberInformation
{
   string ArgumentName { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   string TypeFullyQualifiedWithNullability { get; }
   bool IsFormattable { get; }
   bool IsComparable { get; }
   bool IsParsable { get; }
   ImplementedComparisonOperators ComparisonOperators { get; }
}
