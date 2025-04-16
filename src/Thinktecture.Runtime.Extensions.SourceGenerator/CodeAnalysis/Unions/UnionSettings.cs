namespace Thinktecture.CodeAnalysis.Unions;

public sealed class UnionSettings : IEquatable<UnionSettings>, IHashCodeComputable
{
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public ConversionOperatorsGeneration ConversionFromValue { get; }

   public UnionSettings(AttributeData attribute)
   {
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      ConversionFromValue = attribute.FindConversionFromValue() ?? ConversionOperatorsGeneration.Implicit;
   }

   public override bool Equals(object? obj)
   {
      return obj is UnionSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(UnionSettings? other)
   {
      if (other is null)
         return false;

      if (ReferenceEquals(this, other))
         return true;

      return SwitchMethods == other.SwitchMethods
             && MapMethods == other.MapMethods
             && ConversionFromValue == other.ConversionFromValue;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SwitchMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ MapMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ ConversionFromValue.GetHashCode();

         return hashCode;
      }
   }
}
