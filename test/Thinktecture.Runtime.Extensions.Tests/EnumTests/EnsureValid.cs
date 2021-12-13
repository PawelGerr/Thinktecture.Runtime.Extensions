using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class EnsureValid
{
   [Fact]
   public void Should_not_throw_if_item_is_valid()
   {
      TestEnum.Item1.EnsureValid();
      StructIntegerEnum.Item1.EnsureValid();
      StructIntegerEnumWithZero.Item0.EnsureValid();
      StructStringEnum.Item1.EnsureValid();
      ExtensibleTestValidatableEnum.Item1.EnsureValid();
      ExtendedTestValidatableEnum.Item2.EnsureValid();
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
                             .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructIntegerEnum)}' with identifier '0' is not valid.");

      new StructStringEnum().Invoking(e => e.EnsureValid())
                            .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructStringEnum)}' with identifier '' is not valid.");
   }

   [Fact]
   public void Should_throw_if_item_is_invalid()
   {
      TestEnum.Get("invalid").Invoking(e => e.EnsureValid())
              .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(TestEnum)}' with identifier 'invalid' is not valid.");

      StructStringEnum.Get("invalid").Invoking(e => e.EnsureValid())
                      .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructStringEnum)}' with identifier 'invalid' is not valid.");

      AbstractEnum.Get(42).Invoking(e => e.EnsureValid())
                  .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(AbstractEnum)}' with identifier '42' is not valid.");

      StructIntegerEnum.Get(42).Invoking(e => e.EnsureValid())
                       .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructIntegerEnum)}' with identifier '42' is not valid.");

      // we cannot prevent construction of a struct
      new StructIntegerEnumWithZero().Invoking(e => e.EnsureValid())
                                     .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructIntegerEnumWithZero)}' with identifier '0' is not valid.");

      ExtensibleTestValidatableEnum.Get("invalid").Invoking(e => e.EnsureValid())
                                   .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(ExtensibleTestValidatableEnum)}' with identifier 'invalid' is not valid.");

      ExtendedTestValidatableEnum.Get("invalid").Invoking(e => e.EnsureValid())
                                 .Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(ExtendedTestValidatableEnum)}' with identifier 'invalid' is not valid.");
   }
}