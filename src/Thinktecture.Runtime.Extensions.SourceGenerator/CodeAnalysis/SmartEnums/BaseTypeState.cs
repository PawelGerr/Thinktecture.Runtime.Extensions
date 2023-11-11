using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class BaseTypeState : IEquatable<BaseTypeState>
{
   public IReadOnlyList<ConstructorState> Constructors { get; }
   public bool IsSameAssembly { get; }

   public BaseTypeState(TypedMemberStateFactory factory, INamedTypeSymbol type, bool isSameAssembly)
   {
      IsSameAssembly = isSameAssembly;
      Constructors = type.GetConstructors(factory);
   }

   public override bool Equals(object? obj)
   {
      return obj is BaseTypeState other && Equals(other);
   }

   public bool Equals(BaseTypeState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return Constructors.SequenceEqual(other.Constructors)
             && IsSameAssembly == other.IsSameAssembly;
   }

   public override int GetHashCode()
   {
      var hashCode = Constructors.ComputeHashCode();
      hashCode = (hashCode * 397) ^ IsSameAssembly.GetHashCode();

      return hashCode;
   }
}
