namespace Thinktecture.CodeAnalysis;

public readonly struct GenericTypeParameterState(
   string name,
   ImmutableArray<string> constraints)
   : IEquatable<GenericTypeParameterState>, IHashCodeComputable
{
   public string Name { get; } = name;
   public ImmutableArray<string> Constraints { get; } = constraints;

   public override bool Equals(object? obj)
   {
      return obj is GenericTypeParameterState other && Equals(other);
   }

   public bool Equals(GenericTypeParameterState other)
   {
      return Name == other.Name
             && Constraints.SequenceEqual(other.Constraints);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Name.GetHashCode();
         hashCode = (hashCode * 397) ^ Constraints.ComputeHashCode(StringComparer.Ordinal);
         return hashCode;
      }
   }

   public static bool operator ==(GenericTypeParameterState left, GenericTypeParameterState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(GenericTypeParameterState left, GenericTypeParameterState right)
   {
      return !(left == right);
   }
}
