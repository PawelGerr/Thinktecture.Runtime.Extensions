using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable once InconsistentNaming
public class Map
{
   [Fact]
   public void Should_return_correct_item()
   {
      SmartEnum_StringBased.Item1.Map(item1: 1,
                                      item2: 2)
                           .Should().Be(1);

      SmartEnum_StringBased.Item2.Map(item1: 1,
                                      item2: 2)
                           .Should().Be(2);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_return_ref_struct()
   {
      SmartEnum_StringBased.Item1.Map(item1: new TestRefStruct(1),
                              item2: new TestRefStruct(2))
                   .Value.Should().Be(1);
   }
#endif

   [Fact]
   public void Should_return_correct_item_with_keyless_enum()
   {
      SmartEnum_Keyless.Item1.Map(item1: 1,
                                     item2: 2)
                          .Should().Be(1);

      SmartEnum_Keyless.Item2.Map(item1: 1,
                                     item2: 2)
                          .Should().Be(2);
   }
}
