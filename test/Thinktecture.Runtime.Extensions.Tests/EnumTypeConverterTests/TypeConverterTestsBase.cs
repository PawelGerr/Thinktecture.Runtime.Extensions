using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests
{
   public abstract class TypeConverterTestsBase
   {
      protected ValueTypeConverter<TestEnum, string> StringBasedConverter { get; }
      protected ValueTypeConverter<IntegerEnum, int> IntBasedConverter { get; }
      protected ValueTypeConverter<StructIntegerEnum, int> IntBasedStructEnumConverter { get; }
      protected ValueTypeConverter<ValidTestEnum, string> ValidEnumConverter { get; }

      protected TypeConverterTestsBase()
      {
         StringBasedConverter = new TestEnum_EnumTypeConverter();
         IntBasedConverter = new IntegerEnum_EnumTypeConverter();
         IntBasedStructEnumConverter = new StructIntegerEnum_EnumTypeConverter();
         ValidEnumConverter = new ValidTestEnum_EnumTypeConverter();
      }
   }
}
