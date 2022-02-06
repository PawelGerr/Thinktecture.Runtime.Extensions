using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

public class Initialization
{
   [Fact]
   public void Should_throw_if_enum_has_duplicate_key()
   {
      Action action = () => _ = EnumWithDuplicateKey.Items;
      action.Should().Throw<ArgumentException>()
            .WithMessage($"The type \"{nameof(EnumWithDuplicateKey)}\" has multiple items with the identifier \"item\".");
   }

   [Fact]
   public void Should_not_throw_if_enum_has_2_keys_that_differ_in_casing_only_if_comparer_honors_casing()
   {
      Action action = () => _ = TestEnumWithNonDefaultComparer.Items;
      action.Should().NotThrow<Exception>();
   }
}
