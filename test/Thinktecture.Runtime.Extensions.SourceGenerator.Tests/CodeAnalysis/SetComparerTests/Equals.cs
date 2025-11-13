using System.Linq;
using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.SetComparerTests;

public class Equals
{
   [Fact]
   public void Should_return_true_when_both_arrays_are_default()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = default(ImmutableArray<TestItem>);
      var y = default(ImmutableArray<TestItem>);

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_both_arrays_are_empty()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray<TestItem>.Empty;
      var y = ImmutableArray<TestItem>.Empty;

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_one_is_default_and_other_is_empty()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = default(ImmutableArray<TestItem>);
      var y = ImmutableArray<TestItem>.Empty;

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_first_is_default_and_second_has_elements()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = default(ImmutableArray<TestItem>);
      var y = ImmutableArray.Create(new TestItem(1));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_first_has_elements_and_second_is_default()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1));
      var y = default(ImmutableArray<TestItem>);

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_first_is_empty_and_second_has_elements()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray<TestItem>.Empty;
      var y = ImmutableArray.Create(new TestItem(1));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_first_has_elements_and_second_is_empty()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1));
      var y = ImmutableArray<TestItem>.Empty;

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_arrays_have_different_lengths()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2));
      var y = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(3));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_arrays_have_same_elements_in_same_order()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(3));
      var y = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(3));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_arrays_have_same_elements_in_different_order()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(3));
      var y = ImmutableArray.Create(new TestItem(3), new TestItem(1), new TestItem(2));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_arrays_have_same_length_but_different_elements()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(3));
      var y = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(4));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_single_element_arrays_have_same_element()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1));
      var y = ImmutableArray.Create(new TestItem(1));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_single_element_arrays_have_different_elements()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1));
      var y = ImmutableArray.Create(new TestItem(2));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_large_arrays_correctly()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var items = Enumerable.Range(1, 100).Select(i => new TestItem(i)).ToArray();
      var x = ImmutableArray.Create(items);
      var y = ImmutableArray.Create(items.Reverse().ToArray());

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_large_arrays_differ_by_one_element()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var itemsX = Enumerable.Range(1, 100).Select(i => new TestItem(i)).ToArray();
      var itemsY = Enumerable.Range(1, 100).Select(i => i == 50 ? new TestItem(999) : new TestItem(i)).ToArray();
      var x = ImmutableArray.Create(itemsX);
      var y = ImmutableArray.Create(itemsY);

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_work_with_duplicate_elements_returning_true_when_duplicates_match()
   {
      // Arrange
      // Note: According to the class documentation, arrays should not contain duplicates
      // but we test the behavior anyway
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(2), new TestItem(3));
      var y = ImmutableArray.Create(new TestItem(3), new TestItem(2), new TestItem(1), new TestItem(2));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_one_array_has_duplicates_and_other_does_not()
   {
      // Arrange
      // Note: According to the class documentation, arrays should not contain duplicates
      // but we test the behavior anyway
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(3));
      var y = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(2));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_work_with_string_elements()
   {
      // Arrange
      var comparer = SetComparer<string>.Instance;
      var x = ImmutableArray.Create("apple", "banana", "cherry");
      var y = ImmutableArray.Create("cherry", "apple", "banana");

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_string_arrays_with_different_elements()
   {
      // Arrange
      var comparer = SetComparer<string>.Instance;
      var x = ImmutableArray.Create("apple", "banana", "cherry");
      var y = ImmutableArray.Create("apple", "banana", "date");

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_first_element_not_in_second_array()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2));
      var y = ImmutableArray.Create(new TestItem(3), new TestItem(4));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_partial_overlap_exists()
   {
      // Arrange
      var comparer = SetComparer<TestItem>.Instance;
      var x = ImmutableArray.Create(new TestItem(1), new TestItem(2), new TestItem(3));
      var y = ImmutableArray.Create(new TestItem(2), new TestItem(3), new TestItem(4));

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_both_arrays_contain_nulls_in_different_order()
   {
      // Arrange
      var comparer = SetComparer<string>.Instance;
      var x = Create("a", null, "b");
      var y = Create(null, "b", "a");

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_only_one_array_contains_null()
   {
      // Arrange
      var comparer = SetComparer<string>.Instance;
      var x = Create("a", null);
      var y = Create("a", "b");

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_both_arrays_contain_only_nulls_even_if_duplicated()
   {
      // Arrange
      // Note: Documentation says arrays must not contain duplicates,
      // but we verify current behavior with duplicates anyway.
      var comparer = SetComparer<string>.Instance;
      var x = Create(null, null);
      var y = Create(null, null);

      // Act
      var result = comparer.Equals(x, y);

      // Assert
      result.Should().BeTrue();
   }

   private static ImmutableArray<string> Create(params string?[] items)
   {
      var b = ImmutableArray.CreateBuilder<string>(items.Length);
      foreach (var s in items)
         b.Add(s!);
      return b.ToImmutable();
   }

   private sealed record TestItem(int Value);
}
