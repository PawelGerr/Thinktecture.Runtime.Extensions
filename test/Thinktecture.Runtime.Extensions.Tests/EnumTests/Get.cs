using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Get
{
   [Fact]
   public void Should_return_null_if_null_is_provided()
   {
      SmartEnum_StringBased.Get(null).Should().BeNull();
      SmartEnum_StringBased.Get(null).Should().BeNull();
   }

   [Fact]
   public void Should_return_item_with_provided_key()
   {
      SmartEnum_StringBased.Get("item2").Should().Be(SmartEnum_StringBased.Item2);

      SmartEnum_StringBased.Get("item1").Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_return_item_with_provided_key_ignoring_casing()
   {
      SmartEnum_StringBased.Get("Item1").Should().Be(SmartEnum_StringBased.Item1);
      SmartEnum_StringBased.Get("item1").Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_return_item_of_case_sensitive_enum()
   {
      SmartEnum_CaseSensitive.Get("item").Should().Be(SmartEnum_CaseSensitive.LowerCased);
      SmartEnum_CaseSensitive.Get("ITEM").Should().Be(SmartEnum_CaseSensitive.UpperCased);
   }

   [Fact]
   public void Should_return_derived_type()
   {
      SmartEnum_DerivedTypes.Get(2).Should().Be(SmartEnum_DerivedTypes.ItemOfDerivedType);
   }

   [Fact]
   public void Should_throw_if_key_is_unknown()
   {
      Action action = () => SmartEnum_StringBased.Get("invalid");
      action.Should().Throw<UnknownSmartEnumIdentifierException>().WithMessage("There is no item of type 'SmartEnum_StringBased' with the identifier 'invalid'.");
   }
}
