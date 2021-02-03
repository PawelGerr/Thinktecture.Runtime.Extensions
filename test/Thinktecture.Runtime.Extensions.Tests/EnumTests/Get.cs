using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestEnums.Isolated;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTests
{
   public class Get
   {
      [Fact]
      public void Should_return_null_if_null_is_provided()
      {
         TestEnum.Get(null).Should().BeNull();
         ValidTestEnum.Get(null).Should().BeNull();
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
         TestEnum.Get("item2").Should().Be(TestEnum.Item2);
      }

      [Fact]
      public void Should_return_item_with_provided_key_ignoring_casing()
      {
         StaticCtorTestEnum_Get.Get("Item").Should().Be(StaticCtorTestEnum_Get.Item);
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

      [Fact]
      public void Should_return_valid_item_of_non_validatable_enum()
      {
         ValidTestEnum.Get("item1").Should().Be(ValidTestEnum.Item1);
      }

      [Fact]
      public void Should_throw_if_key_is_unknown_to_non_validatable_enum()
      {
         Action action = () => ValidTestEnum.Get("invalid");
         action.Should().Throw<KeyNotFoundException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
      }
   }
}
