using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class ConversionFromKey
{
   [Fact]
   public void Should_return_null_if_key_is_null()
   {
      string key = null;
      var item = (SmartEnum_StringBased)key;

      item.Should().BeNull();
   }

   [Fact]
   public void Should_be_implicit_if_configured()
   {
      SmartEnum_CustomConversionOperators item = 1;

      item.Should().Be(SmartEnum_CustomConversionOperators.Item);
   }

   [Fact]
   public void Should_return_item()
   {
      var testEnum = (SmartEnum_StringBased)"item1";
      testEnum.Should().Be(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_throw_if_enum_has_no_such_key()
   {
      Action action = () => _ = (SmartEnum_StringBased)"invalid";
      action.Should().Throw<UnknownSmartEnumIdentifierException>().WithMessage("There is no item of type 'SmartEnum_StringBased' with the identifier 'invalid'.");
   }

   [Fact]
   public void Should_convert_from_int_explicitly_for_generic_int_based_enum()
   {
      var item = (SmartEnum_Generic_IntBased<string>)1;
      item.Should().BeSameAs(SmartEnum_Generic_IntBased<string>.Item1);

      item = (SmartEnum_Generic_IntBased<string>)2;
      item.Should().BeSameAs(SmartEnum_Generic_IntBased<string>.Item2);
   }

   [Fact]
   public void Should_convert_from_string_explicitly_for_generic_string_based_enum()
   {
      var item = (SmartEnum_Generic_StringBased<int>)"item1";
      item.Should().BeSameAs(SmartEnum_Generic_StringBased<int>.Item1);

      item = (SmartEnum_Generic_StringBased<int>)"item2";
      item.Should().BeSameAs(SmartEnum_Generic_StringBased<int>.Item2);
   }
}
