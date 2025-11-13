namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct ConstructorState(ImmutableArray<DefaultMemberState> arguments) : IEquatable<ConstructorState>, IHashCodeComputable
{
   public ImmutableArray<DefaultMemberState> Arguments { get; } = arguments;

   public bool Equals(ConstructorState other)
   {
      return Arguments.SequenceEqual(other.Arguments);
   }

   public override bool Equals(object? obj)
   {
      return obj is ConstructorState state && Equals(state);
   }

   public override int GetHashCode()
   {
      return Arguments.ComputeHashCode();
   }

   public static bool operator ==(ConstructorState left, ConstructorState right)
   {
      return left.Equals(right);
   }

   public static bool operator !=(ConstructorState left, ConstructorState right)
   {
      return !(left == right);
   }
}
