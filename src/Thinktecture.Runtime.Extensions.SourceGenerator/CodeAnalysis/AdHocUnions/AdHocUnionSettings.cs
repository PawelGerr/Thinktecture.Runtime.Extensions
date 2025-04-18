namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class AdHocUnionSettings : IEquatable<AdHocUnionSettings>
{
   private readonly AttributeInfo _attributeInfo;

   public bool SkipToString { get; }
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public IReadOnlyList<AdHocUnionMemberTypeSetting> MemberTypeSettings { get; }
   public StringComparison DefaultStringComparison { get; }
   public UnionConstructorAccessModifier ConstructorAccessModifier { get; }
   public ConversionOperatorsGeneration ConversionFromValue { get; }
   public ConversionOperatorsGeneration ConversionToValue { get; }
   public string SwitchMapStateParameterName { get; }
   public bool HasStructLayoutAttribute => _attributeInfo.HasStructLayoutAttribute;

   public AdHocUnionSettings(
      AttributeData attribute,
      int numberOfMemberTypes,
      AttributeInfo attributeInfo)
   {
      SkipToString = attribute.FindSkipToString() ?? false;
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      DefaultStringComparison = attribute.FindDefaultStringComparison();
      ConstructorAccessModifier = attribute.FindUnionConstructorAccessModifier();
      ConversionFromValue = attribute.FindConversionFromValue() ?? ConversionOperatorsGeneration.Implicit;
      ConversionToValue = attribute.FindConversionToValue() ?? ConversionOperatorsGeneration.Explicit;
      SwitchMapStateParameterName = attribute.FindSwitchMapStateParameterName();
      _attributeInfo = attributeInfo;

      var memberTypeSettings = new AdHocUnionMemberTypeSetting[numberOfMemberTypes];
      MemberTypeSettings = memberTypeSettings;

      for (var i = 0; i < numberOfMemberTypes; i++)
      {
         memberTypeSettings[i] = new AdHocUnionMemberTypeSetting(attribute.FindTxIsNullableReferenceType(i + 1),
                                                                 attribute.FindTxName(i + 1));
      }
   }

   public override bool Equals(object? obj)
   {
      return obj is AdHocUnionSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(AdHocUnionSettings? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return SkipToString == other.SkipToString
             && SwitchMethods == other.SwitchMethods
             && MapMethods == other.MapMethods
             && DefaultStringComparison == other.DefaultStringComparison
             && ConstructorAccessModifier == other.ConstructorAccessModifier
             && ConversionFromValue == other.ConversionFromValue
             && ConversionToValue == other.ConversionToValue
             && SwitchMapStateParameterName == other.SwitchMapStateParameterName
             && HasStructLayoutAttribute == other.HasStructLayoutAttribute
             && MemberTypeSettings.SequenceEqual(other.MemberTypeSettings);
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
         hashCode = (hashCode * 397) ^ (int)ConversionFromValue;
         hashCode = (hashCode * 397) ^ (int)ConversionToValue;
         hashCode = (hashCode * 397) ^ SwitchMapStateParameterName.GetHashCode();
         hashCode = (hashCode * 397) ^ HasStructLayoutAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ MemberTypeSettings.ComputeHashCode();

         return hashCode;
      }
   }
}
