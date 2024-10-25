namespace Thinktecture.CodeAnalysis;

public sealed class ContainingTypeState : IEquatable<ContainingTypeState>, IHashCodeComputable
{
   public string Name { get; }
   public bool IsReferenceType { get; }

   public ContainingTypeState(
      string name,
      bool isReferenceType)
   {
      Name = name;
      IsReferenceType = isReferenceType;
   }

   public bool Equals(ContainingTypeState? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return Name == other.Name
             && IsReferenceType == other.IsReferenceType;
   }

   public override bool Equals(object? obj)
   {
      return obj is ContainingTypeState other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         return (Name.GetHashCode() * 397) ^ IsReferenceType.GetHashCode();
      }
   }
}
