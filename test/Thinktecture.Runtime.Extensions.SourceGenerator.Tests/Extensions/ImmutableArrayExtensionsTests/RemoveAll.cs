using System.Collections.Immutable;
using System.Linq;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class RemoveAll
{
   [Fact]
   public void Should_return_empty_array_if_array_is_default()
   {
      ImmutableArray<int> array = default;

      array.RemoveAll((i, arg) => true, 42)
           .IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_empty_array_if_array_is_empty()
   {
      var array = ImmutableArray<int>.Empty;

      array.RemoveAll((i, arg) => true, 42)
           .IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_empty_array_if_all_items_are_removed()
   {
      var array = ImmutableArray.CreateRange([1, 2, 3]);

      array.RemoveAll((i, arg) => true, 42)
           .IsEmpty.Should().BeTrue();
   }

   [Fact]
   public void Should_return_same_array_if_no_items_are_removed()
   {
      var array = ImmutableArray.CreateRange([1, 2, 3]);

      array.RemoveAll((i, arg) => false, 42)
           .Should().BeEquivalentTo([1, 2, 3]);
   }

   [Theory]
   [InlineData(1)]
   [InlineData(2)]
   [InlineData(3)]
   public void Should_return_array_without_excluded_value(int valueToExclude)
   {
      var array = ImmutableArray.CreateRange([1, 2, 3]);
      var expected = new[] { 1, 2, 3 }.Where(i => i != valueToExclude);

      array.RemoveAll((i, arg) => i == valueToExclude && arg == 42, 42)
           .Should().BeEquivalentTo(expected);
   }
}
