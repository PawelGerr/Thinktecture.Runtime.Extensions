using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface IMemberState : IEquatable<IMemberState>
{
   string Name { get; }
   string ArgumentName { get; }
   SpecialType SpecialType { get; }
   string TypeFullyQualified { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   string TypeFullyQualifiedWithNullability { get; }
   bool IsReferenceType { get; }
   bool IsFormattable { get; }
   bool IsComparable { get; }
   bool IsParsable { get; }
   bool IsNullableStruct { get; }
   NullableAnnotation NullableAnnotation { get; }
   bool HasComparisonOperators { get; }
   string TypeMinimallyQualified { get; }
}
