namespace Thinktecture.CodeAnalysis;

public static class Constants
{
   public static class SmartEnum
   {
      public const ValueObjectAccessModifier DEFAULT_KEY_MEMBER_ACCESS_MODIFIER = ValueObjectAccessModifier.Public;
      public const ValueObjectMemberKind DEFAULT_KEY_MEMBER_KIND = ValueObjectMemberKind.Property;
   }

   public static class ValueObject
   {
      public const ValueObjectAccessModifier DEFAULT_KEY_MEMBER_ACCESS_MODIFIER = ValueObjectAccessModifier.Private;
      public const ValueObjectMemberKind DEFAULT_KEY_MEMBER_KIND = ValueObjectMemberKind.Field;
      public const ValueObjectAccessModifier DEFAULT_CONSTRUCTOR_ACCESS_MODIFIER = ValueObjectAccessModifier.Private;
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
      public const string CREATE_INVALID_ITEM = "CreateInvalidItem";
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

   public static class Attributes
   {
      public static class Properties
      {
         public const string KEY_MEMBER_NAME = "KeyMemberName";
         public const string SKIP_KEY_MEMBER = "SkipKeyMember";
         public const string KEY_MEMBER_ACCESS_MODIFIER = "KeyMemberAccessModifier";
         public const string KEY_MEMBER_KIND = "KeyMemberKind";
         public const string CONSTRUCTOR_ACCESS_MODIFIER = "ConstructorAccessModifier";
      }

      public static class ValueObject
      {
         public const string NAMESPACE = "Thinktecture";
         public const string KEYED_NAME = "ValueObjectAttribute";
         public const string COMPLEX_NAME = "ComplexValueObjectAttribute";
         public const string KEYED_FULL_NAME = "Thinktecture.ValueObjectAttribute`1";
         public const string COMPLEX_FULL_NAME = "Thinktecture.ComplexValueObjectAttribute";
      }

      public static class SmartEnum
      {
         public const string NAMESPACE = "Thinktecture";
         public const string NAME = "SmartEnumAttribute";
         public const string KEYED_FULL_NAME = "Thinktecture.SmartEnumAttribute`1";
         public const string KEYLESS_FULL_NAME = "Thinktecture.SmartEnumAttribute";
      }

      public static class Union
      {
         public const string NAMESPACE = "Thinktecture";
         public const string NAME = "UnionAttribute";
         public const string FULL_NAME = "Thinktecture.UnionAttribute";
         public const string FULL_NAME_2_TYPES = "Thinktecture.UnionAttribute`2";
         public const string FULL_NAME_3_TYPES = "Thinktecture.UnionAttribute`3";
         public const string FULL_NAME_4_TYPES = "Thinktecture.UnionAttribute`4";
         public const string FULL_NAME_5_TYPES = "Thinktecture.UnionAttribute`5";
      }
   }
}
