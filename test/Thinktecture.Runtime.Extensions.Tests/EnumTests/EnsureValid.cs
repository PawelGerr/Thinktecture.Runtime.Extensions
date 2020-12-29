using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTests
{
   public class EnsureValid
   {
      [Fact]
      public void Should_not_throw_if_item_is_valid()
      {
         TestEnum.Item1.EnsureValid();
         StructIntegerEnum.Item1.EnsureValid();
         StructIntegerEnumWithZero.Item0.EnsureValid();
         StructStringEnum.Item1.EnsureValid();
      }

      [Fact]
      public void Should_not_throw_for_derived_types()
      {
         EnumWithDerivedType.Item1.EnsureValid();
         EnumWithDerivedType.ItemOfDerivedType.EnsureValid();

         AbstractEnum.Item.EnsureValid();
      }

      [Fact]
      public void Should_not_throw_if_default_struct_is_valid()
      {
         StructIntegerEnumWithZero.Item0.EnsureValid();
      }

      [Fact]
      public void Should_throw_if_default_struct_is_invalid()
      {
         new StructIntegerEnum().Invoking(e => e.EnsureValid())
                                .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructIntegerEnum)}' with key '0' is not valid.");

         new StructStringEnum().Invoking(e => e.EnsureValid())
                               .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructStringEnum)}' with key '' is not valid.");
      }

      [Fact]
      public void Should_throw_if_item_is_invalid()
      {
         TestEnum.Get("invalid").Invoking(e => e.EnsureValid())
                 .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(TestEnum)}' with key 'invalid' is not valid.");

         StructStringEnum.Get("invalid").Invoking(e => e.EnsureValid())
                         .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructStringEnum)}' with key 'invalid' is not valid.");

         AbstractEnum.Get(42).Invoking(e => e.EnsureValid())
                     .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(AbstractEnum)}' with key '42' is not valid.");

         StructIntegerEnum.Get(42).Invoking(e => e.EnsureValid())
                          .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructIntegerEnum)}' with key '42' is not valid.");

         // we cannot prevent construction of a struct
         new StructIntegerEnumWithZero().Invoking(e => e.EnsureValid())
                                        .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructIntegerEnumWithZero)}' with key '0' is not valid.");
      }
   }
}
