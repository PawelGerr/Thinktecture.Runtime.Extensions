using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable once InconsistentNaming
public class MapPartially
{
   [Fact]
   public void Should_return_correct_item()
   {
      ValidTestEnum.Item1.MapPartially(@default: 0,
                                       item1: 1,
                                       item2: 2)
                   .Should().Be(1);

      ValidTestEnum.Item1.MapPartially(@default: 0,
                                       item2: 2)
                   .Should().Be(0);

      ValidTestEnum.Item2.MapPartially(@default: 0,
                                       item1: 1,
                                       item2: 2)
                   .Should().Be(2);

      ValidTestEnum.Item2.MapPartially(@default: 0,
                                       item1: 1)
                   .Should().Be(0);

      ValidTestEnum.Item2.MapPartially(@default: 0)
                   .Should().Be(0);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_return_ref_struct()
   {
      ValidTestEnum.Item1.MapPartially(@default: new TestRefStruct(0),
                                       item1: new TestRefStruct(1),
                                       item2: new TestRefStruct(2))
                   .Value.Should().Be(1);
   }
#endif

   [Theory]
   [InlineData(-1)]
   [InlineData(1)]
   [InlineData(2)]
   public void Should_return_correct_item_having_validatable_enum(int index)
   {
      var testItem = index switch
      {
         -1 => TestEnum.Get("invalid"),
         1 => TestEnum.Item1,
         2 => TestEnum.Item2,
         _ => throw new Exception("Unexpected index")
      };

      testItem.MapPartially(@default: 0,
                            invalid: -1,
                            item1: 1,
                            item2: 2)
              .Should().Be(index);

      testItem.MapPartially(@default: 0)
              .Should().Be(0);
   }

   [Theory]
   [InlineData(-1)]
   [InlineData(1)]
   [InlineData(2)]
   [InlineData(3)]
   [InlineData(4)]
   [InlineData(5)]
   public void Should_return_correct_item_having_struct(int index)
   {
      var testItem = index switch
      {
         -1 => TestSmartEnum_Struct_IntBased_Validatable.Get(0),
         1 => TestSmartEnum_Struct_IntBased_Validatable.Value1,
         2 => TestSmartEnum_Struct_IntBased_Validatable.Value2,
         3 => TestSmartEnum_Struct_IntBased_Validatable.Value3,
         4 => TestSmartEnum_Struct_IntBased_Validatable.Value4,
         5 => TestSmartEnum_Struct_IntBased_Validatable.Value5,
         _ => throw new Exception("Unexpected index")
      };

      testItem.MapPartially(@default: 0,
                            invalid: -1,
                            value1: 1,
                            value2: 2,
                            value3: 3,
                            value4: 4,
                            value5: 5)
              .Should().Be(index);

      testItem.MapPartially(@default: 0)
              .Should().Be(0);
   }

   [Fact]
   public void Should_return_correct_item_with_keyless_enum()
   {
      KeylessTestEnum.Item1.MapPartially(@default: 0,
                                         item1: 1,
                                         item2: 2)
                     .Should().Be(1);

      KeylessTestEnum.Item1.MapPartially(@default: 0,
                                         item2: 2)
                     .Should().Be(0);

      KeylessTestEnum.Item2.MapPartially(@default: 0,
                                         item1: 1,
                                         item2: 2)
                     .Should().Be(2);

      KeylessTestEnum.Item2.MapPartially(@default: 0,
                                         item1: 1)
                     .Should().Be(0);

      KeylessTestEnum.Item2.MapPartially(@default: 0)
                     .Should().Be(0);
   }
}
