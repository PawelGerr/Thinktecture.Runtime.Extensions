namespace Thinktecture.CodeAnalysis.Unions;

public sealed class UnionSettings : IEquatable<UnionSettings>, IHashCodeComputable
{
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public bool SkipImplicitConversionFromValue { get; }

   public UnionSettings(AttributeData attribute)
   {
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      SkipImplicitConversionFromValue = attribute.FindSkipImplicitConversionFromValue();
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
             && SkipImplicitConversionFromValue == other.SkipImplicitConversionFromValue;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SwitchMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ MapMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipImplicitConversionFromValue.GetHashCode();

         return hashCode;
      }
   }
}
