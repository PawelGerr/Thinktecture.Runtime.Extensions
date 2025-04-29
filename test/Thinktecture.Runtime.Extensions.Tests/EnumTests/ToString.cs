using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ToString
{
   [Fact]
   public void Should_return_string_representation_of_the_key()
   {
      SmartEnum_StringBased.Item1.ToString().Should().Be("Item1");
   }
}
