using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public abstract class TypeConverterTestsBase
{
   protected ValueObjectTypeConverter<TestEnum, string, ValidationError> StringBasedTypeConverter { get; }
   protected ValueObjectTypeConverter<IntegerEnum, int, ValidationError> IntBasedTypeConverter { get; }
   protected ValueObjectTypeConverter<StructIntegerEnum, int, ValidationError> IntBasedStructEnumTypeConverter { get; }
   protected ValueObjectTypeConverter<ValidTestEnum, string, ValidationError> ValidEnumTypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      StringBasedTypeConverter = new ValueObjectTypeConverter<TestEnum, string, ValidationError>();
      IntBasedTypeConverter = new ValueObjectTypeConverter<IntegerEnum, int, ValidationError>();
      IntBasedStructEnumTypeConverter = new ValueObjectTypeConverter<StructIntegerEnum, int, ValidationError>();
      ValidEnumTypeConverter = new ValueObjectTypeConverter<ValidTestEnum, string, ValidationError>();
   }
}
