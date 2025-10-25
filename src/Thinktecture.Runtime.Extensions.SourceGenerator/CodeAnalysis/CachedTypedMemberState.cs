namespace Thinktecture.CodeAnalysis;

public sealed class CachedTypedMemberState : IEquatable<CachedTypedMemberState>, ITypedMemberState
{
   private readonly int _hashCode;

   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public SpecialType SpecialType { get; }
   public TypeKind TypeKind { get; }
   public bool IsReferenceType { get; }
   public bool IsStruct { get; }
   public bool IsTypeParameter { get; }
   public bool IsReferenceTypeOrNullableStruct { get; }
   public bool IsNullableStruct { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }
   public bool IsToStringReturnTypeNullable { get; }
   public ImplementedComparisonOperators ComparisonOperators { get; }
   public ImplementedOperators AdditionOperators { get; }
   public ImplementedOperators SubtractionOperators { get; }
   public ImplementedOperators MultiplyOperators { get; }
   public ImplementedOperators DivisionOperators { get; }

   public CachedTypedMemberState(ITypedMemberState typedMemberState)
   {
      _hashCode = typedMemberState.GetHashCode();

      TypeFullyQualified = typedMemberState.TypeFullyQualified;
      TypeMinimallyQualified = typedMemberState.TypeMinimallyQualified;
      SpecialType = typedMemberState.SpecialType;
      TypeKind = typedMemberState.TypeKind;
      IsReferenceType = typedMemberState.IsReferenceType;
      IsStruct = typedMemberState.IsStruct;
      IsTypeParameter = typedMemberState.IsTypeParameter;
      IsReferenceTypeOrNullableStruct = typedMemberState.IsReferenceTypeOrNullableStruct;
      IsNullableStruct = typedMemberState.IsNullableStruct;
      NullableAnnotation = typedMemberState.NullableAnnotation;
      IsFormattable = typedMemberState.IsFormattable;
      IsComparable = typedMemberState.IsComparable;
      IsParsable = typedMemberState.IsParsable;
      IsToStringReturnTypeNullable = typedMemberState.IsToStringReturnTypeNullable;
      ComparisonOperators = typedMemberState.ComparisonOperators;
      AdditionOperators = typedMemberState.AdditionOperators;
      SubtractionOperators = typedMemberState.SubtractionOperators;
      MultiplyOperators = typedMemberState.MultiplyOperators;
      DivisionOperators = typedMemberState.DivisionOperators;
   }

   public override bool Equals(object? obj)
   {
      return ReferenceEquals(this, obj);
   }

   public bool Equals(ITypedMemberState? other)
   {
      return ReferenceEquals(this, other);
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
