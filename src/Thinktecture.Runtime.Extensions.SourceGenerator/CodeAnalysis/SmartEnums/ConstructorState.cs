namespace Thinktecture.CodeAnalysis.SmartEnums;

public class ConstructorState : IEquatable<ConstructorState>
{
   public IReadOnlyList<IMemberState> Arguments { get; }

   public ConstructorState(IReadOnlyList<IMemberState> arguments)
   {
      Arguments = arguments;
   }

   public bool Equals(ConstructorState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return Arguments.EqualsTo(other.Arguments);
   }

   public override bool Equals(object? obj)
   {
      if (ReferenceEquals(null, obj))
         return false;
      if (ReferenceEquals(this, obj))
         return true;

      return Equals(obj as ConstructorState);
   }

   public override int GetHashCode()
   {
      return Arguments.GetHashCode();
   }
}