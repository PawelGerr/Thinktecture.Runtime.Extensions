using System;
using Thinktecture.Collections;

namespace Thinktecture.Runtime.Tests.Collections.ProjectionEqualityComparerTests;

public class Equals
{
   [Fact]
   public void Should_compare_nulls()
   {
      var comparer = new ProjectionEqualityComparer<string, string>(s => s);

      comparer.Equals(null, null).Should().BeTrue();
      comparer.Equals("value", null).Should().BeFalse();
      comparer.Equals(null, "value").Should().BeFalse();
   }

   [Fact]
   public void Should_compare_original_item_if_selector_returns_it()
   {
      var comparer = new ProjectionEqualityComparer<string, string>(s => s);

      comparer.Equals("value", "value").Should().BeTrue();
      comparer.Equals("value", "othervalue").Should().BeFalse();
   }

   [Fact]
   public void Should_use_provided_comparer()
   {
      var caseInsensitiveComparer = new ProjectionEqualityComparer<string, string>(s => s, StringComparer.OrdinalIgnoreCase);
      caseInsensitiveComparer.Equals("value", "VALUE").Should().BeTrue();

      var caseSensitiveComparer = new ProjectionEqualityComparer<string, string>(s => s, StringComparer.Ordinal);
      caseSensitiveComparer.Equals("value", "VALUE").Should().BeFalse();
   }

   [Theory]
   [InlineData("value", "value")]
   [InlineData("value", "val")]
   [InlineData("val", "value")]
   [InlineData("val", "val")]
   public void Should_compare_projected_objects(string value, string otherValue)
   {
      var comparer = new ProjectionEqualityComparer<string, string>(s => s.Substring(0, 3));
      comparer.Equals(value, otherValue).Should().BeTrue();
   }

   [Fact]
   public void Should_throw_if_projection_throws()
   {
      var comparer = new ProjectionEqualityComparer<string, string>(s => s.Substring(0, 10));

      comparer.Invoking(c => c.Equals("value", "value"))
              .Should().Throw<ArgumentOutOfRangeException>();
   }
}
