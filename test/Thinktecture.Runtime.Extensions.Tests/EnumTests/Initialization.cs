using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Initialization
{
   [Fact]
   public void Should_throw_if_enum_has_duplicate_key()
   {
      Action action = () => _ = SmartEnum_DuplicateKey.Items;
      action.Should().Throw<ArgumentException>()
            .WithMessage($"The type \"{nameof(SmartEnum_DuplicateKey)}\" has multiple items with the identifier \"item\".");
   }

   [Fact]
   public void Should_not_throw_if_enum_has_2_keys_that_differ_in_casing_only_if_comparer_honors_casing()
   {
      Action action = () => _ = SmartEnum_CaseSensitive.Items;
      action.Should().NotThrow<Exception>();
   }

   [Fact]
   public void Should_throw_if_custom_validation_throws()
   {
      Action action = () => SmartEnum_CustomConstructorValidation.Get(String.Empty);
      action.Should().Throw<TypeInitializationException>()
            .WithInnerException<ArgumentException>()
            .WithMessage("Key cannot be empty.");
   }
}
