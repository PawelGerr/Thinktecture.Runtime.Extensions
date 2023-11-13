using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTypeConverterTests;

public abstract class TypeConverterTestsBase
{
   protected ValueObjectTypeConverter<IntBasedReferenceValueObject, int, ValidationError> IntBasedReferenceValueObjectTypeConverter { get; }
   protected ValueObjectTypeConverter<IntBasedStructValueObject, int, ValidationError> IntBasedStructValueObjectTypeConverter { get; }
   protected ValueObjectTypeConverter<StringBasedReferenceValueObject, string, ValidationError> StringBasedReferenceValueObjectTypeConverter { get; }
   protected ValueObjectTypeConverter<StringBasedStructValueObject, string, ValidationError> StringBasedStructValueObjectTypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      IntBasedReferenceValueObjectTypeConverter = new ValueObjectTypeConverter<IntBasedReferenceValueObject, int, ValidationError>();
      IntBasedStructValueObjectTypeConverter = new ValueObjectTypeConverter<IntBasedStructValueObject, int, ValidationError>();
      StringBasedReferenceValueObjectTypeConverter = new ValueObjectTypeConverter<StringBasedReferenceValueObject, string, ValidationError>();
      StringBasedStructValueObjectTypeConverter = new ValueObjectTypeConverter<StringBasedStructValueObject, string, ValidationError>();
   }
}
