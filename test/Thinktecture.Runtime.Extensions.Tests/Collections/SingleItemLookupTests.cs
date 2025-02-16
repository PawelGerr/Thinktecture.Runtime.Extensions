using System.Linq;

namespace Thinktecture.Runtime.Tests.Collections;

public class SingleItemLookupTests
{
   private readonly ILookup<int, int> _sut = SingleItem.Lookup(42, [43, 44]);

   [Fact]
   public void Should_have_count_of_1()
   {
      _sut.Count.Should().Be(1);
   }

   [Fact]
   public void Should_have_correct_indexer_behavior()
   {
      _sut[-1].Should().BeEmpty();
      _sut[1].Should().BeEmpty();

      _sut[42].Should().BeEquivalentTo([43, 44]);
   }

   [Fact]
   public void Should_have_ContainsKey()
   {
      _sut.Contains(0).Should().BeFalse();
      _sut.Contains(42).Should().BeTrue();
      _sut.Contains(43).Should().BeFalse();
   }

   [Fact]
   public void Should_have_correct_enumerator_behavior()
   {
      using var enumerator = _sut.GetEnumerator();

      enumerator.Current.Should().BeNull();

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current!.Key.Should().Be(42);
      enumerator.Current.Should().BeEquivalentTo([43, 44]);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().BeNull();

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().BeNull();

      enumerator.Reset();

      enumerator.Current.Should().BeNull();

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current!.Key.Should().Be(42);
      enumerator.Current.Should().BeEquivalentTo([43, 44]);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().BeNull();

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().BeNull();
   }
}
