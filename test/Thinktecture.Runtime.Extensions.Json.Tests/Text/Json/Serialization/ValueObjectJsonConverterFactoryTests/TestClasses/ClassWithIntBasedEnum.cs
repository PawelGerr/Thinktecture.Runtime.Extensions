using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;

public class ClassWithIntBasedEnum
{
   public IntegerEnum Enum { get; set; }

   public ClassWithIntBasedEnum()
   {
   }

   public ClassWithIntBasedEnum(IntegerEnum value)
   {
      Enum = value;
   }
}
