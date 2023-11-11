using System.Collections.Immutable;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class ConstructorState : IEquatable<ConstructorState>, IHashCodeComputable
{
   public ImmutableArray<DefaultMemberState> Arguments { get; }

   public ConstructorState(ImmutableArray<DefaultMemberState> arguments)
   {
      Arguments = arguments;
   }

   public bool Equals(ConstructorState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return Arguments.SequenceEqual(other.Arguments);
   }

   public override bool Equals(object? obj)
   {
      if (obj is null)
         return false;
      if (ReferenceEquals(this, obj))
         return true;

      return Equals(obj as ConstructorState);
   }

   public override int GetHashCode()
   {
      return Arguments.ComputeHashCode();
   }
}
