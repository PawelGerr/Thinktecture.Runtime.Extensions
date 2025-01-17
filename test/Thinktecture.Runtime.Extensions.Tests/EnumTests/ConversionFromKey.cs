using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ConversionFromKey
{
   [Fact]
   public void Should_return_null_if_key_is_null()
   {
      string key = null;
      var item = (TestEnum)key;

      item.Should().BeNull();
   }

   [Fact]
   public void Should_be_implicit_if_configured()
   {
      TestEnumWithCustomConversionOperators item = 1;

      item.Should().Be(TestEnumWithCustomConversionOperators.Item);
   }

   [Fact]
   public void Should_return_invalid_item_if_struct_is_default_and_there_are_no_items_for_default_value()
   {
      int key = default;
      var item = (StructIntegerEnum)key;

      item.Should().Be(new StructIntegerEnum());
   }

   [Fact]
   public void Should_return_item()
   {
      var testEnum = (TestEnum)"item1";
      testEnum.Should().Be(TestEnum.Item1);
   }

   [Fact]
   public void Should_return_invalid_item_if_enum_has_no_such_key()
   {
      var item = TestEnum.Get("invalid");
      item.Key.Should().Be("invalid");
      item.IsValid.Should().BeFalse();
   }

   [Fact]
   public void Should_throw_if_non_validable_enum_has_no_such_key()
   {
      Action action = () => _ = (ValidTestEnum)"invalid";
      action.Should().Throw<UnknownEnumIdentifierException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");
   }
}
