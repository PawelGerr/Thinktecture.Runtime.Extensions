using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ToString
{
   [Fact]
   public void Should_return_string_representation_of_the_key()
   {
      SmartEnum_StringBased.Item1.ToString().Should().Be("Item1");
   }

   [Fact]
   public void Should_return_key_string_for_generic_key_based_unconstraint_enum()
   {
      SmartEnum_GenericKeyBasedUnconstraint<int>.Item1.ToString().Should().Be("1");
      SmartEnum_GenericKeyBasedUnconstraint<int>.Item2.ToString().Should().Be("2");
   }
}
