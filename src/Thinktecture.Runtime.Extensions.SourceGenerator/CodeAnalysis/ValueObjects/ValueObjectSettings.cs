namespace Thinktecture.CodeAnalysis.ValueObjects;

public readonly struct ValueObjectSettings : IEquatable<ValueObjectSettings>
{
   private readonly AllValueObjectSettings _allSettings;
   private readonly AttributeInfo _attributeInfo;

   public bool SkipFactoryMethods => _allSettings.SkipFactoryMethods;
   public bool SkipToString => _allSettings.SkipToString;
   public bool EmptyStringInFactoryMethodsYieldsNull => _allSettings.EmptyStringInFactoryMethodsYieldsNull;
   public bool NullInFactoryMethodsYieldsNull => _allSettings.NullInFactoryMethodsYieldsNull;
   public string DefaultInstancePropertyName => _allSettings.DefaultInstancePropertyName;
   public IReadOnlyList<TypeInfo> DesiredFactorySourceTypes => _attributeInfo.DesiredFactorySourceTypes;

   public ValueObjectSettings(
      AllValueObjectSettings allSettings,
      AttributeInfo attributeInfo)
   {
      _allSettings = allSettings;
      _attributeInfo = attributeInfo;
   }

   public bool Equals(ValueObjectSettings other)
   {
      return _allSettings.SkipFactoryMethods == other._allSettings.SkipFactoryMethods
             && _allSettings.SkipToString == other._allSettings.SkipToString
             && _allSettings.EmptyStringInFactoryMethodsYieldsNull == other._allSettings.EmptyStringInFactoryMethodsYieldsNull
             && _allSettings.NullInFactoryMethodsYieldsNull == other._allSettings.NullInFactoryMethodsYieldsNull
             && _allSettings.DefaultInstancePropertyName == other._allSettings.DefaultInstancePropertyName
             && _attributeInfo.DesiredFactorySourceTypes.EqualsTo(other._attributeInfo.DesiredFactorySourceTypes);
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectSettings other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = _allSettings.SkipFactoryMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ _allSettings.SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ _allSettings.EmptyStringInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ _allSettings.NullInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ _allSettings.DefaultInstancePropertyName.GetHashCode();
         hashCode = (hashCode * 397) ^ _attributeInfo.DesiredFactorySourceTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
