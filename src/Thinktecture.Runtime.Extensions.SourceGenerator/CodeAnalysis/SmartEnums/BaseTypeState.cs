namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct BaseTypeState(IReadOnlyList<ConstructorState> constructors) : IEquatable<BaseTypeState>
{
   public IReadOnlyList<ConstructorState> Constructors { get; } = constructors;

   public override bool Equals(object? obj)
   {
      return obj is BaseTypeState other && Equals(other);
   }

   public bool Equals(BaseTypeState other)
   {
      return Constructors.SequenceEqual(other.Constructors);
   }

   public override int GetHashCode()
   {
      var hashCode = Constructors.ComputeHashCode();

      return hashCode;
   }

   public static bool operator ==(BaseTypeState left, BaseTypeState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(BaseTypeState left, BaseTypeState right)
   {
      return !(left == right);
   }
}
