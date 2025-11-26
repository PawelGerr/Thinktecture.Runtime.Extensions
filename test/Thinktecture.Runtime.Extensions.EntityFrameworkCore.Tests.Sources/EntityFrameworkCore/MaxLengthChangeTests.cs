using System;
using System.Collections.Generic;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore;

public class MaxLengthChangeTests
{
   [Fact]
   public void None_should_have_IsSet_false()
   {
      MaxLengthChange.None.IsSet.Should().BeFalse();
   }

   [Fact]
   public void None_should_throw_when_accessing_Value()
   {
      var act = () => MaxLengthChange.None.Value;

      act.Should().Throw<InvalidOperationException>()
         .WithMessage("Value is not set.");
   }

   [Fact]
   public void Constructor_with_positive_value_should_set_IsSet_true()
   {
      var change = new MaxLengthChange(50);

      change.IsSet.Should().BeTrue();
      change.Value.Should().Be(50);
   }

   [Fact]
   public void Constructor_with_zero_should_set_IsSet_true()
   {
      var change = new MaxLengthChange(0);

      change.IsSet.Should().BeTrue();
      change.Value.Should().Be(0);
   }

   [Fact]
   public void Constructor_with_negative_value_should_set_IsSet_true()
   {
      var change = new MaxLengthChange(-1);

      change.IsSet.Should().BeTrue();
      change.Value.Should().Be(-1);
   }

   [Fact]
   public void Constructor_with_null_should_set_IsSet_true_and_Value_null()
   {
      var change = new MaxLengthChange(null);

      change.IsSet.Should().BeTrue();
      change.Value.Should().BeNull();
   }

   [Fact]
   public void Implicit_conversion_from_int_should_create_MaxLengthChange()
   {
      MaxLengthChange change = 100;

      change.IsSet.Should().BeTrue();
      change.Value.Should().Be(100);
   }

   [Fact]
   public void Implicit_conversion_from_null_should_create_Clear_MaxLengthChange()
   {
      int? nullValue = null;
      MaxLengthChange change = nullValue;

      change.IsSet.Should().BeTrue();
      change.Value.Should().BeNull();
   }

   [Fact]
   public void Default_MaxLengthChange_should_be_None()
   {
      MaxLengthChange change = default;

      change.IsSet.Should().BeFalse();
   }

   [Fact]
   public void Value_should_throw_when_not_set()
   {
      MaxLengthChange change = default;

      var act = () => change.Value;

      act.Should().Throw<InvalidOperationException>()
         .WithMessage("Value is not set.");
   }
}
