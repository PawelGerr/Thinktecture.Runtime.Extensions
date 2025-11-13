namespace Thinktecture.CodeAnalysis;

public readonly struct ParameterState(
   string name,
   string type,
   RefKind refKind)
   : IEquatable<ParameterState>, IHashCodeComputable
{
   public string Name { get; } = name;
   public string Type { get; } = type;
   public RefKind RefKind { get; } = refKind;

   public override bool Equals(object? obj)
   {
      return obj is ParameterState other && Equals(other);
   }

   public bool Equals(ParameterState other)
   {
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

   public static bool operator ==(ParameterState left, ParameterState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ParameterState left, ParameterState right)
   {
      return !(left == right);
   }
}
