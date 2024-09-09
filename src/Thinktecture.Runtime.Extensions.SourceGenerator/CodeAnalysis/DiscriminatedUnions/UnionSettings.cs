namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public readonly struct UnionSettings : IEquatable<UnionSettings>
{
   private readonly AllUnionSettings _settings;
   private readonly AttributeInfo _attributeInfo;

   public bool SkipToString => _settings.SkipToString;
   public SwitchMapMethodsGeneration SwitchMethods => _settings.SwitchMethods;
   public SwitchMapMethodsGeneration MapMethods => _settings.MapMethods;
   public StringComparison DefaultStringComparison => _settings.DefaultStringComparison;
   public UnionConstructorAccessModifier ConstructorAccessModifier => _settings.ConstructorAccessModifier;
   public bool SkipImplicitConversionFromValue => _settings.SkipImplicitConversionFromValue;
   public bool HasStructLayoutAttribute => _attributeInfo.HasStructLayoutAttribute;

   public UnionSettings(AllUnionSettings settings, AttributeInfo attributeInfo)
   {
      _settings = settings;
      _attributeInfo = attributeInfo;
   }

   public override bool Equals(object? obj)
   {
      return obj is UnionSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(UnionSettings other)
   {
      return SkipToString == other.SkipToString
             && SwitchMethods == other.SwitchMethods
             && MapMethods == other.MapMethods
             && DefaultStringComparison == other.DefaultStringComparison
             && ConstructorAccessModifier == other.ConstructorAccessModifier
             && SkipImplicitConversionFromValue == other.SkipImplicitConversionFromValue
             && HasStructLayoutAttribute == other.HasStructLayoutAttribute;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ SwitchMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ MapMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)DefaultStringComparison;
         hashCode = (hashCode * 397) ^ (int)ConstructorAccessModifier;
         hashCode = (hashCode * 397) ^ SkipImplicitConversionFromValue.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStructLayoutAttribute.GetHashCode();

         return hashCode;
      }
   }
}
