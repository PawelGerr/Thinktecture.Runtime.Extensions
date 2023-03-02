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
      StringBasedTypeConverter = new ValueObjectTypeConverter<TestEnum, string>();
      IntBasedTypeConverter = new ValueObjectTypeConverter<IntegerEnum, int>();
      IntBasedStructEnumTypeConverter = new ValueObjectTypeConverter<StructIntegerEnum, int>();
      ValidEnumTypeConverter = new ValueObjectTypeConverter<ValidTestEnum, string>();
   }
}
