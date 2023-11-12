namespace Thinktecture.CodeAnalysis;

public interface ITypedMemberState : IEquatable<ITypedMemberState>
{
   string TypeFullyQualified { get; }
   string TypeFullyQualifiedNullable { get; }
   string TypeFullyQualifiedNullAnnotated { get; }
   string TypeFullyQualifiedWithNullability { get; }
   SpecialType SpecialType { get; }
   bool IsReferenceType { get; }
   bool IsReferenceTypeOrNullableStruct { get; }
   bool IsNullableStruct { get; }
   NullableAnnotation NullableAnnotation { get; }
   bool IsFormattable { get; }
   bool IsComparable { get; }
   bool IsParsable { get; }
   ImplementedComparisonOperators ComparisonOperators { get; }
   ImplementedOperators AdditionOperators { get; }
   ImplementedOperators SubtractionOperators { get; }
   ImplementedOperators MultiplyOperators { get; }
   ImplementedOperators DivisionOperators { get; }
}
