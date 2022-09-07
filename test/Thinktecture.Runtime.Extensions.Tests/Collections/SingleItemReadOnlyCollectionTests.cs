using System;
using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.Collections;

public class SingleItemReadOnlyCollectionTests
{
   private readonly IReadOnlyList<int> _sut = SingleItem.Collection(42);

   [Fact]
   public void Should_have_count_of_1()
   {
      _sut.Count.Should().Be(1);
   }

   [Fact]
   public void Should_have_correct_indexer_behavior()
   {
      _sut.Invoking(sut => sut[-1]).Should().Throw<ArgumentOutOfRangeException>().WithMessage("Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')");
      _sut.Invoking(sut => sut[1]).Should().Throw<ArgumentOutOfRangeException>().WithMessage("Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')");

      _sut[0].Should().Be(42);
   }

   [Fact]
   public void Should_have_correct_enumerator_behavior()
   {
      using var enumerator = _sut.GetEnumerator();

      enumerator.Current.Should().Be(0);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(0);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(0);

      enumerator.Reset();

      enumerator.Current.Should().Be(0);

      enumerator.MoveNext().Should().BeTrue();
      enumerator.Current.Should().Be(42);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(0);

      enumerator.MoveNext().Should().BeFalse();
      enumerator.Current.Should().Be(0);
   }
}
