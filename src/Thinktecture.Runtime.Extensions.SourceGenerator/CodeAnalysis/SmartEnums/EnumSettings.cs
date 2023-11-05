namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct EnumSettings : IEquatable<EnumSettings>
{
   private readonly AllEnumSettings _settings;
   private readonly AttributeInfo _attributeInfo;

   public bool IsValidatable => _settings.IsValidatable;
   public bool SkipToString => _settings.SkipToString;
   public bool SkipSwitchMethods => _settings.SkipSwitchMethods;
   public bool SkipMapMethods => _settings.SkipMapMethods;
   public bool HasStructLayoutAttribute => _attributeInfo.HasStructLayoutAttribute;
   public IReadOnlyList<TypeInfo> DesiredFactorySourceTypes => _attributeInfo.DesiredFactorySourceTypes;

   public EnumSettings(AllEnumSettings settings, AttributeInfo attributeInfo)
   {
      _settings = settings;
      _attributeInfo = attributeInfo;
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(EnumSettings other)
   {
      return IsValidatable == other.IsValidatable
             && SkipToString == other.SkipToString
             && SkipSwitchMethods == other.SkipSwitchMethods
             && SkipMapMethods == other.SkipMapMethods
             && HasStructLayoutAttribute == other.HasStructLayoutAttribute
             && DesiredFactorySourceTypes.EqualsTo(other.DesiredFactorySourceTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = IsValidatable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipSwitchMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipMapMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStructLayoutAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ DesiredFactorySourceTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
