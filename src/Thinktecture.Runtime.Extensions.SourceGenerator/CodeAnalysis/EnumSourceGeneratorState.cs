using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class EnumSourceGeneratorState : EnumSourceGeneratorStateBase<int>, IEquatable<EnumSourceGeneratorState>
{
   public EnumSourceGeneratorState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
      : base(type, enumInterface)
   {
   }

   protected override int GetBaseEnumExtension(INamedTypeSymbol baseType)
   {
      return default;
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumSourceGeneratorState other && Equals(other);
   }

   public bool Equals(EnumSourceGeneratorState? other)
   {
      return base.Equals(other);
   }

   public override int GetHashCode()
   {
      return base.GetHashCode();
   }
}
