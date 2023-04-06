namespace Thinktecture.CodeAnalysis;

public static class Constants
{
   public const string KEY_EQUALITY_COMPARER_NAME = "KeyEqualityComparer";

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
      public const string VALUE_OBJECT = "Thinktecture.ValueObjectAttribute";
   }
}
