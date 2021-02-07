using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests
{
   public abstract class TypeConverterTestsBase
   {
      protected ValueTypeConverter<TestEnum, string> StringBasedConverter { get; }
      protected ValueTypeConverter<IntegerEnum, int> IntBasedConverter { get; }
      protected ValueTypeConverter<StructIntegerEnum, int> IntBasedStructEnumConverter { get; }
      protected ValueTypeConverter<ValidTestEnum, string> ValidEnumConverter { get; }
      protected ValueTypeConverter<ExtensibleTestValidatableEnum, string> ExtensibleTestValidatableEnumConverter { get; }
      protected ValueTypeConverter<ExtendedTestValidatableEnum, string> ExtendedTestValidatableEnumConverter { get; }
      protected ValueTypeConverter<ExtensibleTestEnum, string> ExtensibleTestEnumConverter { get; }
      protected ValueTypeConverter<ExtendedTestEnum, string> ExtendedTestEnumConverter { get; }
      protected ValueTypeConverter<DifferentAssemblyExtendedTestEnum, string> DifferentAssemblyExtendedTestEnumConverter { get; }

      protected TypeConverterTestsBase()
      {
         StringBasedConverter = new TestEnum_EnumTypeConverter();
         IntBasedConverter = new IntegerEnum_EnumTypeConverter();
         IntBasedStructEnumConverter = new StructIntegerEnum_EnumTypeConverter();
         ValidEnumConverter = new ValidTestEnum_EnumTypeConverter();
         ExtensibleTestValidatableEnumConverter = new ExtensibleTestValidatableEnum_EnumTypeConverter();
         ExtendedTestValidatableEnumConverter = new ExtendedTestValidatableEnum_EnumTypeConverter();
         ExtensibleTestEnumConverter = new ExtensibleTestEnum_EnumTypeConverter();
         ExtendedTestEnumConverter = new ExtendedTestEnum_EnumTypeConverter();
         DifferentAssemblyExtendedTestEnumConverter = new DifferentAssemblyExtendedTestEnum_EnumTypeConverter();
      }
   }
}
