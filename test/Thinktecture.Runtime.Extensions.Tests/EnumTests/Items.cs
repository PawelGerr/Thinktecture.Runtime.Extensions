using System.Collections.Generic;
using System.Reflection;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Items
{
   [Fact]
   public void Should_return_empty_collection_if_enum_has_no_items()
   {
      EmptyEnum.Items.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_public_fields_only()
   {
      var enums = TestEnum.Items;
      enums.Should().HaveCount(2);
      enums.Should().Contain(TestEnum.Item1);
      enums.Should().Contain(TestEnum.Item2);
   }

   [Fact]
   public void Should_return_fields_of_a_struct()
   {
      var enums = StructIntegerEnum.Items;
      enums.Should().HaveCount(2);
      enums.Should().Contain(StructIntegerEnum.Item1);
      enums.Should().Contain(StructIntegerEnum.Item2);
   }

   [Fact]
   public void Should_return_fields_of_a_valid_enum()
   {
      var enums = ValidTestEnum.Items;
      enums.Should().HaveCount(2);
      enums.Should().Contain(ValidTestEnum.Item1);
      enums.Should().Contain(ValidTestEnum.Item2);
   }

   [Fact]
   public void Should_return_public_fields_only_via_reflection()
   {
      var enums = (IReadOnlyList<TestEnum>)typeof(TestEnum).GetProperty("Items", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                                           ?.GetValue(null);

      enums.Should().HaveCount(2);
      enums.Should().Contain(TestEnum.Item1);
      enums.Should().Contain(TestEnum.Item2);
   }

   [Fact]
   public void Should_return_items_of_keyless_enum()
   {
      var enums = KeylessTestEnum.Items;

      enums.Should().HaveCount(2);
      enums.Should().Contain(KeylessTestEnum.Item1);
      enums.Should().Contain(KeylessTestEnum.Item2);
   }
}
