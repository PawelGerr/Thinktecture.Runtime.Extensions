using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.SetComparerTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_0_for_default_array()
   {
      // Arrange
      var comparer = SetComparer<int>.Instance;
      var obj = default(ImmutableArray<int>);

      // Act
      var hash = comparer.GetHashCode(obj);

      // Assert
      hash.Should().Be(0);
   }

   [Fact]
   public void Should_return_0_for_empty_array()
   {
      // Arrange
      var comparer = SetComparer<int>.Instance;
      var obj = ImmutableArray<int>.Empty;

      // Act
      var hash = comparer.GetHashCode(obj);

      // Assert
      hash.Should().Be(0);
   }

   [Fact]
   public void Should_return_same_hash_for_default_and_empty()
   {
      // Arrange
      var comparer = SetComparer<int>.Instance;
      var @default = default(ImmutableArray<int>);
      var empty = ImmutableArray<int>.Empty;

      // Act
      var h1 = comparer.GetHashCode(@default);
      var h2 = comparer.GetHashCode(empty);

      // Assert
      h1.Should().Be(h2).And.Be(0);
   }

   [Fact]
   public void Should_be_based_on_length_not_on_order()
   {
      // Arrange
      var comparer = SetComparer<int>.Instance;
      var x = ImmutableArray.Create(1, 2, 3, 4);
      var y = ImmutableArray.Create(4, 3, 2, 1);

      // Sanity: Equals should be true for same elements regardless of order
      comparer.Equals(x, y).Should().BeTrue();

      // Act
      var hx = comparer.GetHashCode(x);
      var hy = comparer.GetHashCode(y);

      // Assert
      hx.Should().Be(hy);
      hx.Should().Be(x.Length.GetHashCode());
   }

   [Fact]
   public void Should_return_different_hash_for_different_lengths()
   {
      // Arrange
      var comparer = SetComparer<int>.Instance;
      var x = ImmutableArray.Create(1, 2, 3);
      var y = ImmutableArray.Create(1, 2, 3, 4);

      // Act
      var hx = comparer.GetHashCode(x);
      var hy = comparer.GetHashCode(y);

      // Assert
      hx.Should().NotBe(hy);
      hx.Should().Be(x.Length.GetHashCode());
      hy.Should().Be(y.Length.GetHashCode());
   }

   [Fact]
   public void Equal_arrays_must_have_same_hash_even_with_duplicates()
   {
      // Arrange
      // Note: The documentation states arrays must not contain duplicates,
      // but we verify the current behavior regardless.
      var comparer = SetComparer<int>.Instance;
      var x = ImmutableArray.Create(1, 2, 2, 3);
      var y = ImmutableArray.Create(3, 2, 1, 2);

      // Sanity: Equals should treat these as equal with current implementation
      comparer.Equals(x, y).Should().BeTrue();

      // Act
      var hx = comparer.GetHashCode(x);
      var hy = comparer.GetHashCode(y);

      // Assert
      hx.Should().Be(hy);
      hx.Should().Be(x.Length.GetHashCode());
   }

   [Fact]
   public void Unequal_arrays_can_have_same_hash_if_lengths_match()
   {
      // Arrange
      var comparer = SetComparer<int>.Instance;
      var x = ImmutableArray.Create(1, 2, 3);
      var y = ImmutableArray.Create(1, 2, 4);

      // Sanity: arrays are not equal
      comparer.Equals(x, y).Should().BeFalse();

      // Act
      var hx = comparer.GetHashCode(x);
      var hy = comparer.GetHashCode(y);

      // Assert
      hx.Should().Be(hy); // length-based hash
      hx.Should().Be(3.GetHashCode());
   }

   [Fact]
   public void Works_with_reference_types()
   {
      // Arrange
      var comparer = SetComparer<string>.Instance;
      var x = ImmutableArray.Create("a", "b", "c");
      var y = ImmutableArray.Create("c", "b", "a");

      comparer.Equals(x, y).Should().BeTrue();

      // Act
      var hx = comparer.GetHashCode(x);
      var hy = comparer.GetHashCode(y);

      // Assert
      hx.Should().Be(hy);
      hx.Should().Be(x.Length.GetHashCode());
   }
}
