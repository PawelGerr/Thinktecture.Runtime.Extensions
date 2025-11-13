using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.Collections;

public class SingleItemReadOnlySetTests
{
   private readonly IReadOnlySet<int> _sut = SingleItem.Set(42);

   [Fact]
   public void Should_have_count_of_1()
   {
      _sut.Count.Should().Be(1);
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

   [Fact]
   public void Should_have_Contains()
   {
      _sut.Contains(0).Should().BeFalse();
      _sut.Contains(1).Should().BeFalse();
      _sut.Contains(42).Should().BeTrue();
   }

   [Fact]
   public void Should_have_IsProperSubsetOf()
   {
      _sut.IsProperSubsetOf([]).Should().BeFalse();
      _sut.IsProperSubsetOf([]).Should().BeFalse();
      _sut.IsProperSubsetOf([1]).Should().BeFalse();
      _sut.IsProperSubsetOf([42]).Should().BeFalse();
      _sut.IsProperSubsetOf([1, 2]).Should().BeFalse();
      _sut.IsProperSubsetOf([42, 42]).Should().BeFalse();
      _sut.IsProperSubsetOf([42, 43]).Should().BeTrue();
      _sut.IsProperSubsetOf([42, 43, 42]).Should().BeTrue();
   }

   [Fact]
   public void Should_have_IsProperSupersetOf()
   {
      _sut.IsProperSupersetOf([]).Should().BeTrue();
      _sut.IsProperSupersetOf([]).Should().BeTrue();
      _sut.IsProperSupersetOf([1]).Should().BeFalse();
      _sut.IsProperSupersetOf([42]).Should().BeFalse();
      _sut.IsProperSupersetOf([1, 2]).Should().BeFalse();
      _sut.IsProperSupersetOf([42, 42]).Should().BeFalse();
      _sut.IsProperSupersetOf([42, 43]).Should().BeFalse();
   }

   [Fact]
   public void Should_have_IsSubsetOf()
   {
      _sut.IsSubsetOf([]).Should().BeFalse();
      _sut.IsSubsetOf([]).Should().BeFalse();
      _sut.IsSubsetOf([1]).Should().BeFalse();
      _sut.IsSubsetOf([42]).Should().BeTrue();
      _sut.IsSubsetOf([1, 2]).Should().BeFalse();
      _sut.IsSubsetOf([42, 42]).Should().BeTrue();
      _sut.IsSubsetOf([42, 43]).Should().BeTrue();
      _sut.IsSubsetOf([42, 43, 42]).Should().BeTrue();
   }

   [Fact]
   public void Should_have_IsSupersetOf()
   {
      _sut.IsSupersetOf([]).Should().BeTrue();
      _sut.IsSupersetOf([]).Should().BeTrue();
      _sut.IsSupersetOf([1]).Should().BeFalse();
      _sut.IsSupersetOf([42]).Should().BeTrue();
      _sut.IsSupersetOf([1, 2]).Should().BeFalse();
      _sut.IsSupersetOf([42, 42]).Should().BeTrue();
      _sut.IsSupersetOf([42, 43]).Should().BeFalse();
   }

   [Fact]
   public void Should_have_Overlaps()
   {
      _sut.Overlaps([]).Should().BeFalse();
      _sut.Overlaps([]).Should().BeFalse();
      _sut.Overlaps([1]).Should().BeFalse();
      _sut.Overlaps([42]).Should().BeTrue();
      _sut.Overlaps([1, 2]).Should().BeFalse();
      _sut.Overlaps([42, 42]).Should().BeTrue();
      _sut.Overlaps([42, 43]).Should().BeTrue();
   }

   [Fact]
   public void Should_have_SetEquals()
   {
      _sut.SetEquals([]).Should().BeFalse();
      _sut.SetEquals([]).Should().BeFalse();
      _sut.SetEquals([1]).Should().BeFalse();
      _sut.SetEquals([42]).Should().BeTrue();
      _sut.SetEquals([1, 2]).Should().BeFalse();
      _sut.SetEquals([42, 42]).Should().BeTrue();
      _sut.SetEquals([42, 43]).Should().BeFalse();
   }
}
