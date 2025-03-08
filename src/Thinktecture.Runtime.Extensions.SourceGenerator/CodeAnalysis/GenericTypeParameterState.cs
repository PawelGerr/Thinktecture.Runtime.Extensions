namespace Thinktecture.CodeAnalysis;

public sealed class GenericTypeParameterState : IEquatable<GenericTypeParameterState>, IHashCodeComputable
{
   public string Name { get; }
   public IReadOnlyList<string> Constraints { get; }

   public GenericTypeParameterState(
      string name,
      IReadOnlyList<string> constraints)
   {
      Name = name;
      Constraints = constraints;
   }

   public override bool Equals(object? obj)
   {
      return obj is GenericTypeParameterState other && Equals(other);
   }

   public bool Equals(GenericTypeParameterState? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return Name == other.Name
             && Constraints.SequenceEqual(other.Constraints);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = Name.GetHashCode();
         hashCode = (hashCode * 397) ^ Constraints.ComputeHashCode();
         return hashCode;
      }
   }
}
