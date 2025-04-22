namespace Thinktecture.CodeAnalysis;

public sealed class KeyMemberState : IMemberState, IEquatable<KeyMemberState>
{
   private readonly ITypedMemberState _typedMemberState;

   public AccessModifier AccessModifier { get; }
   public MemberKind Kind { get; }
   public string Name { get; }
   public string ArgumentName { get; }

   public bool IsRecord => false;

   public SpecialType SpecialType => _typedMemberState.SpecialType;
   public bool IsTypeParameter => _typedMemberState.TypeKind == TypeKind.TypeParameter;
   public bool IsInterface => _typedMemberState.TypeKind == TypeKind.Interface;
   public string TypeFullyQualified => _typedMemberState.TypeFullyQualified;
   public bool IsReferenceType => _typedMemberState.IsReferenceType;
   public bool IsFormattable => _typedMemberState.IsFormattable;
   public bool IsComparable => _typedMemberState.IsComparable;
   public bool IsParsable => _typedMemberState.IsParsable;
   public bool IsToStringReturnTypeNullable => _typedMemberState.IsToStringReturnTypeNullable;
   public ImplementedComparisonOperators ComparisonOperators => _typedMemberState.ComparisonOperators;
   public ImplementedOperators AdditionOperators => _typedMemberState.AdditionOperators;
   public ImplementedOperators SubtractionOperators => _typedMemberState.SubtractionOperators;
   public ImplementedOperators MultiplyOperators => _typedMemberState.MultiplyOperators;
   public ImplementedOperators DivisionOperators => _typedMemberState.DivisionOperators;
   public bool IsNullableStruct => _typedMemberState.IsNullableStruct;
   public NullableAnnotation NullableAnnotation => _typedMemberState.NullableAnnotation;

   public KeyMemberState(
      ITypedMemberState typedMemberState,
      AccessModifier accessModifier,
      MemberKind kind,
      string name,
      string argumentName)
   {
      _typedMemberState = typedMemberState;
      AccessModifier = accessModifier;
      Kind = kind;
      Name = name;
      ArgumentName = argumentName;
   }

   public override bool Equals(object? obj)
   {
      return obj is KeyMemberState other && Equals(other);
   }

   public bool Equals(IMemberState? obj)
   {
      return obj is KeyMemberState other && Equals(other);
   }

   public bool Equals(KeyMemberState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return _typedMemberState.Equals(other._typedMemberState)
             && AccessModifier == other.AccessModifier
             && Kind == other.Kind
             && Name == other.Name
             && ArgumentName == other.ArgumentName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = _typedMemberState.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)AccessModifier;
         hashCode = (hashCode * 397) ^ (int)Kind;
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ ArgumentName.GetHashCode();

         return hashCode;
      }
   }
}
