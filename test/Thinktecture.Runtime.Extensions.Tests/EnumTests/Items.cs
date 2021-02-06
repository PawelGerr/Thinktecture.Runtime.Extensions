using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTests
{
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

         var extensibleItems = ExtensibleTestEnum.Items;
         extensibleItems.Should().HaveCount(2);
         extensibleItems.Should().Contain(ExtensibleTestEnum.Item1);
         extensibleItems.Should().Contain(ExtensibleTestEnum.DerivedItem);

         var extendedItems = ExtendedTestEnum.Items;
         extendedItems.Should().HaveCount(3);
         extendedItems.Should().Contain(ExtendedTestEnum.Item1);
         extendedItems.Should().Contain(ExtendedTestEnum.Item2);
         extendedItems.Should().Contain(ExtendedTestEnum.DerivedItem);
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
         var enums = (IReadOnlyList<IEnum<string>>)typeof(TestEnum).GetProperty("Items", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                                                   ?.GetValue(null);

         enums.Should().HaveCount(2);
         enums.Should().Contain(TestEnum.Item1);
         enums.Should().Contain(TestEnum.Item2);
      }
   }
}
