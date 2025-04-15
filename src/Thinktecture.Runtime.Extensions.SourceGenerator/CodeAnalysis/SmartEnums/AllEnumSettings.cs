namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class AllEnumSettings : IEquatable<AllEnumSettings>, IKeyMemberSettings
{
   public ValueObjectAccessModifier KeyMemberAccessModifier { get; }
   public ValueObjectMemberKind KeyMemberKind { get; }
   public string KeyMemberName { get; }
   public bool IsValidatable { get; }
   public bool SkipIComparable { get; }
   public bool SkipIParsable { get; }
   public OperatorsGeneration ComparisonOperators { get; }
   public OperatorsGeneration EqualityComparisonOperators { get; }
   public bool SkipIFormattable { get; }
   public bool SkipToString { get; }
   public SwitchMapMethodsGeneration SwitchMethods { get; }
   public SwitchMapMethodsGeneration MapMethods { get; }
   public ConversionOperatorsGeneration ConversionFromKeyMemberType { get; }
   public ConversionOperatorsGeneration ConversionToKeyMemberType { get; }
   public SerializationFrameworks SerializationFrameworks { get; }

   public AllEnumSettings(AttributeData attribute)
   {
      KeyMemberAccessModifier = attribute.FindKeyMemberAccessModifier() ?? Constants.SmartEnum.DEFAULT_KEY_MEMBER_ACCESS_MODIFIER;
      KeyMemberKind = attribute.FindKeyMemberKind() ?? Constants.SmartEnum.DEFAULT_KEY_MEMBER_KIND;
      KeyMemberName = attribute.FindKeyMemberName() ?? Helper.GetDefaultSmartEnumKeyMemberName(KeyMemberAccessModifier, KeyMemberKind);
      IsValidatable = attribute.FindIsValidatable() ?? false;
      SkipIComparable = attribute.FindSkipIComparable() ?? false;
      SkipIParsable = attribute.FindSkipIParsable() ?? false;
      ComparisonOperators = attribute.FindComparisonOperators();
      EqualityComparisonOperators = attribute.FindEqualityComparisonOperators();
      SkipIFormattable = attribute.FindSkipIFormattable() ?? false;
      SkipToString = attribute.FindSkipToString() ?? false;
      SwitchMethods = attribute.FindSwitchMethods();
      MapMethods = attribute.FindMapMethods();
      ConversionToKeyMemberType = attribute.FindConversionToKeyMemberType() ?? ConversionOperatorsGeneration.Implicit;
      ConversionFromKeyMemberType = attribute.FindConversionFromKeyMemberType() ?? ConversionOperatorsGeneration.Explicit;
      SerializationFrameworks = attribute.FindSerializationFrameworks();

      // Comparison operators depend on the equality comparison operators
      if (ComparisonOperators > EqualityComparisonOperators)
         EqualityComparisonOperators = ComparisonOperators;
   }

   public override bool Equals(object? obj)
   {
      return obj is AllEnumSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(AllEnumSettings? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return KeyMemberAccessModifier == other.KeyMemberAccessModifier
             && KeyMemberKind == other.KeyMemberKind
             && KeyMemberName == other.KeyMemberName
             && IsValidatable == other.IsValidatable
             && SkipIComparable == other.SkipIComparable
             && SkipIParsable == other.SkipIParsable
             && ComparisonOperators == other.ComparisonOperators
             && EqualityComparisonOperators == other.EqualityComparisonOperators
             && SkipIFormattable == other.SkipIFormattable
             && SkipToString == other.SkipToString
             && SwitchMethods == other.SwitchMethods
             && MapMethods == other.MapMethods
             && ConversionToKeyMemberType == other.ConversionToKeyMemberType
             && ConversionFromKeyMemberType == other.ConversionFromKeyMemberType
             && SerializationFrameworks == other.SerializationFrameworks;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = (int)KeyMemberAccessModifier;
         hashCode = (hashCode * 397) ^ (int)KeyMemberKind;
         hashCode = (hashCode * 397) ^ KeyMemberName.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValidatable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)ComparisonOperators;
         hashCode = (hashCode * 397) ^ (int)EqualityComparisonOperators;
         hashCode = (hashCode * 397) ^ SkipIFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)SwitchMethods;
         hashCode = (hashCode * 397) ^ (int)MapMethods;
         hashCode = (hashCode * 397) ^ (int)ConversionToKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)ConversionFromKeyMemberType;
         hashCode = (hashCode * 397) ^ (int)SerializationFrameworks;

         return hashCode;
      }
   }
}
