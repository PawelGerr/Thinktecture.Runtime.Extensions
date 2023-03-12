using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class TypedMemberState : IEquatable<TypedMemberState>, ITypedMemberState
{
   private readonly ITypeSymbol _type;

   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => _type.IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeFullyQualifiedWithNullability => _type is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated } ? TypeFullyQualifiedNullAnnotated : TypeFullyQualified;

   private string? _typeMinimallyQualified;
   public string TypeMinimallyQualified => _typeMinimallyQualified ??= _type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

   public NullableAnnotation NullableAnnotation => _type.NullableAnnotation;
   public SpecialType SpecialType => _type.SpecialType;
   public bool IsReferenceType => _type.IsReferenceType;
   public bool IsNullableStruct => _type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public bool IsReferenceTypeOrNullableStruct => _type.IsReferenceType || IsNullableStruct;
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
      _type = type;

      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";

      foreach (var @interface in type.AllInterfaces)
      {
         if (@interface.IsFormattableInterface())
         {
            IsFormattable = true;
         }
         else if (@interface.IsComparableInterface(_type))
         {
            IsComparable = true;
         }
         else if (@interface.IsParsableInterface(_type))
         {
            IsParsable = true;
         }
         else if (@interface.IsIAdditionOperators(_type))
         {
            HasAdditionOperators = true;
         }
         else if (@interface.IsISubtractionOperators(_type))
         {
            HasSubtractionOperators = true;
         }
         else if (@interface.IsIMultiplyOperators(_type))
         {
            HasMultiplyOperators = true;
         }
         else if (@interface.IsIDivisionOperators(_type))
         {
            HasDivisionOperators = true;
         }
         else if (@interface.IsIComparisonOperators(_type))
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
             && _type.OriginalDefinition.SpecialType == other._type.OriginalDefinition.SpecialType
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
         hashCode = (hashCode * 397) ^ _type.OriginalDefinition.SpecialType.GetHashCode();
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
