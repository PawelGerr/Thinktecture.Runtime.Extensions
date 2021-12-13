using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public abstract class TypeConverterTestsBase
{
   protected ValueObjectTypeConverter<TestEnum, string> StringBasedTypeConverter { get; }
   protected ValueObjectTypeConverter<IntegerEnum, int> IntBasedTypeConverter { get; }
   protected ValueObjectTypeConverter<StructIntegerEnum, int> IntBasedStructEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<ValidTestEnum, string> ValidEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<ExtensibleTestValidatableEnum, string> ExtensibleTestValidatableEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<ExtendedTestValidatableEnum, string> ExtendedTestValidatableEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<ExtensibleTestEnum, string> ExtensibleTestEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<ExtendedTestEnum, string> ExtendedTestEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<DifferentAssemblyExtendedTestEnum, string> DifferentAssemblyExtendedTestEnumTypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      StringBasedTypeConverter = new TestEnum_EnumTypeConverter();
      IntBasedTypeConverter = new IntegerEnum_EnumTypeConverter();
      IntBasedStructEnumTypeConverter = new StructIntegerEnum_EnumTypeConverter();
      ValidEnumTypeConverter = new ValidTestEnum_EnumTypeConverter();
      ExtensibleTestValidatableEnumTypeConverter = new ExtensibleTestValidatableEnum_EnumTypeConverter();
      ExtendedTestValidatableEnumTypeConverter = new ExtendedTestValidatableEnum_EnumTypeConverter();
      ExtensibleTestEnumTypeConverter = new ExtensibleTestEnum_EnumTypeConverter();
      ExtendedTestEnumTypeConverter = new ExtendedTestEnum_EnumTypeConverter();
      DifferentAssemblyExtendedTestEnumTypeConverter = new DifferentAssemblyExtendedTestEnum_EnumTypeConverter();
   }
}