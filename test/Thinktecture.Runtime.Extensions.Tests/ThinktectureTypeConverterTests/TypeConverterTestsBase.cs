using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ThinktectureTypeConverterTests;

public abstract class TypeConverterTestsBase
{
   protected ThinktectureTypeConverter<IntBasedReferenceValueObject, int, ValidationError> IntBasedReferenceValueObjectTypeConverter { get; }
   protected ThinktectureTypeConverter<IntBasedStructValueObject, int, ValidationError> IntBasedStructValueObjectTypeConverter { get; }
   protected ThinktectureTypeConverter<StringBasedReferenceValueObject, string, ValidationError> StringBasedReferenceValueObjectTypeConverter { get; }
   protected ThinktectureTypeConverter<StringBasedStructValueObject, string, ValidationError> StringBasedStructValueObjectTypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      IntBasedReferenceValueObjectTypeConverter = new ThinktectureTypeConverter<IntBasedReferenceValueObject, int, ValidationError>();
      IntBasedStructValueObjectTypeConverter = new ThinktectureTypeConverter<IntBasedStructValueObject, int, ValidationError>();
      StringBasedReferenceValueObjectTypeConverter = new ThinktectureTypeConverter<StringBasedReferenceValueObject, string, ValidationError>();
      StringBasedStructValueObjectTypeConverter = new ThinktectureTypeConverter<StringBasedStructValueObject, string, ValidationError>();
   }
}
