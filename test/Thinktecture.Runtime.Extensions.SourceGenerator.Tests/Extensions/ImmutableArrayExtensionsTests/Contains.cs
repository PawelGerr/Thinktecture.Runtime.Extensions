using System.Collections.Generic;
using System.Collections.Immutable;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class Contains
{
   [Fact]
   public void Should_return_false_if_array_is_empty()
   {
      var array = ImmutableArray<int>.Empty;

      array.Contains(0, EqualityComparer<int>.Default).Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_if_array_doesnt_contain_item()
   {
      var array = ImmutableArray.CreateRange(new[] { 1, 2 });

      array.Contains(0, EqualityComparer<int>.Default).Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_if_array_contains_element()
   {
      var array = ImmutableArray.Create(new[] { 1, 0, 2 });

      array.Contains(0, EqualityComparer<int>.Default).Should().BeTrue();
   }
}
