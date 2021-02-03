using Thinktecture.Runtime.Tests.TestValueTypes;

namespace Thinktecture.Runtime.Tests.ValueTypeTypeConverterTests
{
   public abstract class TypeConverterTestsBase
   {
      protected ValueTypeConverter<IntBasedReferenceValueType, int> IntBasedReferenceValueTypeConverter { get; }
      protected ValueTypeConverter<IntBasedStructValueType, int> IntBasedStructValueTypeConverter { get; }
      protected ValueTypeConverter<StringBasedReferenceValueType, string> StringBasedReferenceValueTypeConverter { get; }
      protected ValueTypeConverter<StringBasedStructValueType, string> StringBasedStructValueTypeConverter { get; }

      protected TypeConverterTestsBase()
      {
         IntBasedReferenceValueTypeConverter = new IntBasedReferenceValueType_ValueTypeConverter();
         IntBasedStructValueTypeConverter = new IntBasedStructValueType_ValueTypeConverter();
         StringBasedReferenceValueTypeConverter = new StringBasedReferenceValueType_ValueTypeConverter();
         StringBasedStructValueTypeConverter = new StringBasedStructValueType_ValueTypeConverter();
      }
   }
}
