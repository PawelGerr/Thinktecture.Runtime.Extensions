namespace Thinktecture.CodeAnalysis.RegularUnions;

public sealed class RegularUnionSettings : IEquatable<RegularUnionSettings>, IHashCodeComputable
{
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public ConversionOperatorsGeneration ConversionFromValue { get; }
   public string SwitchMapStateParameterName { get; }

   public RegularUnionSettings(AttributeData attribute)
   {
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      ConversionFromValue = attribute.FindConversionFromValue() ?? ConversionOperatorsGeneration.Implicit;
      SwitchMapStateParameterName = attribute.FindSwitchMapStateParameterName();
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
             && SwitchMapStateParameterName == other.SwitchMapStateParameterName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SwitchMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ MapMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ ConversionFromValue.GetHashCode();
         hashCode = (hashCode * 397) ^ SwitchMapStateParameterName.GetHashCode();

         return hashCode;
      }
   }
}
