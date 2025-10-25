using System;
using System.Collections.Immutable;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class DistinctWithComparer
{
   [Fact]
   public void Should_use_custom_comparer_case_insensitive()
   {
      var array = ImmutableArray.CreateRange(["a", "A", "b", "B", "c"]);

      var result = array.Distinct(StringComparer.OrdinalIgnoreCase);

      result.Should().BeEquivalentTo(new[] { "a", "b", "c" });
   }

   [Fact]
   public void Should_not_deduplicate_with_default_comparer_case_sensitive()
   {
      var array = ImmutableArray.CreateRange(["a", "A"]);

      var result = array.Distinct();

      result.Should().BeEquivalentTo("a", "A");
   }
}
