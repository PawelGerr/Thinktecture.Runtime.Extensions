using Thinktecture.TestEnums;

namespace Thinktecture.EnumTypeConverterTests
{
   public abstract class TypeConverterTestsBase
   {
      protected EnumTypeConverter<TestEnum, string> StringBasedConverter { get; }
      protected EnumTypeConverter<IntegerEnum, int> IntBasedConverter { get; }
      protected EnumTypeConverter<StructIntegerEnum, int> IntBasedStructEnumConverter { get; }

      protected TypeConverterTestsBase()
      {
         StringBasedConverter = new TestEnum_EnumTypeConverter();
         IntBasedConverter = new IntegerEnum_EnumTypeConverter();
         IntBasedStructEnumConverter = new StructIntegerEnum_EnumTypeConverter();
      }
   }
}
