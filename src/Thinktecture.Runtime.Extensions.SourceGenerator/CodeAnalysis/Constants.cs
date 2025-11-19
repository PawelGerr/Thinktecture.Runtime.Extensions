namespace Thinktecture.CodeAnalysis;

public static class Constants
{
   public static class SmartEnum
   {
      public const AccessModifier DEFAULT_KEY_MEMBER_ACCESS_MODIFIER = AccessModifier.Public;
      public const MemberKind DEFAULT_KEY_MEMBER_KIND = MemberKind.Property;
   }

   public static class ValueObject
   {
      public const AccessModifier DEFAULT_KEY_MEMBER_ACCESS_MODIFIER = AccessModifier.Private;
      public const MemberKind DEFAULT_KEY_MEMBER_KIND = MemberKind.Field;
      public const AccessModifier DEFAULT_CONSTRUCTOR_ACCESS_MODIFIER = AccessModifier.Private;
   }

   public static class ValidationError
   {
      public const string NAME = "ValidationError";
      public const string NAMESPACE = "Thinktecture";
      public const string FULL_NAME = $"{NAMESPACE}.{NAME}";
   }

   public static class Modules
   {
      public const string THINKTECTURE_RUNTIME_EXTENSIONS_JSON = "Thinktecture.Runtime.Extensions.Json.dll";
      public const string THINKTECTURE_RUNTIME_EXTENSIONS_NEWTONSOFT_JSON = "Thinktecture.Runtime.Extensions.Newtonsoft.Json.dll";
      public const string THINKTECTURE_RUNTIME_EXTENSIONS_MESSAGEPACK = "Thinktecture.Runtime.Extensions.MessagePack.dll";
   }

   public static class ComparerAccessor
   {
      public const string ORDINAL_IGNORE_CASE = "global::Thinktecture.ComparerAccessors.StringOrdinalIgnoreCase";
   }

   public static class Methods
   {
      public const string VALIDATE_FACTORY_ARGUMENTS = "ValidateFactoryArguments";
      public const string GET = "Get";
   }

   public static class Configuration
   {
      public const string COUNTER = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_Counter";
      public const string GENERATE_JETBRAINS_ANNOTATIONS = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_GenerateJetBrainsAnnotations";
      public const string LOG_FILE_PATH = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogFilePath";
      public const string LOG_FILE_PATH_UNIQUE = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogFilePathMustBeUnique";
      public const string LOG_LEVEL = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogLevel";
      public const string LOG_INITIAL_BUFFER_SIZE = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogMessageInitialBufferSize";
   }

   public static class Interfaces
   {
      public static class IDisallowDefaultValue
      {
         public const string NAME = "IDisallowDefaultValue";
         public const string NAMESPACE = "Thinktecture";
      }

      public static class ObjectFactory
      {
         public const string NAME = "IObjectFactory";
         public const string NAMESPACE = "Thinktecture";
      }

      public static class Convertible
      {
         public const string NAME = "IConvertible";
         public const string NAMESPACE = "Thinktecture";
      }
   }

   public static class Attributes
   {
      public const string NAMESPACE = "Thinktecture";

