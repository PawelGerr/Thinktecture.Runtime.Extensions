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
   public IReadOnlyList<DesiredFactory> DesiredFactories => _attributeInfo.DesiredFactories;

   public ValueObjectSettings(
      AllValueObjectSettings allSettings,
      AttributeInfo attributeInfo)
   {
      _allSettings = allSettings;
      _attributeInfo = attributeInfo;
   }

   public bool Equals(ValueObjectSettings other)
   {
      return SkipFactoryMethods == other.SkipFactoryMethods
             && SkipToString == other.SkipToString
             && EmptyStringInFactoryMethodsYieldsNull == other.EmptyStringInFactoryMethodsYieldsNull
             && NullInFactoryMethodsYieldsNull == other.NullInFactoryMethodsYieldsNull
             && DefaultInstancePropertyName == other.DefaultInstancePropertyName
             && DesiredFactories.EqualsTo(other.DesiredFactories);
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectSettings other && Equals(other);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SkipFactoryMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ EmptyStringInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ NullInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ DefaultInstancePropertyName.GetHashCode();
         hashCode = (hashCode * 397) ^ DesiredFactories.ComputeHashCode();

         return hashCode;
      }
   }
}
