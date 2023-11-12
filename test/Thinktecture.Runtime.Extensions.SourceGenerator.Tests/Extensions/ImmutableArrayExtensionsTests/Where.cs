using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class Where
{
   [Fact]
   public void Should_return_empty_array_if_array_is_default()
   {
      ImmutableArray<int> array = default;

      array.Where((i, arg) => true, 42)
           .IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_empty_array_if_array_is_empty()
   {
      var array = ImmutableArray<int>.Empty;

      array.Where((i, arg) => true, 42)
           .IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_empty_array_if_all_items_are_removed()
   {
      var array = ImmutableArray.CreateRange(new[] { 1, 2, 3 });

      array.Where((i, arg) => false, 42)
           .IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_same_array_if_no_items_are_removed()
   {
      var array = ImmutableArray.CreateRange(new[] { 1, 2, 3 });

      array.Where((i, arg) => true, 42)
           .Should().BeEquivalentTo(new[] { 1, 2, 3 });
   }

   [Theory]
   [InlineData(1)]
   [InlineData(2)]
   [InlineData(3)]
   public void Should_return_array_without_excluded_value(int valueToExclude)
   {
      var array = ImmutableArray.CreateRange(new[] { 1, 2, 3 });
      var expected = new[] { 1, 2, 3 }.Where(i => i != valueToExclude);

      array.Where((i, arg) => i != valueToExclude && arg == 42, 42)
           .Should().BeEquivalentTo(expected);
   }

   [Fact]
   public void Should_return_array_without_excluded_values()
   {
      ImmutableArray.CreateRange(new[] { 1, 2, 1, 3 })
                    .Where(i => i != 1)
                    .Should().BeEquivalentTo(new[] { 2, 3 });

      ImmutableArray.CreateRange(new[] { 2, 1, 3, 1, 2 })
                    .Where(i => i != 1)
                    .Should().BeEquivalentTo(new[] { 2, 3, 2 });
   }
}
