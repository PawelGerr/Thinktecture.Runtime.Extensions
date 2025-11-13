namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct BaseTypeState(ImmutableArray<ConstructorState> constructors) : IEquatable<BaseTypeState>
{
   public ImmutableArray<ConstructorState> Constructors { get; } = constructors;

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
      return Constructors.ComputeHashCode();
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
