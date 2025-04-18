using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class AttributeDataExtensions
{
   public static string? FindDefaultInstancePropertyName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, "DefaultInstancePropertyName");
   }

   public static string? FindDelegateName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, "DelegateName");
   }

   public static bool? FindSkipKeyMember(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_KEY_MEMBER);
   }

   public static bool? FindSkipFactoryMethods(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipFactoryMethods");
   }

   public static AccessModifier? FindKeyMemberAccessModifier(this AttributeData attributeData)
   {
      return attributeData.FindAccessModifier(Constants.Attributes.Properties.KEY_MEMBER_ACCESS_MODIFIER);
   }

   public static AccessModifier? FindConstructorAccessModifier(this AttributeData attributeData)
   {
      return attributeData.FindAccessModifier(Constants.Attributes.Properties.CONSTRUCTOR_ACCESS_MODIFIER);
   }

   private static AccessModifier? FindAccessModifier(this AttributeData attributeData, string propertyName)
   {
      var modifier = (AccessModifier?)GetIntegerParameterValue(attributeData, propertyName);

      if (modifier is null || !modifier.Value.IsValid())
         return null;

      return modifier;
   }

   public static MemberKind? FindKeyMemberKind(this AttributeData attributeData)
   {
      var kind = (MemberKind?)GetIntegerParameterValue(attributeData, Constants.Attributes.Properties.KEY_MEMBER_KIND);

      if (kind is null || !kind.Value.IsValid())
         return null;

      return kind;
   }

   public static string? FindKeyMemberName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, Constants.Attributes.Properties.KEY_MEMBER_NAME);
   }

   public static string? FindCreateFactoryMethodName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, "CreateFactoryMethodName");
   }

   public static string? FindTryCreateFactoryMethodName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, "TryCreateFactoryMethodName");
   }

   public static bool? FindEmptyStringInFactoryMethodsYieldsNull(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "EmptyStringInFactoryMethodsYieldsNull");
   }

   public static bool? FindNullInFactoryMethodsYieldsNull(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "NullInFactoryMethodsYieldsNull");
   }

   public static bool? FindIsValidatable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.IS_VALIDATABLE);
   }

   public static bool? FindSkipIComparable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_ICOMPARABLE);
   }

   public static bool? FindSkipIParsable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipIParsable");
   }

   public static bool? FindSkipIFormattable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipIFormattable");
   }

   public static OperatorsGeneration FindAdditionOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, "AdditionOperators");
   }

   public static OperatorsGeneration FindSubtractionOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, "SubtractionOperators");
   }

   public static OperatorsGeneration FindMultiplyOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, "MultiplyOperators");
   }

   public static OperatorsGeneration FindDivisionOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, "DivisionOperators");
   }

   public static OperatorsGeneration FindComparisonOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, "ComparisonOperators");
   }

   public static OperatorsGeneration FindEqualityComparisonOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, "EqualityComparisonOperators");
   }

   public static bool? FindSkipToString(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipToString");
   }

   public static SwitchMapMethodsGeneration FindSwitchMethods(this AttributeData attributeData)
   {
      return GetSwitchMapGeneration(attributeData, "SwitchMethods");
   }

   public static SwitchMapMethodsGeneration FindMapMethods(this AttributeData attributeData)
   {
      return GetSwitchMapGeneration(attributeData, "MapMethods");
   }

   public static ConversionOperatorsGeneration? FindConversionFromValue(this AttributeData attributeData)
   {
      return GetConversionOperatorsGeneration(attributeData, "ConversionFromValue");
   }

   public static ConversionOperatorsGeneration? FindConversionToValue(this AttributeData attributeData)
   {
      return GetConversionOperatorsGeneration(attributeData, "ConversionToValue");
   }

   public static ConversionOperatorsGeneration? FindConversionToKeyMemberType(this AttributeData attributeData)
   {
      return GetConversionOperatorsGeneration(attributeData, "ConversionToKeyMemberType");
   }

   public static ConversionOperatorsGeneration? FindUnsafeConversionToKeyMemberType(this AttributeData attributeData)
   {
      return GetConversionOperatorsGeneration(attributeData, "UnsafeConversionToKeyMemberType");
   }

   public static ConversionOperatorsGeneration? FindConversionFromKeyMemberType(this AttributeData attributeData)
   {
      return GetConversionOperatorsGeneration(attributeData, "ConversionFromKeyMemberType");
   }

   public static SerializationFrameworks FindUseForSerialization(this AttributeData attributeData)
   {
      var frameworks = (SerializationFrameworks?)GetIntegerParameterValue(attributeData, "UseForSerialization");

      if (frameworks is null || !frameworks.Value.IsValid())
         return SerializationFrameworks.None;

      return frameworks.Value;
   }

   public static SerializationFrameworks FindSerializationFrameworks(this AttributeData attributeData)
   {
      var frameworks = (SerializationFrameworks?)GetIntegerParameterValue(attributeData, "SerializationFrameworks");

      if (frameworks is null || !frameworks.Value.IsValid())
         return SerializationFrameworks.All;

      return frameworks.Value;
   }

   public static StringComparison FindDefaultStringComparison(this AttributeData attributeData)
   {
      var defaultStringComparison = (StringComparison?)GetIntegerParameterValue(attributeData, "DefaultStringComparison");

      if (defaultStringComparison is null || !defaultStringComparison.Value.IsValid())
         return StringComparison.OrdinalIgnoreCase;

      return defaultStringComparison.Value;
   }

   public static bool FindTxIsNullableReferenceType(this AttributeData attributeData, int index)
   {
      return GetBooleanParameterValue(attributeData, $"T{index}IsNullableReferenceType") ?? false;
   }

   public static string? FindTxName(this AttributeData attributeData, int index)
   {
      return GetStringParameterValue(attributeData, $"T{index}Name");
   }

   public static string FindSwitchMapStateParameterName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, "SwitchMapStateParameterName") ?? "state";
   }

   public static UnionConstructorAccessModifier FindUnionConstructorAccessModifier(this AttributeData attributeData)
   {
      return (UnionConstructorAccessModifier?)GetIntegerParameterValue(attributeData, "ConstructorAccessModifier")
             ?? UnionConstructorAccessModifier.Public;
   }

   public static bool FindAllowDefaultStructs(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.ALLOW_DEFAULT_STRUCTS) ?? false;
   }

   public static (ITypeSymbol ComparerType, ITypeSymbol ItemType)? GetComparerTypes(this AttributeData attributeData)
   {
      if (attributeData.AttributeClass is not { } attributeClass || attributeClass.TypeKind == TypeKind.Error)
         return null;

      var typeArguments = attributeClass.TypeArguments;

      if (typeArguments.IsDefaultOrEmpty || typeArguments.Length != 2)
         return null;

      var comparerAccessorTypes = typeArguments[0];
      var keyType = typeArguments[1];

      if (comparerAccessorTypes.TypeKind == TypeKind.Error || keyType.TypeKind == TypeKind.Error)
         return null;

      return (comparerAccessorTypes, keyType);
   }

   private static string? GetStringParameterValue(AttributeData attributeData, string name)
   {
      var value = (string?)attributeData.FindNamedAttribute(name).Value;

      return String.IsNullOrWhiteSpace(value) ? null : value?.Trim();
   }

   private static OperatorsGeneration GetOperatorsGeneration(AttributeData attributeData, string name)
   {
      var generation = (OperatorsGeneration?)GetIntegerParameterValue(attributeData, name);

      if (generation is null || !generation.Value.IsValid())
         return OperatorsGeneration.Default;

      return generation.Value;
   }

   private static SwitchMapMethodsGeneration GetSwitchMapGeneration(AttributeData attributeData, string name)
   {
      var generation = (SwitchMapMethodsGeneration?)GetIntegerParameterValue(attributeData, name);

      if (generation is null || !generation.Value.IsValid())
         return SwitchMapMethodsGeneration.Default;

      return generation.Value;
   }

   private static ConversionOperatorsGeneration? GetConversionOperatorsGeneration(AttributeData attributeData, string name)
   {
      var generation = (ConversionOperatorsGeneration?)GetIntegerParameterValue(attributeData, name);

      if (generation is null || !generation.Value.IsValid())
         return null;

      return generation.Value;
   }

   private static int? GetIntegerParameterValue(AttributeData attributeData, string name)
   {
      return (int?)attributeData.FindNamedAttribute(name).Value;
   }

   private static bool? GetBooleanParameterValue(AttributeData attributeData, string name)
   {
      return (bool?)attributeData.FindNamedAttribute(name).Value;
   }

   private static TypedConstant FindNamedAttribute(this AttributeData attributeData, string name)
   {
      return attributeData.NamedArguments.FirstOrDefault(static (a, n) => a.Key == n, name).Value;
   }
}
