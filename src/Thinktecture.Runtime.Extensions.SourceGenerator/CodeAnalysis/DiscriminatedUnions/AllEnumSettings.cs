namespace Thinktecture.CodeAnalysis.DiscriminatedUnions;

public sealed class AllUnionSettings : IEquatable<AllUnionSettings>
{
   public bool SkipToString { get; }
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public IReadOnlyList<MemberTypeSetting> MemberTypeSettings { get; }
   public StringComparison DefaultStringComparison { get; }
   public UnionConstructorAccessModifier ConstructorAccessModifier { get; }
   public bool SkipImplicitConversionFromValue { get; }

   public AllUnionSettings(AttributeData attribute, int numberOfMemberTypes)
   {
      SkipToString = attribute.FindSkipToString() ?? false;
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      DefaultStringComparison = attribute.FindDefaultStringComparison();
      ConstructorAccessModifier = attribute.FindUnionConstructorAccessModifier();
      SkipImplicitConversionFromValue = attribute.FindSkipImplicitConversionFromValue();

      var memberTypeSettings = new MemberTypeSetting[numberOfMemberTypes];
      MemberTypeSettings = memberTypeSettings;

      for (var i = 0; i < numberOfMemberTypes; i++)
      {
         memberTypeSettings[i] = new MemberTypeSetting(attribute.FindTxIsNullableReferenceType(i + 1),
                                                       attribute.FindTxName(i + 1));
      }
   }

   public override bool Equals(object? obj)
   {
      return obj is AllUnionSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(AllUnionSettings? other)
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
             && SkipImplicitConversionFromValue == other.SkipImplicitConversionFromValue
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
         hashCode = (hashCode * 397) ^ SkipImplicitConversionFromValue.GetHashCode();
         hashCode = (hashCode * 397) ^ MemberTypeSettings.ComputeHashCode();

         return hashCode;
      }
   }
}
