namespace Thinktecture.CodeAnalysis.SmartEnums;

public readonly struct EnumSettings : IEquatable<EnumSettings>
{
   private readonly AllEnumSettings _settings;
   private readonly AttributeInfo _attributeInfo;

   public bool IsValidatable => _settings.IsValidatable;
   public bool SkipToString => _settings.SkipToString;
   public SwitchMapMethodsGeneration SwitchMethods => _settings.SwitchMethods;
   public SwitchMapMethodsGeneration MapMethods => _settings.MapMethods;
   public bool HasStructLayoutAttribute => _attributeInfo.HasStructLayoutAttribute;
   public string? KeyMemberEqualityComparerAccessor => _attributeInfo.KeyMemberEqualityComparerAccessor;
   public ImmutableArray<DesiredFactory> DesiredFactories => _attributeInfo.DesiredFactories;

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
             && SwitchMethods == other.SwitchMethods
             && MapMethods == other.MapMethods
             && HasStructLayoutAttribute == other.HasStructLayoutAttribute
             && KeyMemberEqualityComparerAccessor == other.KeyMemberEqualityComparerAccessor
             && DesiredFactories.SequenceEqual(other.DesiredFactories);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = IsValidatable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ SwitchMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ MapMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStructLayoutAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMemberEqualityComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ DesiredFactories.ComputeHashCode();

         return hashCode;
      }
   }
}
