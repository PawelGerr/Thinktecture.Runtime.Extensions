namespace Thinktecture.CodeAnalysis;

public sealed class CachedTypedMemberState(ITypedMemberState typedMemberState)
   : IEquatable<CachedTypedMemberState>,
     ITypedMemberState
{
   private readonly int _hashCode = typedMemberState.GetHashCode();

   public string TypeFullyQualified { get; } = typedMemberState.TypeFullyQualified;
   public string TypeMinimallyQualified { get; } = typedMemberState.TypeMinimallyQualified;
   public SpecialType SpecialType { get; } = typedMemberState.SpecialType;
   public TypeKind TypeKind { get; } = typedMemberState.TypeKind;
   public bool IsReferenceType { get; } = typedMemberState.IsReferenceType;
   public bool IsValueType { get; } = typedMemberState.IsValueType;
   public bool IsTypeParameter { get; } = typedMemberState.IsTypeParameter;
   public bool IsReferenceTypeOrNullableStruct { get; } = typedMemberState.IsReferenceTypeOrNullableStruct;
   public bool IsNullableStruct { get; } = typedMemberState.IsNullableStruct;
   public NullableAnnotation NullableAnnotation { get; } = typedMemberState.NullableAnnotation;
   public bool IsFormattable { get; } = typedMemberState.IsFormattable;
   public bool IsComparable { get; } = typedMemberState.IsComparable;
   public bool IsParsable { get; } = typedMemberState.IsParsable;
   public bool IsSpanParsable { get; } = typedMemberState.IsSpanParsable;
   public bool IsToStringReturnTypeNullable { get; } = typedMemberState.IsToStringReturnTypeNullable;
   public ImplementedComparisonOperators ComparisonOperators { get; } = typedMemberState.ComparisonOperators;
   public ImplementedOperators AdditionOperators { get; } = typedMemberState.AdditionOperators;
   public ImplementedOperators SubtractionOperators { get; } = typedMemberState.SubtractionOperators;
   public ImplementedOperators MultiplyOperators { get; } = typedMemberState.MultiplyOperators;
   public ImplementedOperators DivisionOperators { get; } = typedMemberState.DivisionOperators;

   public override bool Equals(object? obj)
   {
      return obj switch
      {
         CachedTypedMemberState cachedTypedMemberState => Equals(cachedTypedMemberState),
         ITypedMemberState typedMemberState => Equals(typedMemberState),
         _ => false
      };
   }

   public bool Equals(ITypedMemberState? other)
   {
      switch (other)
      {
         case CachedTypedMemberState cachedTypedMemberState:
            return ReferenceEquals(this, cachedTypedMemberState);
         case null:
            return false;
      }

      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && SpecialType == other.SpecialType
             && TypeKind == other.TypeKind
             && NullableAnnotation == other.NullableAnnotation
             && IsNullableStruct == other.IsNullableStruct
             && IsReferenceType == other.IsReferenceType
             && IsValueType == other.IsValueType
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && IsParsable == other.IsParsable
             && IsSpanParsable == other.IsSpanParsable
             && IsToStringReturnTypeNullable == other.IsToStringReturnTypeNullable
             && ComparisonOperators == other.ComparisonOperators
             && AdditionOperators == other.AdditionOperators
             && SubtractionOperators == other.SubtractionOperators
             && MultiplyOperators == other.MultiplyOperators
             && DivisionOperators == other.DivisionOperators;
   }

   public bool Equals(CachedTypedMemberState? other)
   {
      return ReferenceEquals(this, other);
   }

   public override int GetHashCode()
   {
      return _hashCode;
   }
}
