using System;
using Thinktecture.Collections;

namespace Thinktecture.Runtime.Tests.Collections.ProjectionEqualityComparerTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_hashcode_of_nulls()
   {
      var comparer = new ProjectionEqualityComparer<string, string>(s => s);

      comparer.GetHashCode(null).Should().Be(0);
   }

   [Fact]
   public void Should_use_provided_comparer()
   {
      var caseInsensitiveComparer = new ProjectionEqualityComparer<string, string>(s => s, StringComparer.OrdinalIgnoreCase);
      caseInsensitiveComparer.GetHashCode("value").Should().Be(caseInsensitiveComparer.GetHashCode("VALUE"));

      var caseSensitiveComparer = new ProjectionEqualityComparer<string, string>(s => s, StringComparer.Ordinal);
      caseSensitiveComparer.GetHashCode("value").Should().NotBe(caseSensitiveComparer.GetHashCode("VALUE"));
   }

   [Fact]
   public void Should_return_hashcode_of_projected_objects()
   {
      var comparer = new ProjectionEqualityComparer<string, string>(s => s.Substring(0, 3));
      comparer.GetHashCode("value").Should().Be(comparer.GetHashCode("val"));
   }

   [Fact]
   public void Should_throw_if_projection_throws()
   {
      var comparer = new ProjectionEqualityComparer<string, string>(s => s.Substring(0, 10));

      comparer.Invoking(c => c.GetHashCode("value"))
              .Should().Throw<ArgumentOutOfRangeException>();
   }
}
