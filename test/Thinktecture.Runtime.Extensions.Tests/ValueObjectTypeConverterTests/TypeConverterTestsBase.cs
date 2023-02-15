using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTypeConverterTests;

public abstract class TypeConverterTestsBase
{
   protected ValueObjectTypeConverter<IntBasedReferenceValueObject, int> IntBasedReferenceValueObjectTypeConverter { get; }
   protected ValueObjectTypeConverter<IntBasedStructValueObject, int> IntBasedStructValueObjectTypeConverter { get; }
   protected ValueObjectTypeConverter<StringBasedReferenceValueObject, string> StringBasedReferenceValueObjectTypeConverter { get; }
   protected ValueObjectTypeConverter<StringBasedStructValueObject, string> StringBasedStructValueObjectTypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      IntBasedReferenceValueObjectTypeConverter = new ValueObjectTypeConverter<IntBasedReferenceValueObject, int>();
      IntBasedStructValueObjectTypeConverter = new ValueObjectTypeConverter<IntBasedStructValueObject, int>();
      StringBasedReferenceValueObjectTypeConverter = new ValueObjectTypeConverter<StringBasedReferenceValueObject, string>();
      StringBasedStructValueObjectTypeConverter = new ValueObjectTypeConverter<StringBasedStructValueObject, string>();
   }
}
