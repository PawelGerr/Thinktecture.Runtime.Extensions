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

   [Fact]
   public void Should_return_correct_value_for_generic_keyless_enum()
   {
      var result = SmartEnum_Generic_Keyless<string>.Item1.Map(
         item1: "one",
         item2: "two");

      result.Should().Be("one");

      result = SmartEnum_Generic_Keyless<string>.Item2.Map(
         item1: "one",
         item2: "two");

      result.Should().Be("two");
   }

   [Fact]
   public void Should_work_with_different_result_types_for_generic_keyless_enum()
   {
      var result = SmartEnum_Generic_Keyless<string>.Item1.Map(
         item1: 1,
         item2: 2);

      result.Should().Be(1);

      result = SmartEnum_Generic_Keyless<string>.Item2.Map(
         item1: 1,
         item2: 2);

      result.Should().Be(2);
   }

   [Fact]
   public void Should_return_correct_value_for_generic_int_based_enum()
   {
      var result = SmartEnum_Generic_IntBased<string>.Item1.Map(
         item1: "one",
         item2: "two",
         item3: "three");

      result.Should().Be("one");

      result = SmartEnum_Generic_IntBased<string>.Item2.Map(
         item1: "one",
         item2: "two",
         item3: "three");

      result.Should().Be("two");

      result = SmartEnum_Generic_IntBased<string>.Item3.Map(
         item1: "one",
         item2: "two",
         item3: "three");

      result.Should().Be("three");
   }

   [Fact]
   public void Should_return_correct_value_for_generic_string_based_enum()
   {
      var result = SmartEnum_Generic_StringBased<int>.Item1.Map(
         item1: "one",
         item2: "two");

      result.Should().Be("one");

      result = SmartEnum_Generic_StringBased<int>.Item2.Map(
         item1: "one",
         item2: "two");

      result.Should().Be("two");
   }
}
