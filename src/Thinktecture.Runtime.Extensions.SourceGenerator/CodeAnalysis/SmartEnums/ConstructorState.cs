namespace Thinktecture.CodeAnalysis.SmartEnums;

public class ConstructorState : IEquatable<ConstructorState>
{
   public IReadOnlyList<IMemberState> ConstructorArguments { get; }

   public ConstructorState(IReadOnlyList<IMemberState> constructorArguments)
   {
      ConstructorArguments = constructorArguments;
   }

   public bool Equals(ConstructorState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return ConstructorArguments.EqualsTo(other.ConstructorArguments);
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
      return ConstructorArguments.GetHashCode();
   }
}
