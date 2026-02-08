namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumSettings(
   AllEnumSettings settings,
   AttributeInfo attributeInfo) : IEquatable<SmartEnumSettings>
{
   public bool SkipToString => settings.SkipToString;
   public bool DisableSpanBasedJsonConversion => settings.DisableSpanBasedJsonConversion;
   public SwitchMapMethodsGeneration SwitchMethods => settings.SwitchMethods;
   public SwitchMapMethodsGeneration MapMethods => settings.MapMethods;
   public string SwitchMapStateParameterName => settings.SwitchMapStateParameterName;
   public ConversionOperatorsGeneration ConversionToKeyMemberType => settings.ConversionToKeyMemberType;
   public ConversionOperatorsGeneration ConversionFromKeyMemberType => settings.ConversionFromKeyMemberType;
   public string? KeyMemberEqualityComparerAccessor => attributeInfo.KeyMemberEqualityComparerAccessor;

   public override bool Equals(object? obj)
   {
      return obj is SmartEnumSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(SmartEnumSettings? other)
   {
      if (other is null)
         return false;

      return SkipToString == other.SkipToString
             && DisableSpanBasedJsonConversion == other.DisableSpanBasedJsonConversion
             && SwitchMethods == other.SwitchMethods
             && MapMethods == other.MapMethods
             && ConversionToKeyMemberType == other.ConversionToKeyMemberType
             && ConversionFromKeyMemberType == other.ConversionFromKeyMemberType
             && KeyMemberEqualityComparerAccessor == other.KeyMemberEqualityComparerAccessor
             && SwitchMapStateParameterName == other.SwitchMapStateParameterName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ DisableSpanBasedJsonConversion.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)SwitchMethods;
         hashCode = (hashCode * 397) ^ (int)MapMethods;
         hashCode = (hashCode * 397) ^ (int)ConversionFromKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)ConversionToKeyMemberType;
         hashCode = (hashCode * 397) ^ (KeyMemberEqualityComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ SwitchMapStateParameterName.GetHashCode();

         return hashCode;
      }
   }
}
