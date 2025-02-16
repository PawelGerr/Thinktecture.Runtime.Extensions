using System.Collections.Immutable;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class Distinct
{
   [Fact]
   public void Should_return_empty_array_if_array_is_default()
   {
      ImmutableArray<int> array = default;

      array.Distinct().IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_empty_array_if_array_is_empty()
   {
      var array = ImmutableArray<int>.Empty;

      array.Distinct().IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_same_array_if_it_contains_no_duplicates()
   {
      var array = ImmutableArray.CreateRange([1, 2, 3]);

      array.Distinct().Should().BeEquivalentTo([1, 2, 3]);
   }

   [Theory]
   [InlineData(new[] { 1, 1, 2, 3 }, new[] { 1, 2, 3 })]
   [InlineData(new[] { 1, 2, 2, 3 }, new[] { 1, 2, 3 })]
   [InlineData(new[] { 1, 2, 3, 3 }, new[] { 1, 2, 3 })]
   [InlineData(new[] { 3, 1, 2, 3 }, new[] { 1, 2, 3 })]
   [InlineData(new[] { 3, 2, 3, 1 }, new[] { 1, 2, 3 })]
   [InlineData(new[] { 1, 2, 3, 1 }, new[] { 1, 2, 3 })]
   public void Should_remove_duplicates(int[] input, int[] expected)
   {
      var array = ImmutableArray.CreateRange(input);

      array.Distinct().Should().BeEquivalentTo(expected);
   }
}
