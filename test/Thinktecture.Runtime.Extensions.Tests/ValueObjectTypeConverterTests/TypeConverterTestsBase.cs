using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTypeConverterTests
{
   public abstract class TypeConverterTestsBase
   {
      protected ValueObjectTypeConverter<IntBasedReferenceValueObject, int> IntBasedReferenceValueObjectTypeConverter { get; }
      protected ValueObjectTypeConverter<IntBasedStructValueObject, int> IntBasedStructValueObjectTypeConverter { get; }
      protected ValueObjectTypeConverter<StringBasedReferenceValueObject, string> StringBasedReferenceValueObjectTypeConverter { get; }
      protected ValueObjectTypeConverter<StringBasedStructValueObject, string> StringBasedStructValueObjectTypeConverter { get; }

      protected TypeConverterTestsBase()
      {
         IntBasedReferenceValueObjectTypeConverter = new IntBasedReferenceValueObject_ValueObjectTypeConverter();
         IntBasedStructValueObjectTypeConverter = new IntBasedStructValueObject_ValueObjectTypeConverter();
         StringBasedReferenceValueObjectTypeConverter = new StringBasedReferenceValueObject_ValueObjectTypeConverter();
         StringBasedStructValueObjectTypeConverter = new StringBasedStructValueObject_ValueObjectTypeConverter();
      }
   }
}
