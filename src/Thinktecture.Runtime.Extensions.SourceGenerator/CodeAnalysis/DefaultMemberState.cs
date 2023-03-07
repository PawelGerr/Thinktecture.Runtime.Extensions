using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public sealed class DefaultMemberState : IMemberState, IEquatable<DefaultMemberState>
{
   private readonly ITypedMemberState _typedMemberState;

   public string Name { get; }
   public string ArgumentName { get; }

   public bool IsStatic { get; }

   public SpecialType SpecialType => _typedMemberState.SpecialType;
   public string TypeFullyQualified => _typedMemberState.TypeFullyQualified;
   public string TypeFullyQualifiedNullAnnotated => _typedMemberState.TypeFullyQualifiedNullAnnotated;
   public string TypeFullyQualifiedWithNullability => _typedMemberState.TypeFullyQualifiedWithNullability;
   public bool IsReferenceType => _typedMemberState.IsReferenceType;
   public bool IsFormattable => _typedMemberState.IsFormattable;
   public bool IsComparable => _typedMemberState.IsComparable;
   public bool IsParsable => _typedMemberState.IsParsable;
   public bool IsNullableStruct => _typedMemberState.IsNullableStruct;
   public NullableAnnotation NullableAnnotation => _typedMemberState.NullableAnnotation;
   public bool HasComparisonOperators => _typedMemberState.HasComparisonOperators;
   public string TypeMinimallyQualified => _typedMemberState.TypeMinimallyQualified;

   public DefaultMemberState(ITypedMemberState typedMemberState, string name, string argumentName, bool isStatic)
   {
      _typedMemberState = typedMemberState;
      Name = name;
      ArgumentName = argumentName;
      IsStatic = isStatic;
   }

   public override bool Equals(object? obj)
   {
      return obj is DefaultMemberState other && Equals(other);
   }

   public bool Equals(IMemberState? obj)
   {
      return obj is DefaultMemberState other && Equals(other);
   }

   public bool Equals(DefaultMemberState? other)
   {
      if (ReferenceEquals(this, other))
         return true;
      if (ReferenceEquals(null, other))
         return false;

      return _typedMemberState.Equals(other._typedMemberState)
             && Name == other.Name
             && ArgumentName == other.ArgumentName
             && IsStatic == other.IsStatic;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = _typedMemberState.GetHashCode();
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ ArgumentName.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStatic.GetHashCode();

         return hashCode;
      }
   }
}
