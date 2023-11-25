using System;
using System.Reflection;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

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

      item.Should().NotBeNull();
      item!.IsValid.Should().BeFalse();
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
   public void Should_throw_if_CreateInvalidItem_uses_key_of_valid_item()
   {
      Action action = () => TestEnumWithInvalidCreateInvalidItem.Get(TestEnumWithInvalidCreateInvalidItem.INVALID_KEY_FOR_TESTING_KEY_REUSE);
      action.Should().Throw<Exception>().WithMessage("The implementation of method 'CreateInvalidItem' must not return an instance with property 'Key' equals to one of a valid item.");
   }

   [Fact]
   public void Should_throw_if_CreateInvalidItem_isValid_is_true()
   {
      Action action = () => TestEnumWithInvalidCreateInvalidItem.Get(TestEnumWithInvalidCreateInvalidItem.INVALID_KEY_FOR_TESTING_IS_VALID_TRUE);
      action.Should().Throw<Exception>().WithMessage("The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'.");
   }

   [Fact]
   public void Should_throw_if_custom_validation_throws()
   {
      Action action = () => TestEnum.Get(String.Empty);
      action.Should().Throw<ArgumentException>().WithMessage("Key cannot be empty.");
   }

   [Fact]
   public void Should_return_item_with_provided_key()
   {
      TestEnum.Get("item2").Should().Be(TestEnum.Item2);

      ValidTestEnum.Get("item1").Should().Be(ValidTestEnum.Item1);
   }

   [Fact]
   public void Should_return_item_with_provided_key_ignoring_casing()
   {
      TestEnum.Get("Item1").Should().Be(TestEnum.Item1);
      TestEnum.Get("item1").Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_return_item_of_case_sensitive_enum()
   {
      ValidatableTestEnumCaseSensitive.Get("item").Should().Be(ValidatableTestEnumCaseSensitive.LowerCased);
      ValidatableTestEnumCaseSensitive.Get("ITEM").Should().Be(ValidatableTestEnumCaseSensitive.UpperCased);
   }

   [Fact]
   public void Should_return_invalid_item_if_the_casing_does_not_match_according_to_comparer()
   {
      var item = ValidatableTestEnumCaseSensitive.Get("ITEM2");
      item.Key.Should().Be("ITEM2");
      item.IsValid.Should().BeFalse();
   }

   [Fact]
   public void Should_return_derived_type()
   {
      EnumWithDerivedType.Get(2).Should().Be(EnumWithDerivedType.ItemOfDerivedType);

      AbstractEnum.Get(1).Should().Be(AbstractEnum.Item);
   }

   [Fact]
   public void Should_throw_if_key_is_unknown_to_non_validatable_enum()
   {
      Action action = () => ValidTestEnum.Get("invalid");
      action.Should().Throw<UnknownEnumIdentifierException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }
}
