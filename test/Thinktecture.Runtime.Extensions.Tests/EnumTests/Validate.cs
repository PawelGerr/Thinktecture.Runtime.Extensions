using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Validate
{
   [Fact]
   public void Should_return_error_if_null_is_provided()
   {
      var validationError = SmartEnum_StringBased.Validate(null!, null, out var testEnum);
      validationError.Should().NotBeNull();
      validationError.ToString().Should().Be("There is no item of type 'SmartEnum_StringBased' with the identifier ''.");

      testEnum.Should().BeNull();
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_return_item_having_ReadOnlySpanOfChar()
   {
      var validationError = SmartEnum_StringBased.Validate(SmartEnum_StringBased.Item1.Key.AsSpan(), null, out var item);

      validationError.Should().BeNull();
      item.Should().Be(SmartEnum_StringBased.Item1);
   }
#endif

   [Fact]
   public void Should_return_item_with_provided_key()
   {
      SmartEnum_StringBased.Validate("Item2", null, out var item).Should().BeNull();
      item.Should().Be(SmartEnum_StringBased.Item2);
   }

   [Fact]
   public void Should_return_item_with_provided_key_ignoring_casing()
   {
      SmartEnum_StringBased.Validate("Item1", null, out var item).Should().BeNull();
      item.Should().Be(SmartEnum_StringBased.Item1);

      SmartEnum_StringBased.Validate("item1", null, out item).Should().BeNull();
      item.Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_return_derived_type()
   {
      SmartEnum_DerivedTypes.Validate(2, null, out var item).Should().BeNull();
      item.Should().Be(SmartEnum_DerivedTypes.ItemOfDerivedType);
   }

   [Fact]
   public void Should_return_error_if_key_is_unknown()
   {
      var validationError = SmartEnum_StringBased.Validate("invalid", null, out var item);
      validationError.ToString().Should().Be("There is no item of type 'SmartEnum_StringBased' with the identifier 'invalid'.");

      item.Should().BeNull();
   }

   [Fact]
   public void Should_return_item_using_factory_specified_via_ObjectFactoryAttribute()
   {
      var validationError = SmartEnum_Factory.Validate("=1=", null, out var item);
      validationError.Should().BeNull();

      item.Should().Be(SmartEnum_Factory.Item1);
   }

   [Fact]
   public void Should_return_custom_error_if_enum_uses_ValidationErrorAttribute()
   {
      var validationError = TestSmartEnum_CustomError.Validate("invalid", null, out var item);
      validationError.Should().BeOfType<CustomValidationError>();
      validationError.ToString().Should().Be("There is no item of type 'TestSmartEnum_CustomError' with the identifier 'invalid'.");

      item.Should().BeNull();
   }
}
