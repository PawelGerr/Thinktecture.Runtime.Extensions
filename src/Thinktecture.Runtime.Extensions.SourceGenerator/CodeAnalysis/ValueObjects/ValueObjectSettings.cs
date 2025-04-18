namespace Thinktecture.CodeAnalysis.ValueObjects;

public readonly struct ValueObjectSettings : IEquatable<ValueObjectSettings>
{
   private readonly AllValueObjectSettings _allSettings;
   private readonly AttributeInfo _attributeInfo;

   public bool SkipFactoryMethods => _allSettings.SkipFactoryMethods;
   public AccessModifier ConstructorAccessModifier => _allSettings.ConstructorAccessModifier;
   public bool SkipKeyMember => _allSettings.SkipKeyMember;
   public string CreateFactoryMethodName => _allSettings.CreateFactoryMethodName;
   public string TryCreateFactoryMethodName => _allSettings.TryCreateFactoryMethodName;
   public bool SkipToString => _allSettings.SkipToString;
   public bool EmptyStringInFactoryMethodsYieldsNull => _allSettings.EmptyStringInFactoryMethodsYieldsNull;
   public bool NullInFactoryMethodsYieldsNull => _allSettings.NullInFactoryMethodsYieldsNull;
   public string DefaultInstancePropertyName => _allSettings.DefaultInstancePropertyName;
   public bool AllowDefaultStructs => _allSettings.AllowDefaultStructs;
   public ConversionOperatorsGeneration ConversionToKeyMemberType => _allSettings.ConversionToKeyMemberType;
   public ConversionOperatorsGeneration UnsafeConversionToKeyMemberType => _allSettings.UnsafeConversionToKeyMemberType;
   public ConversionOperatorsGeneration ConversionFromKeyMemberType => _allSettings.ConversionFromKeyMemberType;
   public StringComparison DefaultStringComparison => _allSettings.DefaultStringComparison;
   public bool HasStructLayoutAttribute => _attributeInfo.HasStructLayoutAttribute;
   public string? KeyMemberEqualityComparerAccessor => _attributeInfo.KeyMemberEqualityComparerAccessor;
   public SerializationFrameworks SerializationFrameworks => _allSettings.SerializationFrameworks;
   public ImmutableArray<DesiredFactory> DesiredFactories => _attributeInfo.DesiredFactories;

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
             && ConstructorAccessModifier == other.ConstructorAccessModifier
             && SkipKeyMember == other.SkipKeyMember
             && CreateFactoryMethodName == other.CreateFactoryMethodName
             && TryCreateFactoryMethodName == other.TryCreateFactoryMethodName
             && SkipToString == other.SkipToString
             && EmptyStringInFactoryMethodsYieldsNull == other.EmptyStringInFactoryMethodsYieldsNull
             && NullInFactoryMethodsYieldsNull == other.NullInFactoryMethodsYieldsNull
             && DefaultInstancePropertyName == other.DefaultInstancePropertyName
             && AllowDefaultStructs == other.AllowDefaultStructs
             && ConversionToKeyMemberType == other.ConversionToKeyMemberType
             && UnsafeConversionToKeyMemberType == other.UnsafeConversionToKeyMemberType
             && ConversionFromKeyMemberType == other.ConversionFromKeyMemberType
             && DefaultStringComparison == other.DefaultStringComparison
             && HasStructLayoutAttribute == other.HasStructLayoutAttribute
             && KeyMemberEqualityComparerAccessor == other.KeyMemberEqualityComparerAccessor
             && SerializationFrameworks == other.SerializationFrameworks
             && DesiredFactories.SequenceEqual(other.DesiredFactories);
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
         hashCode = (hashCode * 397) ^ (int)ConstructorAccessModifier;
         hashCode = (hashCode * 397) ^ SkipKeyMember.GetHashCode();
         hashCode = (hashCode * 397) ^ CreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ TryCreateFactoryMethodName.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ EmptyStringInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ NullInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ DefaultInstancePropertyName.GetHashCode();
         hashCode = (hashCode * 397) ^ AllowDefaultStructs.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)ConversionToKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)UnsafeConversionToKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)ConversionFromKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)DefaultStringComparison;
         hashCode = (hashCode * 397) ^ HasStructLayoutAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMemberEqualityComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ (int)SerializationFrameworks;
         hashCode = (hashCode * 397) ^ DesiredFactories.ComputeHashCode();

         return hashCode;
      }
   }
}
