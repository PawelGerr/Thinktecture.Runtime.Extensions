using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public interface ITypedMemberState : IEquatable<ITypedMemberState>
{
   string TypeFullyQualified { get; }
   string TypeFullyQualifiedNullable { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   string TypeFullyQualifiedWithNullability { get; }
   string TypeMinimallyQualified { get; }
   SpecialType SpecialType { get; }
   bool IsReferenceType { get; }
   bool IsReferenceTypeOrNullableStruct { get; }
   bool IsNullableStruct { get; }
   NullableAnnotation NullableAnnotation { get; }
   bool IsFormattable { get; }
   bool IsComparable { get; }
   bool IsParsable { get; }
   bool HasComparisonOperators { get; }
   bool HasAdditionOperators { get; }
   bool HasSubtractionOperators { get; }
   bool HasMultiplyOperators { get; }
   bool HasDivisionOperators { get; }
}