using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// ReSharper disable SuspiciousTypeConversion.Global
public class Equals
{
   [Fact]
   public void Should_return_false_if_item_is_null()
   {
      SmartEnum_StringBased.Item1.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_item_is_of_different_type()
   {
      SmartEnum_StringBased.Item1.Equals(SmartEnum_CaseSensitive.LowerCased).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_on_reference_equality()
   {
      SmartEnum_StringBased.Item1.Equals(SmartEnum_StringBased.Item1).Should().BeTrue();
   }

   [Fact]
   public void Should_compare_keyless_smart_enum_via_reference_equality()
   {
      SmartEnum_Keyless.Item1.Equals(SmartEnum_Keyless.Item1).Should().BeTrue();
      SmartEnum_Keyless.Item2.Equals(SmartEnum_Keyless.Item2).Should().BeTrue();
      SmartEnum_Keyless.Item1.Equals(SmartEnum_Keyless.Item2).Should().BeFalse();
   }

   [Fact]
   public void Should_support_equals_for_generic_keyless_enum()
   {
      var item1 = SmartEnum_Generic_Keyless<string>.Item1;
      var item1Again = SmartEnum_Generic_Keyless<string>.Item1;
      var item2 = SmartEnum_Generic_Keyless<string>.Item2;

      item1.Equals(item1Again).Should().BeTrue();
      item1.Equals(item2).Should().BeFalse();
   }

   [Fact]
   public void Should_support_equals_for_generic_int_based_enum()
   {
      var item1 = SmartEnum_Generic_IntBased<string>.Item1;
      var item1Again = SmartEnum_Generic_IntBased<string>.Get(1);
      var item2 = SmartEnum_Generic_IntBased<string>.Item2;

      item1.Equals(item1Again).Should().BeTrue();
      item1.Equals(item2).Should().BeFalse();
   }

   [Fact]
   public void Should_support_equals_for_generic_string_based_enum()
   {
      var item1 = SmartEnum_Generic_StringBased<int>.Item1;
      var item1Again = SmartEnum_Generic_StringBased<int>.Get("item1");
      var item2 = SmartEnum_Generic_StringBased<int>.Item2;

      item1.Equals(item1Again).Should().BeTrue();
      item1.Equals(item2).Should().BeFalse();
   }
}