      public static class Properties
      {
         public const string ADDITION_OPERATORS = "AdditionOperators";
         public const string ALLOW_DEFAULT_STRUCTS = "AllowDefaultStructs";
         public const string COMPARISON_OPERATORS = "ComparisonOperators";
         public const string CONDITION = "Condition";
         public const string CONSTRUCTOR_ACCESS_MODIFIER = "ConstructorAccessModifier";
         public const string CONVERSION_FROM_KEY_MEMBER_TYPE = "ConversionFromKeyMemberType";
         public const string CONVERSION_FROM_VALUE = "ConversionFromValue";
         public const string CONVERSION_TO_KEY_MEMBER_TYPE = "ConversionToKeyMemberType";
         public const string CONVERSION_TO_VALUE = "ConversionToValue";
         public const string CREATE_FACTORY_METHOD_NAME = "CreateFactoryMethodName";
         public const string DEFAULT_INSTANCE_PROPERTY_NAME = "DefaultInstancePropertyName";
         public const string DEFAULT_STRING_COMPARISON = "DefaultStringComparison";
         public const string DELEGATE_NAME = "DelegateName";
         public const string DIVISION_OPERATORS = "DivisionOperators";
         public const string EMPTY_STRING_IN_FACTORY_METHODS_YIELDS_NULL = "EmptyStringInFactoryMethodsYieldsNull";
         public const string EQUALITY_COMPARISON_OPERATORS = "EqualityComparisonOperators";
         public const string HAS_CORRESPONDING_CONSTRUCTOR = "HasCorrespondingConstructor";
         public const string KEY_MEMBER_ACCESS_MODIFIER = "KeyMemberAccessModifier";
         public const string KEY_MEMBER_KIND = "KeyMemberKind";
         public const string KEY_MEMBER_NAME = "KeyMemberName";
         public const string MAP_METHODS = "MapMethods";
         public const string MULTIPLY_OPERATORS = "MultiplyOperators";
         public const string NULL_IN_FACTORY_METHODS_YIELDS_NULL = "NullInFactoryMethodsYieldsNull";
         public const string SERIALIZATION_FRAMEWORKS = "SerializationFrameworks";
         public const string SKIP_FACTORY_METHODS = "SkipFactoryMethods";
         public const string SKIP_ICOMPARABLE = "SkipIComparable";
         public const string SKIP_EQUALITY_COMPARISON = "SkipEqualityComparison";
         public const string SKIP_IFORMATTABLE = "SkipIFormattable";
         public const string SKIP_IPARSABLE = "SkipIParsable";
         public const string SKIP_KEY_MEMBER = "SkipKeyMember";
         public const string SKIP_TO_STRING = "SkipToString";
         public const string STOP_AT = "StopAt";
         public const string SUBTRACTION_OPERATORS = "SubtractionOperators";
         public const string SWITCH_MAP_STATE_PARAMETER_NAME = "SwitchMapStateParameterName";
         public const string SWITCH_METHODS = "SwitchMethods";
         public const string TRY_CREATE_FACTORY_METHOD_NAME = "TryCreateFactoryMethodName";
         public const string UNSAFE_CONVERSION_TO_KEY_MEMBER_TYPE = "UnsafeConversionToKeyMemberType";
         public const string USE_FOR_MODEL_BINDING = "UseForModelBinding";
         public const string USE_FOR_SERIALIZATION = "UseForSerialization";
         public const string USE_SINGLE_BACKING_FIELD = "UseSingleBackingField";
         public const string USE_WITH_ENTITY_FRAMEWORK = "UseWithEntityFramework";
      }

      public static class ValueObject
      {
         public const string KEYED_NAME = "ValueObjectAttribute";
         public const string COMPLEX_NAME = "ComplexValueObjectAttribute";
         public const string KEYED_FULL_NAME = $"{NAMESPACE}.{KEYED_NAME}`1";
         public const string COMPLEX_FULL_NAME = $"{NAMESPACE}.{COMPLEX_NAME}";
      }

      public static class SmartEnum
      {
         public const string NAME = "SmartEnumAttribute";
         public const string KEYED_FULL_NAME = $"{NAMESPACE}.{NAME}`1";
         public const string KEYLESS_FULL_NAME = $"{NAMESPACE}.{NAME}";
      }

      public static class UseDelegateFromConstructor
      {
         public const string NAME = "UseDelegateFromConstructorAttribute";
      }

      public static class ObjectFactory
      {
         public const string NAME = "ObjectFactoryAttribute";
         public const string FULL_NAME = $"{NAMESPACE}.{NAME}`1";
      }

      public static class Union
      {
         public const string NAME_AD_HOC = "AdHocUnionAttribute";
         public const string FULL_NAME_AD_HOC = $"{NAMESPACE}.{NAME_AD_HOC}";

         public const string NAME = "UnionAttribute";
         public const string FULL_NAME = $"{NAMESPACE}.{NAME}";
         public const string FULL_NAME_2_TYPES = $"{NAMESPACE}.{NAME}`2";
         public const string FULL_NAME_3_TYPES = $"{NAMESPACE}.{NAME}`3";
         public const string FULL_NAME_4_TYPES = $"{NAMESPACE}.{NAME}`4";
         public const string FULL_NAME_5_TYPES = $"{NAMESPACE}.{NAME}`5";
      }

      public static class UnionSwitchMapOverload
      {
         public const string NAME = "UnionSwitchMapOverloadAttribute";
         public const string FULL_NAME = $"{NAMESPACE}.{NAME}";
      }
   }
}
