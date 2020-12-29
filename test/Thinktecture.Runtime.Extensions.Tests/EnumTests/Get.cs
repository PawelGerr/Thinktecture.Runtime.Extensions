using System;
using System.Reflection;
using FluentAssertions;
using Thinktecture.TestEnums;
using Thinktecture.TestEnums.Isolated;
using Xunit;

namespace Thinktecture.EnumTests
{
   public class Get
   {
      [Fact]
      public void Should_return_null_if_null_is_provided()
      {
         var item = TestEnum.Get(null);

         item.Should().BeNull();
      }

      [Fact]
      public void Should_return_invalid_item_if_enum_doesnt_have_any_items()
      {
         var item = EmptyEnum.Get("unknown");

         item.IsValid.Should().BeFalse();
         item.Key.Should().Be("unknown");
      }

      [Fact]
      public void Should_return_invalid_item_via_reflection_if_enum_doesnt_have_any_items()
      {
         // ReSharper disable once PossibleNullReferenceException
         var item = (EmptyEnum)typeof(EmptyEnum).GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                                                .Invoke(null, new object[] { "unknown" });

         item.IsValid.Should().BeFalse();
         item.Key.Should().Be("unknown");
      }

      [Fact]
      public void Should_return_invalid_item_if_enum_doesnt_have_item_with_provided_key()
      {
         var item = TestEnum.Get("unknown");

         item.IsValid.Should().BeFalse();
         item.Key.Should().Be("unknown");
      }

      [Fact]
      public void Should_return_item_with_provided_key()
      {
         var item = TestEnum.Get("item2");
         item.Should().Be(TestEnum.Item2);
      }

      [Fact]
      public void Should_return_item_with_provided_key_ignoring_casing()
      {
         var item = StaticCtorTestEnum_Get.Get("Item");
         item.Should().Be(StaticCtorTestEnum_Get.Item);
      }

      [Fact]
      public void Should_return_invalid_item_if_the_casing_does_not_match_according_to_comparer()
      {
         var item = TestEnumWithNonDefaultComparer.Get("Item2");
         item.Key.Should().Be("Item2");
         item.IsValid.Should().BeFalse();
      }

      [Fact]
      public void Should_return_derived_type()
      {
         EnumWithDerivedType.Get(2).Should().Be(EnumWithDerivedType.ItemOfDerivedType);

         AbstractEnum.Get(1).Should().Be(AbstractEnum.Item);
      }
   }
}
