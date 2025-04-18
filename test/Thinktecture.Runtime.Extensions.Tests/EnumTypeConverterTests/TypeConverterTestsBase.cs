using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public abstract class TypeConverterTestsBase
{
   protected ThinktectureTypeConverter<TestEnum, string, ValidationError> StringBasedTypeConverter { get; }
   protected ThinktectureTypeConverter<IntegerEnum, int, ValidationError> IntBasedTypeConverter { get; }
   protected ThinktectureTypeConverter<StructIntegerEnum, int, ValidationError> IntBasedStructEnumTypeConverter { get; }
   protected ThinktectureTypeConverter<ValidTestEnum, string, ValidationError> ValidEnumTypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      StringBasedTypeConverter = new ThinktectureTypeConverter<TestEnum, string, ValidationError>();
      IntBasedTypeConverter = new ThinktectureTypeConverter<IntegerEnum, int, ValidationError>();
      IntBasedStructEnumTypeConverter = new ThinktectureTypeConverter<StructIntegerEnum, int, ValidationError>();
      ValidEnumTypeConverter = new ThinktectureTypeConverter<ValidTestEnum, string, ValidationError>();
   }
}
