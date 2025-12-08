namespace Thinktecture.CodeAnalysis.RegularUnions;

public sealed class RegularUnionSettings : IEquatable<RegularUnionSettings>, IHashCodeComputable
{
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public ConversionOperatorsGeneration ConversionFromValue { get; }
   public string SwitchMapStateParameterName { get; }
   public ImmutableArray<RegularUnionSwitchMapOverload> SwitchMapOverloads { get; }
   public NestedUnionParameterNameGeneration NestedUnionParameterNames { get; }

   public RegularUnionSettings(
      AttributeData attribute,
      ImmutableArray<RegularUnionSwitchMapOverload> switchMapOverloads)
   {
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      ConversionFromValue = attribute.FindConversionFromValue() ?? ConversionOperatorsGeneration.Implicit;
      SwitchMapStateParameterName = attribute.FindSwitchMapStateParameterName();
      SwitchMapOverloads = switchMapOverloads;
      NestedUnionParameterNames = attribute.FindNestedUnionParameterNameGeneration();
   }

   public override bool Equals(object? obj)
   {
      return obj is RegularUnionSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(RegularUnionSettings? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return SwitchMethods == other.SwitchMethods
             && MapMethods == other.MapMethods
             && ConversionFromValue == other.ConversionFromValue
             && SwitchMapStateParameterName == other.SwitchMapStateParameterName
             && NestedUnionParameterNames == other.NestedUnionParameterNames
             && SwitchMapOverloads.SequenceEqual(other.SwitchMapOverloads);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = (int)SwitchMethods;
         hashCode = (hashCode * 397) ^ (int)MapMethods;
         hashCode = (hashCode * 397) ^ (int)ConversionFromValue;
         hashCode = (hashCode * 397) ^ SwitchMapStateParameterName.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)NestedUnionParameterNames;
         hashCode = (hashCode * 397) ^ SwitchMapOverloads.ComputeHashCode();

         return hashCode;
      }
   }
}
