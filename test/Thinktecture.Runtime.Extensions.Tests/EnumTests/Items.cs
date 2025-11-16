using System.Collections.Generic;
using System.Reflection;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Items
{
   [Fact]
   public void Should_return_empty_collection_if_enum_has_no_items()
   {
      SmartEnum_Empty.Items.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_public_fields_only()
   {
      var enums = SmartEnum_StringBased.Items;
      enums.Should().HaveCount(2);
      enums.Should().Contain(SmartEnum_StringBased.Item1);
      enums.Should().Contain(SmartEnum_StringBased.Item2);
   }

   [Fact]
   public void Should_return_fields_of_a_valid_enum()
   {
      var enums = SmartEnum_StringBased.Items;
      enums.Should().HaveCount(2);
      enums.Should().Contain(SmartEnum_StringBased.Item1);
      enums.Should().Contain(SmartEnum_StringBased.Item2);
   }

   [Fact]
   public void Should_return_public_fields_only_via_reflection()
   {
      var enums = (IReadOnlyList<SmartEnum_StringBased>)typeof(SmartEnum_StringBased).GetProperty("Items", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                                           ?.GetValue(null);

      enums.Should().HaveCount(2);
      enums.Should().Contain(SmartEnum_StringBased.Item1);
      enums.Should().Contain(SmartEnum_StringBased.Item2);
   }

   [Fact]
   public void Should_return_items_of_keyless_enum()
   {
      var enums = SmartEnum_Keyless.Items;

      enums.Should().HaveCount(2);
      enums.Should().Contain(SmartEnum_Keyless.Item1);
      enums.Should().Contain(SmartEnum_Keyless.Item2);
   }

   [Fact]
   public void Should_return_items_for_generic_keyless_enum_with_different_type_arguments()
   {
      var stringEnums = SmartEnum_Generic_Keyless<string>.Items;
      stringEnums.Should().HaveCount(2);
      stringEnums.Should().Contain(SmartEnum_Generic_Keyless<string>.Item1);
      stringEnums.Should().Contain(SmartEnum_Generic_Keyless<string>.Item2);

      var objectEnums = SmartEnum_Generic_Keyless<object>.Items;
      objectEnums.Should().HaveCount(2);
      objectEnums.Should().Contain(SmartEnum_Generic_Keyless<object>.Item1);
      objectEnums.Should().Contain(SmartEnum_Generic_Keyless<object>.Item2);
   }

   [Fact]
   public void Should_return_items_for_generic_int_based_enum_with_different_type_arguments()
   {
      var stringEnums = SmartEnum_Generic_IntBased<string>.Items;
      stringEnums.Should().HaveCount(3);
      stringEnums.Should().Contain(SmartEnum_Generic_IntBased<string>.Item1);
      stringEnums.Should().Contain(SmartEnum_Generic_IntBased<string>.Item2);
      stringEnums.Should().Contain(SmartEnum_Generic_IntBased<string>.Item3);

      var intEnums = SmartEnum_Generic_IntBased<int>.Items;
      intEnums.Should().HaveCount(3);
   }

   [Fact]
   public void Should_return_items_for_generic_string_based_enum()
   {
      var intEnums = SmartEnum_Generic_StringBased<int>.Items;
      intEnums.Should().HaveCount(2);
      intEnums.Should().Contain(SmartEnum_Generic_StringBased<int>.Item1);
      intEnums.Should().Contain(SmartEnum_Generic_StringBased<int>.Item2);
   }

   [Fact]
   public void Should_maintain_separate_instances_for_generic_enums_with_different_type_arguments()
   {
      SmartEnum_Generic_Keyless<string>.Item1.Should().NotBeSameAs(SmartEnum_Generic_Keyless<object>.Item1);
   }
}
