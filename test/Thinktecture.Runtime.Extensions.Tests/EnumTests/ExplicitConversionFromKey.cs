using System;
using System.Collections.Generic;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTests
{
   public class ExplicitConversionFromKey
   {
      [Fact]
      public void Should_return_null_if_key_is_null()
      {
         string key = null;
         var item = (TestEnum)key;

         item.Should().BeNull();
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

         var extensibleItem = (ExtensibleTestEnum)"Item1";
         extensibleItem.Should().Be(ExtensibleTestEnum.Item1);

         var extendedItem = (ExtendedTestEnum)"Item1";
         extendedItem.Should().Be(ExtendedTestEnum.Item1);

         extendedItem = (ExtendedTestEnum)"Item2";
         extendedItem.Should().Be(ExtendedTestEnum.Item2);
      }

      [Fact]
      public void Should_return_invalid_item_if_enum_has_no_such_key()
      {
         var item = TestEnum.Get("invalid");
         item.Key.Should().Be("invalid");
         item.IsValid.Should().BeFalse();

         var extensibleItem = ExtensibleTestValidatableEnum.Get("invalid");
         extensibleItem.Key.Should().Be("invalid");
         extensibleItem.IsValid.Should().BeFalse();

         var extendedItem = ExtendedTestValidatableEnum.Get("invalid");
         extendedItem.Key.Should().Be("invalid");
         extendedItem.IsValid.Should().BeFalse();
      }

      [Fact]
      public void Should_throw_if_non_validable_enum_has_no_such_key()
      {
         Action action = () =>
                         {
                            var item = (ValidTestEnum)"invalid";
                         };
         action.Should().Throw<KeyNotFoundException>().WithMessage("There is no item of type 'ValidTestEnum' with the identifier 'invalid'.");

         action = () =>
                  {
                     var item = (ExtensibleTestEnum)"invalid";
                  };
         action.Should().Throw<KeyNotFoundException>().WithMessage("There is no item of type 'ExtensibleTestEnum' with the identifier 'invalid'.");

         action = () =>
                  {
                     var item = (ExtendedTestEnum)"invalid";
                  };
         action.Should().Throw<KeyNotFoundException>().WithMessage("There is no item of type 'ExtendedTestEnum' with the identifier 'invalid'.");
      }
   }
}
