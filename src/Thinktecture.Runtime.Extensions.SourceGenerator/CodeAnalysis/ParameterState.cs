namespace Thinktecture.CodeAnalysis;

public sealed class ParameterState : IEquatable<ParameterState>, IHashCodeComputable
{
   public string Name { get; }
   public string Type { get; }
   public RefKind RefKind { get; }

   public ParameterState(
      string name,
      string type,
      RefKind refKind)
   {
      Name = name;
      Type = type;
      RefKind = refKind;
   }

   public override bool Equals(object? obj)
   {
      return obj is ParameterState other && Equals(other);
   }

   public bool Equals(ParameterState? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return Name == other.Name
             && Type == other.Type
             && RefKind == other.RefKind;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Name.GetHashCode();
         hashCode = (hashCode * 397) ^ Type.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)RefKind;
         return hashCode;
      }
   }
}
