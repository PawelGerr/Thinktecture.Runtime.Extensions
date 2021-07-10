using System;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Runtime.Tests.Extensions.StringExtensionsTests
{
   public class TrimOrNullify
   {
      [Fact]
      public void Should_return_null_when_value_is_null()
      {
         string value = null;

         value.TrimOrNullify().Should().BeNull();
         value.TrimOrNullify(42).Should().BeNull();
      }

      [Fact]
      public void Should_return_null_when_value_is_empty()
      {
         var value = String.Empty;

         value.TrimOrNullify().Should().BeNull();
         value.TrimOrNullify(42).Should().BeNull();
      }

      [Fact]
      public void Should_return_null_when_value_is_whitespace()
      {
         var value = " ";

         value.TrimOrNullify().Should().BeNull();
         value.TrimOrNullify(42).Should().BeNull();

         value = Environment.NewLine;

         value.TrimOrNullify().Should().BeNull();
         value.TrimOrNullify(42).Should().BeNull();

         value = $" {Environment.NewLine} ";

         value.TrimOrNullify().Should().BeNull();
         value.TrimOrNullify(42).Should().BeNull();
      }

      [Fact]
      public void Should_return_value_without_whitespaces_as_is()
      {
         var value = "foo";

         value.TrimOrNullify().Should().Be(value);
      }

      [Fact]
      public void Should_return_trimmed_value()
      {
         var value = " foo ";

         value.TrimOrNullify().Should().Be("foo");
      }

      [Fact]
      public void Should_not_shorten_value_if_it_is_shorted_than_maxlength()
      {
         var value = "foo";

         value.TrimOrNullify(4).Should().Be("foo");
      }

      [Fact]
      public void Should_not_shorten_value_if_it_is_as_long_as_maxlength()
      {
         var value = "foo";

         value.TrimOrNullify(3).Should().Be("foo");
      }

      [Fact]
      public void Should_shorten_value_if_it_is_longer_as_maxlength()
      {
         var value = "foo";

         value.TrimOrNullify(2).Should().Be("fo");
      }

      [Fact]
      public void Should_shorten_value_after_trim()
      {
         var value = " foo ";

         value.TrimOrNullify(2).Should().Be("fo");
      }

      [Fact]
      public void Should_throw_if_maxlength_is_0()
      {
         "foo".Invoking(s => s.TrimOrNullify(0))
              .Should().Throw<ArgumentException>().WithMessage("The maximum length must be bigger than 0. (Parameter 'maxLength')");
      }

      [Fact]
      public void Should_throw_if_maxlength_is_negative()
      {
         "foo".Invoking(s => s.TrimOrNullify(-1))
              .Should().Throw<ArgumentException>().WithMessage("The maximum length must be bigger than 0. (Parameter 'maxLength')");
      }
   }
}
