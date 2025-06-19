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
      public const string LOG_FILE_PATH = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogFilePath";
      public const string LOG_FILE_PATH_UNIQUE = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogFilePathMustBeUnique";
      public const string LOG_LEVEL = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogLevel";
      public const string LOG_INITIAL_BUFFER_SIZE = "build_property.ThinktectureRuntimeExtensions_SourceGenerator_LogMessageInitialBufferSize";
   }

   public static class Interfaces
   {
      public static class DisallowDefaultStructs
      {
         public const string NAME = "IDisallowDefaultValue";
         public const string NAMESPACE = "Thinktecture";
      }
   }

   public static class Attributes
   {
      public const string NAMESPACE = "Thinktecture";

      public static class Properties
      {
         public const string KEY_MEMBER_NAME = "KeyMemberName";
         public const string SKIP_KEY_MEMBER = "SkipKeyMember";
         public const string KEY_MEMBER_ACCESS_MODIFIER = "KeyMemberAccessModifier";
         public const string KEY_MEMBER_KIND = "KeyMemberKind";
         public const string CONSTRUCTOR_ACCESS_MODIFIER = "ConstructorAccessModifier";
         public const string DEFAULT_STRING_COMPARISON = "DefaultStringComparison";
         public const string SKIP_ICOMPARABLE = "SkipIComparable";
         public const string ALLOW_DEFAULT_STRUCTS = "AllowDefaultStructs";
         public const string HAS_CORRESPONDING_CONSTRUCTOR = "HasCorrespondingConstructor";
      }

      public static class ValueObject
      {
         public const string KEYED_NAME = "ValueObjectAttribute";
         public const string COMPLEX_NAME = "ComplexValueObjectAttribute";
         public const string KEYED_FULL_NAME = "Thinktecture.ValueObjectAttribute`1";
         public const string COMPLEX_FULL_NAME = "Thinktecture.ComplexValueObjectAttribute";
      }

      public static class SmartEnum
      {
         public const string NAME = "SmartEnumAttribute";
         public const string KEYED_FULL_NAME = "Thinktecture.SmartEnumAttribute`1";
         public const string KEYLESS_FULL_NAME = "Thinktecture.SmartEnumAttribute";
      }

      public static class UseDelegateFromConstructor
      {
         public const string NAME = "UseDelegateFromConstructorAttribute";
      }

      public static class ObjectFactory
      {
         public const string NAME = "ObjectFactoryAttribute";
         public const string NAME_OBSOLETE = "ValueObjectFactoryAttribute";
         public const string FULL_NAME = $"{NAMESPACE}.{NAME}`1";
         public const string FULL_NAME_OBSOLETE = $"{NAMESPACE}.{NAME_OBSOLETE}`1";
      }

      public static class Union
      {
         public const string NAME = "UnionAttribute";
         public const string FULL_NAME = "Thinktecture.UnionAttribute";
         public const string FULL_NAME_2_TYPES = "Thinktecture.UnionAttribute`2";
         public const string FULL_NAME_3_TYPES = "Thinktecture.UnionAttribute`3";
         public const string FULL_NAME_4_TYPES = "Thinktecture.UnionAttribute`4";
         public const string FULL_NAME_5_TYPES = "Thinktecture.UnionAttribute`5";
      }
   }
}
