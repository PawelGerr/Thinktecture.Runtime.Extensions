using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class TypedMemberState : IEquatable<TypedMemberState>, ITypedMemberState
{
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeFullyQualifiedWithNullability => IsReferenceType && NullableAnnotation == NullableAnnotation.Annotated ? TypeFullyQualifiedNullAnnotated : TypeFullyQualified;
   public string TypeMinimallyQualified { get; }

   public NullableAnnotation NullableAnnotation { get; }
   public SpecialType SpecialType { get; }
   public bool IsReferenceType { get; }
   public bool IsNullableStruct { get; }
   public bool IsReferenceTypeOrNullableStruct => IsReferenceType || IsNullableStruct;
   public bool IsFormattable { get; }
   public bool IsComparable { get; }
   public bool IsParsable { get; }
   public bool HasComparisonOperators { get; }
   public bool HasAdditionOperators { get; }
   public bool HasSubtractionOperators { get; }
   public bool HasMultiplyOperators { get; }
   public bool HasDivisionOperators { get; }

   public TypedMemberState(ITypeSymbol type)
   {
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      NullableAnnotation = type.NullableAnnotation;
      SpecialType = type.SpecialType;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

      foreach (var @interface in type.AllInterfaces)
      {
         if (@interface.IsFormattableInterface())
         {
            IsFormattable = true;
         }
         else if (@interface.IsComparableInterface(type))
         {
            IsComparable = true;
         }
         else if (@interface.IsParsableInterface(type))
         {
            IsParsable = true;
         }
         else if (@interface.IsIAdditionOperators(type))
         {
            HasAdditionOperators = true;
         }
         else if (@interface.IsISubtractionOperators(type))
         {
            HasSubtractionOperators = true;
         }
         else if (@interface.IsIMultiplyOperators(type))
         {
            HasMultiplyOperators = true;
         }
         else if (@interface.IsIDivisionOperators(type))
         {
            HasDivisionOperators = true;
         }
         else if (@interface.IsIComparisonOperators(type))
         {
            HasComparisonOperators = true;
         }
      }
   }

   public override bool Equals(object? obj)
   {
      return obj is TypedMemberState other && Equals(other);
   }

   public bool Equals(ITypedMemberState? obj)
   {
      return obj is TypedMemberState other && Equals(other);
   }

   public bool Equals(TypedMemberState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualifiedWithNullability == other.TypeFullyQualifiedWithNullability
             && SpecialType == other.SpecialType
             && IsNullableStruct == other.IsNullableStruct
             && IsReferenceType == other.IsReferenceType
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && IsParsable == other.IsParsable
             && HasComparisonOperators == other.HasComparisonOperators
             && HasAdditionOperators == other.HasAdditionOperators
             && HasSubtractionOperators == other.HasSubtractionOperators
             && HasMultiplyOperators == other.HasMultiplyOperators
             && HasDivisionOperators == other.HasDivisionOperators;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualifiedWithNullability.GetHashCode();
         hashCode = (hashCode * 397) ^ SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsNullableStruct.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ HasComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasAdditionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasSubtractionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasMultiplyOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ HasDivisionOperators.GetHashCode();

         return hashCode;
      }
   }
}
