using System.Runtime.CompilerServices;
using Thinktecture.CodeAnalysis;
using Thinktecture.Json;

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
      return GetEnumParameterValue<AccessModifier>(attributeData, Constants.Attributes.Properties.KEY_MEMBER_ACCESS_MODIFIER);
   }

   public static AccessModifier? FindConstructorAccessModifier(this AttributeData attributeData)
   {
      return GetEnumParameterValue<AccessModifier>(attributeData, Constants.Attributes.Properties.CONSTRUCTOR_ACCESS_MODIFIER);
   }

   public static MemberKind? FindKeyMemberKind(this AttributeData attributeData)
   {
      return GetEnumParameterValue<MemberKind>(attributeData, Constants.Attributes.Properties.KEY_MEMBER_KIND);
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

   public static bool? FindSkipEqualityComparison(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "SkipEqualityComparison");
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
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, "ConversionFromValue");
   }

   public static ConversionOperatorsGeneration? FindConversionToValue(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, "ConversionToValue");
   }

   public static ConversionOperatorsGeneration? FindConversionToKeyMemberType(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, "ConversionToKeyMemberType");
   }

   public static ConversionOperatorsGeneration? FindUnsafeConversionToKeyMemberType(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, "UnsafeConversionToKeyMemberType");
   }

   public static ConversionOperatorsGeneration? FindConversionFromKeyMemberType(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, "ConversionFromKeyMemberType");
   }

   public static SerializationFrameworks FindUseForSerialization(this AttributeData attributeData)
   {
      return GetEnumParameterValue<SerializationFrameworks>(attributeData, "UseForSerialization")
             ?? SerializationFrameworks.None;
   }

   public static SerializationFrameworks FindSerializationFrameworks(this AttributeData attributeData)
   {
      return GetEnumParameterValue<SerializationFrameworks>(attributeData, "SerializationFrameworks")
             ?? SerializationFrameworks.All;
   }

   public static StringComparison FindDefaultStringComparison(this AttributeData attributeData)
   {
      return GetEnumParameterValue<StringComparison>(attributeData, "DefaultStringComparison")
             ?? StringComparison.OrdinalIgnoreCase;
   }

   public static bool HasDefaultStringComparison(this AttributeData attributeData)
   {
      return GetEnumParameterValue<StringComparison>(attributeData, "DefaultStringComparison") is not null;
   }

   public static bool FindTxIsNullableReferenceType(this AttributeData attributeData, int index)
   {
      return GetBooleanParameterValue(attributeData, $"T{index}IsNullableReferenceType") ?? false;
   }

   public static bool FindUseWithEntityFramework(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "UseWithEntityFramework") ?? false;
   }

   public static bool FindUseForModelBinding(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "UseForModelBinding") ?? false;
   }

   public static bool FindHasCorrespondingConstructor(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.HAS_CORRESPONDING_CONSTRUCTOR) ?? false;
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
      return GetEnumParameterValue<UnionConstructorAccessModifier>(attributeData, "ConstructorAccessModifier")
             ?? UnionConstructorAccessModifier.Public;
   }

   public static bool? FindUseSingleBackingField(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, "UseSingleBackingField");
   }

   public static JsonIgnoreCondition? FindJsonIgnoreCondition(this AttributeData attributeData)
   {
      return GetEnumParameterValue<JsonIgnoreCondition>(attributeData, "Condition");
   }

   public static bool FindAllowDefaultStructs(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.ALLOW_DEFAULT_STRUCTS) ?? false;
   }

   public static IReadOnlyList<string> FindUnionSwitchMapOverloadStopAtTypes(this AttributeData attributeData)
   {
      var stopAtArgument = attributeData.FindNamedAttribute("StopAt");

      if (stopAtArgument.Kind != TypedConstantKind.Array)
         return [];

      var values = stopAtArgument.Values;
      var length = values.Length;

      if (length == 0)
         return [];

      var result = new List<string>(length);

      for (var i = 0; i < length; i++)
      {
         var value = values[i];

         if (value.Value is not ITypeSymbol typeSymbol)
            continue;

         if (typeSymbol.TypeKind == TypeKind.Error)
            return [];

         result.Add(typeSymbol.ToFullyQualifiedDisplayString());
      }

      return result;
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

   public static bool NeedsToValueMethod(this AttributeData objectFactoryAttributeData)
   {
      var useForSerialization = objectFactoryAttributeData.FindUseForSerialization();
      var useWithEntityFramework = objectFactoryAttributeData.FindUseWithEntityFramework();

      return useForSerialization != SerializationFrameworks.None || useWithEntityFramework;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static string? GetStringParameterValue(AttributeData attributeData, string name)
   {
      if (attributeData.FindNamedAttribute(name).Value is not string value)
         return null;

      value = value.Trim();
      return value.Length == 0 ? null : value;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static OperatorsGeneration GetOperatorsGeneration(AttributeData attributeData, string name)
   {
      return GetEnumParameterValue<OperatorsGeneration>(attributeData, name)
             ?? OperatorsGeneration.Default;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static SwitchMapMethodsGeneration GetSwitchMapGeneration(AttributeData attributeData, string name)
   {
      return GetEnumParameterValue<SwitchMapMethodsGeneration>(attributeData, name)
             ?? SwitchMapMethodsGeneration.Default;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static T? GetEnumParameterValue<T>(AttributeData attributeData, string name)
      where T : struct, Enum
   {
      return attributeData.FindNamedAttribute(name).Value is int value ? value.GetValidValue<T>() : null;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static bool? GetBooleanParameterValue(AttributeData attributeData, string name)
   {
      return attributeData.FindNamedAttribute(name).Value as bool?;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private static TypedConstant FindNamedAttribute(this AttributeData attributeData, string name)
   {
      for (var i = 0; i < attributeData.NamedArguments.Length; i++)
      {
         var arg = attributeData.NamedArguments[i];

         if (arg.Key == name)
            return arg.Value;
      }

      return default;
   }
}
