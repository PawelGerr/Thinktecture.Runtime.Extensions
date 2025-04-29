using System.ComponentModel;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public class GetConverter
{
   [Fact]
   public void Should_return_enum_typeconverter_via_typedescriptor()
   {
      TypeDescriptor.GetConverter(typeof(SmartEnum_StringBased)).Should().BeOfType(typeof(ThinktectureTypeConverter<SmartEnum_StringBased, string, ValidationError>));
   }
}
