using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class CachedTypedMemberState : IEquatable<CachedTypedMemberState>, ITypedMemberState
{
   private readonly int _hashCode;

   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated { get; }
   public string TypeFullyQualifiedWithNullability { get; }
   public SpecialType SpecialType { get; }
   public bool IsReferenceType { get; }
   public bool IsReferenceTypeOrNullableStruct { get; }
   public bool IsNullableStruct { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }
   public bool HasComparisonOperators { get; }
   public bool HasAdditionOperators { get; }
   public bool HasSubtractionOperators { get; }
   public bool HasMultiplyOperators { get; }
   public bool HasDivisionOperators { get; }

   public CachedTypedMemberState(ITypedMemberState typedMemberState)
   {
      _hashCode = typedMemberState.GetHashCode();

      TypeFullyQualified = typedMemberState.TypeFullyQualified;
      TypeFullyQualifiedNullable = typedMemberState.TypeFullyQualifiedNullable;
      TypeFullyQualifiedNullAnnotated = typedMemberState.TypeFullyQualifiedNullAnnotated;
      TypeFullyQualifiedWithNullability = typedMemberState.TypeFullyQualifiedWithNullability;
      SpecialType = typedMemberState.SpecialType;
      IsReferenceType = typedMemberState.IsReferenceType;
      IsReferenceTypeOrNullableStruct = typedMemberState.IsReferenceTypeOrNullableStruct;
      IsNullableStruct = typedMemberState.IsNullableStruct;
      NullableAnnotation = typedMemberState.NullableAnnotation;
      IsFormattable = typedMemberState.IsFormattable;
      IsComparable = typedMemberState.IsComparable;
      IsParsable = typedMemberState.IsParsable;
      HasComparisonOperators = typedMemberState.HasComparisonOperators;
      HasAdditionOperators = typedMemberState.HasAdditionOperators;
      HasSubtractionOperators = typedMemberState.HasSubtractionOperators;
      HasMultiplyOperators = typedMemberState.HasMultiplyOperators;
      HasDivisionOperators = typedMemberState.HasDivisionOperators;
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
