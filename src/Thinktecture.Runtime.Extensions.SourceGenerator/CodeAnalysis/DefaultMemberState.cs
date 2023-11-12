namespace Thinktecture.CodeAnalysis;

public sealed class DefaultMemberState : IMemberState, IEquatable<DefaultMemberState>
{
   private readonly ITypedMemberState _typedMemberState;

   public string Name { get; }
   public ArgumentName ArgumentName { get; }

   public SpecialType SpecialType => _typedMemberState.SpecialType;
   public string TypeFullyQualified => _typedMemberState.TypeFullyQualified;
   public string TypeFullyQualifiedNullAnnotated => _typedMemberState.TypeFullyQualifiedNullAnnotated;
   public string TypeFullyQualifiedWithNullability => _typedMemberState.TypeFullyQualifiedWithNullability;
   public bool IsReferenceType => _typedMemberState.IsReferenceType;
   public bool IsFormattable => _typedMemberState.IsFormattable;
   public bool IsComparable => _typedMemberState.IsComparable;
   public bool IsParsable => _typedMemberState.IsParsable;
   public ImplementedComparisonOperators ComparisonOperators => _typedMemberState.ComparisonOperators;

   public DefaultMemberState(ITypedMemberState typedMemberState, string name, ArgumentName argumentName)
   {
      _typedMemberState = typedMemberState;
      Name = name;
      ArgumentName = argumentName;
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
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return _typedMemberState.Equals(other._typedMemberState)
             && Name == other.Name
             && ArgumentName == other.ArgumentName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = _typedMemberState.GetHashCode();
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ ArgumentName.GetHashCode();

         return hashCode;
      }
   }
}
