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
}
