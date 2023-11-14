using System;
using System.ComponentModel.DataAnnotations;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Validate
{
   [Fact]
   public void Should_return_error_if_null_is_provided()
   {
      var testEnumValidationError = TestEnum.Validate(null!, null, out var testEnum);
      testEnumValidationError.Should().NotBeNull();
      testEnumValidationError.ToString().Should().Be("There is no item of type 'TestEnum' with the identifier ''.");

      testEnum.Should().BeNull();

      var validTestEnumValidationError = ValidTestEnum.Validate(null!, null, out var validTestEnum);
      validTestEnumValidationError.Should().NotBeNull();
      validTestEnumValidationError.ToString().Should().Be("There is no item of type 'ValidTestEnum' with the identifier ''.");

      validTestEnum.Should().BeNull();
   }

   [Fact]
   public void Should_return_invalid_item_if_enum_doesnt_have_any_items()
   {
      var validationError = EmptyEnum.Validate("unknown", null, out var item);

      validationError.Should().NotBeNull();
      validationError.ToString().Should().Be("There is no item of type 'EmptyEnum' with the identifier 'unknown'.");

      item.Should().NotBeNull();
      item!.IsValid.Should().BeFalse();
      item.Key.Should().Be("unknown");
   }

   [Fact]
   public void Should_return_invalid_item_if_enum_doesnt_have_item_with_provided_key()
   {
      var validationError = TestEnum.Validate("unknown", null, out var item);

      validationError.Should().NotBeNull();
      validationError.ToString().Should().Be("There is no item of type 'TestEnum' with the identifier 'unknown'.");

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
      var validationError = TestEnumWithNonDefaultComparer.Validate("Item2", null, out var item);
      validationError.Should().NotBeNull();
      validationError.ToString().Should().Be("There is no item of type 'TestEnumWithNonDefaultComparer' with the identifier 'Item2'.");

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
      var validationError = ValidTestEnum.Validate("invalid", null, out var item);
      validationError.ToString().Should().Be("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");

      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_item_using_factory_specified_via_ValueObjectFactoryAttribute()
   {
      var validationError = EnumWithFactory.Validate("=1=", null, out var item);
      validationError.Should().BeNull();

      item.Should().Be(EnumWithFactory.Item1);
   }

   [Fact]
   public void Should_return_custom_error_if_enum_uses_ValueObjectValidationErrorAttribute()
   {
      var validationError = TestEnumWithCustomError.Validate("invalid", null, out var item);
      validationError.Should().BeOfType<TestEnumValidationError>();
      validationError.ToString().Should().Be("There is no item of type 'TestEnumWithCustomError' with the identifier 'invalid'.");

      item.Should().BeNull();
   }
}
