using System.Collections.Immutable;

namespace Thinktecture.Runtime.Tests.ImmutableArrayExtensionsTests;

public class ComputeHashCodeTests
{
   [Fact]
   public void Should_return_0_for_default_array_T_items()
   {
      ImmutableArray<TestItem> array = default;

      array.ComputeHashCode().Should().Be(0);
   }

   [Fact]
   public void Should_return_0_for_empty_array_T_items()
   {
      var array = ImmutableArray<TestItem>.Empty;

      array.ComputeHashCode().Should().Be(0);
   }

   [Fact]
   public void Should_aggregate_hash_for_T_items()
   {
      var array = ImmutableArray.CreateRange([new TestItem(1), new TestItem(2), new TestItem(3)]);

      var expected = array[0].GetHashCode();
      expected = (expected * 397) ^ array[1].GetHashCode();
      expected = (expected * 397) ^ array[2].GetHashCode();

      array.ComputeHashCode().Should().Be(expected);
   }

   private sealed class TestItem(int value) : IHashCodeComputable
   {
      private int Value { get; } = value;

      public override int GetHashCode() => Value;
   }
}
