using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ToString
{
   [Fact]
   public void Should_return_string_representation_of_the_key()
   {
      TestEnum.Item1.ToString().Should().Be("item1");
   }

   [Fact]
   public void Should_return_string_representation_of_the_key_for_structs()
   {
      StructIntegerEnum.Item1.ToString().Should().Be("1");
   }
}
