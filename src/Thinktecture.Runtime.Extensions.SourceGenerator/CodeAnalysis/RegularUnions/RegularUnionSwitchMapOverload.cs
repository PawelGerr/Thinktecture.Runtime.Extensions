namespace Thinktecture.CodeAnalysis.RegularUnions;

public sealed class RegularUnionSwitchMapOverload : IEquatable<RegularUnionSwitchMapOverload>, IHashCodeComputable
{
   public IReadOnlyList<string> StopAtTypeNames { get; }

   public RegularUnionSwitchMapOverload(IReadOnlyList<string> stopAtTypeNames)
   {
      StopAtTypeNames = stopAtTypeNames;
   }

   public bool Equals(RegularUnionSwitchMapOverload? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return StopAtTypeNames.SequenceEqual(other.StopAtTypeNames);
   }

   public override bool Equals(object? obj)
   {
      return obj is RegularUnionSwitchMapOverload other && Equals(other);
   }

   public override int GetHashCode()
   {
      return StopAtTypeNames.ComputeHashCode(StringComparer.Ordinal);
   }
}
