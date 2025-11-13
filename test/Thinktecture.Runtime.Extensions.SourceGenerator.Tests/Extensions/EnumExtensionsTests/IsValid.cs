using System;

namespace Thinktecture.Runtime.Tests.EnumExtensionsTests;

public class IsValid
{
   private enum MyIntEnum
   {
      None = 0,
      A = 1,
      B = 2,
      Negative = -5
   }

   [Flags]
   private enum FlagsNoCombined
   {
      None = 0,
      A = 1,
      B = 2,
      C = 4
   }

   [Flags]
   private enum FlagsWithCombined
   {
      None = 0,
      A = 1,
      B = 2,
      C = 4,
      AB = A | B
   }

   [Fact]
   public void IsValid_returns_true_for_defined_member()
   {
      MyIntEnum.None.IsValid().Should().BeTrue();
      MyIntEnum.A.IsValid().Should().BeTrue();
      MyIntEnum.B.IsValid().Should().BeTrue();
      MyIntEnum.Negative.IsValid().Should().BeTrue();
   }

   [Fact]
   public void IsValid_returns_false_for_undefined_member()
   {
      var undefined = (MyIntEnum)12345;
      undefined.IsValid().Should().BeFalse();
   }

   [Fact]
   public void IsValid_returns_true_for_unnamed_flags_combination()
   {
      var ab = FlagsNoCombined.A | FlagsNoCombined.B; // value 3 not declared
      ab.IsValid().Should().BeTrue();
   }

   [Fact]
   public void IsValid_returns_true_for_named_flags_combination()
   {
      var ab = FlagsWithCombined.A | FlagsWithCombined.B; // equals FlagsWithCombined.AB
      ab.IsValid().Should().BeTrue();
   }
}
