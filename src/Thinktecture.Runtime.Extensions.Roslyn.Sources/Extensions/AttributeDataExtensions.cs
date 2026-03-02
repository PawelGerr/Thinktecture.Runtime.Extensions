using Thinktecture.CodeAnalysis;
using Thinktecture.Json;

namespace Thinktecture;

public static class AttributeDataExtensions
{
   public static string? FindDefaultInstancePropertyName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, Constants.Attributes.Properties.DEFAULT_INSTANCE_PROPERTY_NAME);
   }

   public static string? FindDelegateName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, Constants.Attributes.Properties.DELEGATE_NAME);
   }

   public static bool? FindSkipKeyMember(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_KEY_MEMBER);
   }

   public static bool? FindSkipFactoryMethods(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_FACTORY_METHODS);
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
      return GetStringParameterValue(attributeData, Constants.Attributes.Properties.CREATE_FACTORY_METHOD_NAME);
   }

   public static string? FindTryCreateFactoryMethodName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, Constants.Attributes.Properties.TRY_CREATE_FACTORY_METHOD_NAME);
   }

   public static bool? FindEmptyStringInFactoryMethodsYieldsNull(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.EMPTY_STRING_IN_FACTORY_METHODS_YIELDS_NULL);
   }

   public static bool? FindNullInFactoryMethodsYieldsNull(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.NULL_IN_FACTORY_METHODS_YIELDS_NULL);
   }

   public static bool? FindSkipIComparable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_ICOMPARABLE);
   }

   public static bool? FindSkipIParsable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_IPARSABLE);
   }

   public static bool? FindSkipISpanParsable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_ISPAN_PARSABLE);
   }

   public static bool? FindSkipIFormattable(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_IFORMATTABLE);
   }

   public static bool? FindSkipEqualityComparison(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_EQUALITY_COMPARISON);
   }

   public static OperatorsGeneration FindAdditionOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, Constants.Attributes.Properties.ADDITION_OPERATORS);
   }

   public static OperatorsGeneration FindSubtractionOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, Constants.Attributes.Properties.SUBTRACTION_OPERATORS);
   }

   public static OperatorsGeneration FindMultiplyOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, Constants.Attributes.Properties.MULTIPLY_OPERATORS);
   }

   public static OperatorsGeneration FindDivisionOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, Constants.Attributes.Properties.DIVISION_OPERATORS);
   }

   public static OperatorsGeneration FindComparisonOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, Constants.Attributes.Properties.COMPARISON_OPERATORS);
   }

   public static OperatorsGeneration FindEqualityComparisonOperators(this AttributeData attributeData)
   {
      return GetOperatorsGeneration(attributeData, Constants.Attributes.Properties.EQUALITY_COMPARISON_OPERATORS);
   }

   public static bool? FindSkipToString(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.SKIP_TO_STRING);
   }

   public static SwitchMapMethodsGeneration FindSwitchMethods(this AttributeData attributeData)
   {
      return GetSwitchMapGeneration(attributeData, Constants.Attributes.Properties.SWITCH_METHODS);
   }

   public static SwitchMapMethodsGeneration FindMapMethods(this AttributeData attributeData)
   {
      return GetSwitchMapGeneration(attributeData, Constants.Attributes.Properties.MAP_METHODS);
   }

   public static ConversionOperatorsGeneration? FindConversionFromValue(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, Constants.Attributes.Properties.CONVERSION_FROM_VALUE);
   }

   public static ConversionOperatorsGeneration? FindConversionToValue(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, Constants.Attributes.Properties.CONVERSION_TO_VALUE);
   }

   public static ConversionOperatorsGeneration? FindConversionToKeyMemberType(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, Constants.Attributes.Properties.CONVERSION_TO_KEY_MEMBER_TYPE);
   }

   public static ConversionOperatorsGeneration? FindUnsafeConversionToKeyMemberType(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, Constants.Attributes.Properties.UNSAFE_CONVERSION_TO_KEY_MEMBER_TYPE);
   }

   public static ConversionOperatorsGeneration? FindConversionFromKeyMemberType(this AttributeData attributeData)
   {
      return GetEnumParameterValue<ConversionOperatorsGeneration>(attributeData, Constants.Attributes.Properties.CONVERSION_FROM_KEY_MEMBER_TYPE);
   }

   public static SerializationFrameworks FindUseForSerialization(this AttributeData attributeData)
   {
      return GetEnumParameterValue<SerializationFrameworks>(attributeData, Constants.Attributes.Properties.USE_FOR_SERIALIZATION)
             ?? SerializationFrameworks.None;
   }

   public static SerializationFrameworks FindSerializationFrameworks(this AttributeData attributeData)
   {
      return GetEnumParameterValue<SerializationFrameworks>(attributeData, Constants.Attributes.Properties.SERIALIZATION_FRAMEWORKS)
             ?? SerializationFrameworks.All;
   }

   public static bool FindDisableSpanBasedJsonConversion(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.DISABLE_SPAN_BASED_JSON_CONVERSION) ?? false;
   }

   public static StringComparison FindDefaultStringComparison(this AttributeData attributeData)
   {
      return GetEnumParameterValue<StringComparison>(attributeData, Constants.Attributes.Properties.DEFAULT_STRING_COMPARISON)
             ?? StringComparison.OrdinalIgnoreCase;
   }

   public static bool HasDefaultStringComparison(this AttributeData attributeData)
   {
      return GetEnumParameterValue<StringComparison>(attributeData, "DefaultStringComparison") is not null;
   }

   public static bool FindTxIsNullableReferenceType(this AttributeData attributeData, int index)
   {
      var name = index switch
      {
         1 => "T1IsNullableReferenceType",
         2 => "T2IsNullableReferenceType",
         3 => "T3IsNullableReferenceType",
         4 => "T4IsNullableReferenceType",
         5 => "T5IsNullableReferenceType",
         _ => throw new ArgumentOutOfRangeException(nameof(index)),
      };

      return GetBooleanParameterValue(attributeData, name) ?? false;
   }

   public static bool FindTxIsStateless(this AttributeData attributeData, int index)
   {
      var name = index switch
      {
         1 => "T1IsStateless",
         2 => "T2IsStateless",
         3 => "T3IsStateless",
         4 => "T4IsStateless",
         5 => "T5IsStateless",
         _ => throw new ArgumentOutOfRangeException(nameof(index)),
      };

      return GetBooleanParameterValue(attributeData, name) ?? false;
   }

   public static bool FindUseWithEntityFramework(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.USE_WITH_ENTITY_FRAMEWORK) ?? false;
   }

   public static bool FindUseForModelBinding(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.USE_FOR_MODEL_BINDING) ?? false;
   }

   public static bool FindHasCorrespondingConstructor(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.HAS_CORRESPONDING_CONSTRUCTOR) ?? false;
   }

   public static string? FindTxName(this AttributeData attributeData, int index)
   {
      var name = index switch
      {
         1 => "T1Name",
         2 => "T2Name",
         3 => "T3Name",
         4 => "T4Name",
         5 => "T5Name",
         _ => throw new ArgumentOutOfRangeException(nameof(index)),
      };

      return GetStringParameterValue(attributeData, name);
   }

   public static string FindSwitchMapStateParameterName(this AttributeData attributeData)
   {
      return GetStringParameterValue(attributeData, Constants.Attributes.Properties.SWITCH_MAP_STATE_PARAMETER_NAME) ?? Constants.Parameters.STATE;
   }

   public static NestedUnionParameterNameGeneration FindNestedUnionParameterNameGeneration(
      this AttributeData attribute)
   {
      return GetEnumParameterValue<NestedUnionParameterNameGeneration>(attribute, Constants.Attributes.Properties.NESTED_UNION_PARAMETER_NAMES)
             ?? NestedUnionParameterNameGeneration.Default;
   }

   public static UnionConstructorAccessModifier FindUnionConstructorAccessModifier(this AttributeData attributeData)
   {
      return GetEnumParameterValue<UnionConstructorAccessModifier>(attributeData, Constants.Attributes.Properties.CONSTRUCTOR_ACCESS_MODIFIER)
             ?? UnionConstructorAccessModifier.Public;
   }

   public static bool? FindUseSingleBackingField(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.USE_SINGLE_BACKING_FIELD);
   }

   public static JsonIgnoreCondition? FindJsonIgnoreCondition(this AttributeData attributeData)
   {
      return GetEnumParameterValue<JsonIgnoreCondition>(attributeData, Constants.Attributes.Properties.CONDITION);
   }

   public static bool FindAllowDefaultStructs(this AttributeData attributeData)
   {
      return GetBooleanParameterValue(attributeData, Constants.Attributes.Properties.ALLOW_DEFAULT_STRUCTS) ?? false;
   }

   public static IReadOnlyList<string> FindUnionSwitchMapOverloadStopAtTypes(this AttributeData attributeData)
   {
      var stopAtArgument = attributeData.FindNamedAttributeValue(Constants.Attributes.Properties.STOP_AT);

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
            continue;

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

   private static string? GetStringParameterValue(AttributeData attributeData, string name)
   {
      if (attributeData.FindNamedAttributeValue(name).Value is not string value)
         return null;

      value = value.Trim();
      return value.Length == 0 ? null : value;
   }

   private static OperatorsGeneration GetOperatorsGeneration(AttributeData attributeData, string name)
   {
      return GetEnumParameterValue<OperatorsGeneration>(attributeData, name)
             ?? OperatorsGeneration.Default;
   }

   private static SwitchMapMethodsGeneration GetSwitchMapGeneration(AttributeData attributeData, string name)
   {
      return GetEnumParameterValue<SwitchMapMethodsGeneration>(attributeData, name)
             ?? SwitchMapMethodsGeneration.Default;
   }

   private static T? GetEnumParameterValue<T>(AttributeData attributeData, string name)
      where T : struct, Enum
   {
      var typedConstant = attributeData.FindNamedAttributeValue(name);

      // Roslyn boxes enum values using their underlying integral type
      return typedConstant.Value switch
      {
         int intValue => intValue.GetValidValue<T>(),
         byte byteValue => byteValue.GetValidValue<T>(),
         sbyte sbyteValue => sbyteValue.GetValidValue<T>(),
         short shortValue => shortValue.GetValidValue<T>(),
         ushort ushortValue => ushortValue.GetValidValue<T>(),
         uint uintValue => uintValue.GetValidValue<T>(),
         long longValue => longValue.GetValidValue<T>(),
         ulong ulongValue => ulongValue.GetValidValue<T>(),
         _ => null
      };
   }

   private static bool? GetBooleanParameterValue(AttributeData attributeData, string name)
   {
      return attributeData.FindNamedAttributeValue(name).Value as bool?;
   }

   private static TypedConstant FindNamedAttributeValue(this AttributeData attributeData, string name)
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
