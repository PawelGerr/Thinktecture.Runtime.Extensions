using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable once InconsistentNaming
public class MapPartially
{
   [Fact]
   public void Should_return_correct_item()
   {
      SmartEnum_StringBased_SwitchMapPartially.Item1.MapPartially(@default: 0,
                                                                  item1: 1,
                                                                  item2: 2)
                                              .Should().Be(1);

      SmartEnum_StringBased_SwitchMapPartially.Item1.MapPartially(@default: 0,
                                                                  item2: 2)
                                              .Should().Be(0);

      SmartEnum_StringBased_SwitchMapPartially.Item2.MapPartially(@default: 0,
                                                                  item1: 1,
                                                                  item2: 2)
                                              .Should().Be(2);

      SmartEnum_StringBased_SwitchMapPartially.Item2.MapPartially(@default: 0,
                                                                  item1: 1)
                                              .Should().Be(0);

      SmartEnum_StringBased_SwitchMapPartially.Item2.MapPartially(@default: 0)
                                              .Should().Be(0);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_return_ref_struct()
   {
      SmartEnum_StringBased_SwitchMapPartially.Item1.MapPartially(@default: new TestRefStruct(0),
                                                                  item1: new TestRefStruct(1),
                                                                  item2: new TestRefStruct(2))
                                              .Value.Should().Be(1);
   }
#endif

   [Fact]
   public void Should_return_correct_item_with_keyless_enum()
   {
      SmartEnum_Keyless.Item1.MapPartially(@default: 0,
                                           item1: 1,
                                           item2: 2)
                       .Should().Be(1);

      SmartEnum_Keyless.Item1.MapPartially(@default: 0,
                                           item2: 2)
                       .Should().Be(0);

      SmartEnum_Keyless.Item2.MapPartially(@default: 0,
                                           item1: 1,
                                           item2: 2)
                       .Should().Be(2);

      SmartEnum_Keyless.Item2.MapPartially(@default: 0,
                                           item1: 1)
                       .Should().Be(0);

      SmartEnum_Keyless.Item2.MapPartially(@default: 0)
                       .Should().Be(0);
   }
}
