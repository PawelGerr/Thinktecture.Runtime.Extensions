using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Map
{
   [Fact]
   public void Should_return_correct_item()
   {
      TestEnum.Item1.Map(TestEnum.Item1, 1,
                         TestEnum.Item2, 2)
              .Should().Be(1);

      TestEnum.Item2.Map(TestEnum.Item1, 1,
                         TestEnum.Item2, 2)
              .Should().Be(2);
   }
}
