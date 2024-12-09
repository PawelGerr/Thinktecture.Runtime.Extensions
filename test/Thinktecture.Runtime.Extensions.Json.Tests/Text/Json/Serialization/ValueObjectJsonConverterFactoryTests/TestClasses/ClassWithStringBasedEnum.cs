using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests.TestClasses;

public class ClassWithStringBasedEnum
{
   public TestEnum Enum { get; set; }

   public ClassWithStringBasedEnum()
   {
   }

   public ClassWithStringBasedEnum(TestEnum value)
   {
      Enum = value;
   }
}
