using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public abstract class TypeConverterTestsBase
{
   protected ValueObjectTypeConverter<TestEnum, string> StringBasedTypeConverter { get; }
   protected ValueObjectTypeConverter<IntegerEnum, int> IntBasedTypeConverter { get; }
   protected ValueObjectTypeConverter<StructIntegerEnum, int> IntBasedStructEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<ValidTestEnum, string> ValidEnumTypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      StringBasedTypeConverter = new TestEnum_EnumTypeConverter();
      IntBasedTypeConverter = new IntegerEnum_EnumTypeConverter();
      IntBasedStructEnumTypeConverter = new StructIntegerEnum_EnumTypeConverter();
      ValidEnumTypeConverter = new ValidTestEnum_EnumTypeConverter();
   }
}
