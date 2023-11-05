using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Validate
{
   [Fact]
   public void Should_return_error_if_null_is_provided()
   {
      var testEnumValidationResult = TestEnum.Validate(null!, null, out var testEnum);
      testEnumValidationResult.Should().NotBeNull();
      testEnumValidationResult.ErrorMessage.Should().Be("There is no item of type 'TestEnum' with the identifier ''.");
      testEnumValidationResult.MemberNames.Should().BeEquivalentTo(nameof(TestEnum.Key));

      testEnum.Should().BeNull();

      var validTestEnumValidationResult = ValidTestEnum.Validate(null!, null, out var validTestEnum);
      validTestEnumValidationResult.Should().NotBeNull();
      validTestEnumValidationResult.ErrorMessage.Should().Be("There is no item of type 'ValidTestEnum' with the identifier ''.");
      validTestEnumValidationResult.MemberNames.Should().BeEquivalentTo(nameof(TestEnum.Key));

      validTestEnum.Should().BeNull();
   }

   [Fact]
   public void Should_return_invalid_item_if_enum_doesnt_have_any_items()
   {
      var validationResult = EmptyEnum.Validate("unknown", null, out var item);

      validationResult.Should().NotBeNull();
      validationResult.ErrorMessage.Should().Be("There is no item of type 'EmptyEnum' with the identifier 'unknown'.");
      validationResult.MemberNames.Should().BeEquivalentTo(nameof(EmptyEnum.Key));

      item.Should().NotBeNull();
      item!.IsValid.Should().BeFalse();
      item.Key.Should().Be("unknown");
   }

   [Fact]
   public void Should_return_invalid_item_if_enum_doesnt_have_item_with_provided_key()
   {
      var validationResult = TestEnum.Validate("unknown", null, out var item);

      validationResult.Should().NotBeNull();
      validationResult.ErrorMessage.Should().Be("There is no item of type 'TestEnum' with the identifier 'unknown'.");
      validationResult.MemberNames.Should().BeEquivalentTo(nameof(TestEnum.Key));

      item.Should().NotBeNull();
      item!.IsValid.Should().BeFalse();
      item.Key.Should().Be("unknown");
   }

   [Fact]
   public void Should_throw_if_CreateInvalidItem_uses_key_of_valid_item()
   {
      Action action = () => TestEnumWithInvalidCreateInvalidItem.Validate(TestEnumWithInvalidCreateInvalidItem.INVALID_KEY_FOR_TESTING_KEY_REUSE, null, out _);
      action.Should().Throw<Exception>().WithMessage("The implementation of method 'CreateInvalidItem' must not return an instance with property 'Key' equals to one of a valid item.");
   }

   [Fact]
   public void Should_throw_if_CreateInvalidItem_isValid_is_true()
   {
      Action action = () => TestEnumWithInvalidCreateInvalidItem.Validate(TestEnumWithInvalidCreateInvalidItem.INVALID_KEY_FOR_TESTING_IS_VALID_TRUE, null, out _);
      action.Should().Throw<Exception>().WithMessage("The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'.");
   }

   [Fact]
   public void Should_throw_if_custom_validation_throws()
   {
      Action action = () => TestEnum.Validate(String.Empty, null, out _);
      action.Should().Throw<ArgumentException>().WithMessage("Key cannot be empty.");
   }

   [Fact]
   public void Should_return_item_with_provided_key()
   {
      TestEnum.Validate("item2", null, out var testEnum).Should().BeNull();
      testEnum.Should().Be(TestEnum.Item2);

      ValidTestEnum.Validate("item1", null, out var validTestEnum).Should().BeNull();
      validTestEnum.Should().Be(ValidTestEnum.Item1);
   }

   [Fact]
   public void Should_return_item_with_provided_key_ignoring_casing()
   {
      TestEnum.Validate("Item1", null, out var item).Should().BeNull();
      item.Should().Be(TestEnum.Item1);

      TestEnum.Validate("item1", null, out item).Should().BeNull();
      item.Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_return_invalid_item_if_the_casing_does_not_match_according_to_comparer()
   {
      var validationResult = TestEnumWithNonDefaultComparer.Validate("Item2", null, out var item);
      validationResult.Should().NotBeNull();
      validationResult.ErrorMessage.Should().Be("There is no item of type 'TestEnumWithNonDefaultComparer' with the identifier 'Item2'.");
      validationResult.MemberNames.Should().BeEquivalentTo(nameof(TestEnum.Key));

      item!.Key.Should().Be("Item2");
      item.IsValid.Should().BeFalse();
   }

   [Fact]
   public void Should_return_derived_type()
   {
      EnumWithDerivedType.Validate(2, null, out var item).Should().BeNull();
      item.Should().Be(EnumWithDerivedType.ItemOfDerivedType);

      AbstractEnum.Validate(1, null, out var otherItem).Should().BeNull();
      otherItem.Should().Be(AbstractEnum.Item);
   }

   [Fact]
   public void Should_return_error_if_key_is_unknown_to_non_validatable_enum()
   {
      var validationResult = ValidTestEnum.Validate("invalid", null, out var item);
      validationResult.ErrorMessage.Should().Be("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");

      item.Should().BeNull();
   }
}
