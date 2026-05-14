namespace Thinktecture.CodeAnalysis.AdHocUnions;

public sealed class AdHocUnionSettings : IEquatable<AdHocUnionSettings>
{
   public bool SkipToString { get; }
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public ImmutableArray<AdHocUnionMemberTypeSetting> MemberTypeSettings { get; }
   public StringComparison DefaultStringComparison { get; }
   public UnionConstructorAccessModifier ConstructorAccessModifier { get; }
   public ConversionOperatorsGeneration ConversionFromValue { get; }
   public ConversionOperatorsGeneration ConversionToValue { get; }
   public string SwitchMapStateParameterName { get; }
   public bool UseSingleBackingField { get; }
   public bool SkipEqualityComparison { get; }
   public FactoryMethodGeneration FactoryMethodGeneration { get; }

   /// <summary>
   /// Resolved/normalized info for the user-supplied <c>SingleBackingFieldType</c>.
   /// <c>null</c> when the property was not set, or when it was set to <c>typeof(object)</c>
   /// (normalized away to preserve the legacy <c>UseSingleBackingField = true</c> emission path).
   /// </summary>
   public SingleBackingFieldTypeInfo? SingleBackingFieldType { get; }

   public AdHocUnionSettings(
      AttributeData attribute,
      int numberOfMemberTypes,
      bool hasSingleBackingFieldType,
      SingleBackingFieldTypeInfo? singleBackingFieldType)
   {
      SkipToString = attribute.FindSkipToString() ?? false;
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      DefaultStringComparison = attribute.FindDefaultStringComparison();
      ConstructorAccessModifier = attribute.FindUnionConstructorAccessModifier();
      ConversionFromValue = attribute.FindConversionFromValue() ?? ConversionOperatorsGeneration.Implicit;
      ConversionToValue = attribute.FindConversionToValue() ?? ConversionOperatorsGeneration.Explicit;
      SwitchMapStateParameterName = attribute.FindSwitchMapStateParameterName();
      SkipEqualityComparison = attribute.FindSkipEqualityComparison() ?? false;
      FactoryMethodGeneration = attribute.FindFactoryMethodGeneration();

      SingleBackingFieldType = singleBackingFieldType;

      // The cascade ("setting SingleBackingFieldType implies UseSingleBackingField = true") must
      // fire whenever the user provided ANY value -- including typeof(object). The caller already
      // determined this via the unmodified attribute value, before any normalization.
      UseSingleBackingField = hasSingleBackingFieldType
                              || (attribute.FindUseSingleBackingField() ?? false);

      var memberTypeSettings = ImmutableArray.CreateBuilder<AdHocUnionMemberTypeSetting>(numberOfMemberTypes);

      for (var i = 0; i < numberOfMemberTypes; i++)
      {
         memberTypeSettings.Add(new AdHocUnionMemberTypeSetting(attribute.FindTxIsNullableReferenceType(i + 1),
                                                                attribute.FindTxIsStateless(i + 1),
                                                                attribute.FindTxName(i + 1)));
      }

      MemberTypeSettings = memberTypeSettings.DrainToImmutable();
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
             && UseSingleBackingField == other.UseSingleBackingField
             && SkipEqualityComparison == other.SkipEqualityComparison
             && FactoryMethodGeneration == other.FactoryMethodGeneration
             && SingleBackingFieldType == other.SingleBackingFieldType
             && MemberTypeSettings.SequenceEqual(other.MemberTypeSettings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)SwitchMethods;
         hashCode = (hashCode * 397) ^ (int)MapMethods;
         hashCode = (hashCode * 397) ^ (int)DefaultStringComparison;
         hashCode = (hashCode * 397) ^ (int)ConstructorAccessModifier;
         hashCode = (hashCode * 397) ^ (int)ConversionFromValue;
         hashCode = (hashCode * 397) ^ (int)ConversionToValue;
         hashCode = (hashCode * 397) ^ SwitchMapStateParameterName.GetHashCode();
         hashCode = (hashCode * 397) ^ UseSingleBackingField.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipEqualityComparison.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)FactoryMethodGeneration;
         hashCode = (hashCode * 397) ^ (SingleBackingFieldType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ MemberTypeSettings.ComputeHashCode();

         return hashCode;
      }
   }
}
