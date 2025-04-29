using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

// ReSharper disable InconsistentNaming
public abstract class TypeConverterTestsBase
{
   protected ThinktectureTypeConverter<SmartEnum_StringBased, string, ValidationError> SmartEnum_StringBased_TypeConverter { get; }
   protected ThinktectureTypeConverter<SmartEnum_IntBased, int, ValidationError> SmartEnum_IntBased_TypeConverter { get; }

   protected TypeConverterTestsBase()
   {
      SmartEnum_StringBased_TypeConverter = new ThinktectureTypeConverter<SmartEnum_StringBased, string, ValidationError>();
      SmartEnum_IntBased_TypeConverter = new ThinktectureTypeConverter<SmartEnum_IntBased, int, ValidationError>();
   }
}
