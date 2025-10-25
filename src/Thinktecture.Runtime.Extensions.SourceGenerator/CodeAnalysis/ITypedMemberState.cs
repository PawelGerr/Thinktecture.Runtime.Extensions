namespace Thinktecture.CodeAnalysis;

public interface ITypedMemberState : IEquatable<ITypedMemberState>
{
   string TypeFullyQualified { get; }
   string TypeMinimallyQualified { get; }
   SpecialType SpecialType { get; }
   TypeKind TypeKind { get; }
   bool IsReferenceType { get; }
   bool IsValueType { get; }
   bool IsTypeParameter { get; }
   bool IsReferenceTypeOrNullableStruct { get; }
   bool IsNullableStruct { get; }
   NullableAnnotation NullableAnnotation { get; }
   bool IsFormattable { get; }
   bool IsComparable { get; }
   bool IsParsable { get; }
   bool IsToStringReturnTypeNullable { get; }
   ImplementedComparisonOperators ComparisonOperators { get; }
   ImplementedOperators AdditionOperators { get; }
   ImplementedOperators SubtractionOperators { get; }
   ImplementedOperators MultiplyOperators { get; }
   ImplementedOperators DivisionOperators { get; }
}
