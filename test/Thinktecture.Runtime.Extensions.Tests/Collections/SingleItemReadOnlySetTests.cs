using System;
using System.Collections.Generic;
using System.Linq;

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
      _sut.IsProperSubsetOf(Enumerable.Empty<int>()).Should().BeFalse();
      _sut.IsProperSubsetOf(Array.Empty<int>()).Should().BeFalse();
      _sut.IsProperSubsetOf(new[] { 1 }).Should().BeFalse();
      _sut.IsProperSubsetOf(new[] { 42 }).Should().BeFalse();
      _sut.IsProperSubsetOf(new[] { 1, 2 }).Should().BeFalse();
      _sut.IsProperSubsetOf(new[] { 42, 42 }).Should().BeFalse();
      _sut.IsProperSubsetOf(new[] { 42, 43 }).Should().BeTrue();
      _sut.IsProperSubsetOf(new[] { 42, 43, 42 }).Should().BeTrue();
   }

   [Fact]
   public void Should_have_IsProperSupersetOf()
   {
      _sut.IsProperSupersetOf(Enumerable.Empty<int>()).Should().BeTrue();
      _sut.IsProperSupersetOf(Array.Empty<int>()).Should().BeTrue();
      _sut.IsProperSupersetOf(new[] { 1 }).Should().BeFalse();
      _sut.IsProperSupersetOf(new[] { 42 }).Should().BeFalse();
      _sut.IsProperSupersetOf(new[] { 1, 2 }).Should().BeFalse();
      _sut.IsProperSupersetOf(new[] { 42, 42 }).Should().BeFalse();
      _sut.IsProperSupersetOf(new[] { 42, 43 }).Should().BeFalse();
   }

   [Fact]
   public void Should_have_IsSubsetOf()
   {
      _sut.IsSubsetOf(Enumerable.Empty<int>()).Should().BeFalse();
      _sut.IsSubsetOf(Array.Empty<int>()).Should().BeFalse();
      _sut.IsSubsetOf(new[] { 1 }).Should().BeFalse();
      _sut.IsSubsetOf(new[] { 42 }).Should().BeTrue();
      _sut.IsSubsetOf(new[] { 1, 2 }).Should().BeFalse();
      _sut.IsSubsetOf(new[] { 42, 42 }).Should().BeTrue();
      _sut.IsSubsetOf(new[] { 42, 43 }).Should().BeTrue();
      _sut.IsSubsetOf(new[] { 42, 43, 42 }).Should().BeTrue();
   }

   [Fact]
   public void Should_have_IsSupersetOf()
   {
      _sut.IsSupersetOf(Enumerable.Empty<int>()).Should().BeTrue();
      _sut.IsSupersetOf(Array.Empty<int>()).Should().BeTrue();
      _sut.IsSupersetOf(new[] { 1 }).Should().BeFalse();
      _sut.IsSupersetOf(new[] { 42 }).Should().BeTrue();
      _sut.IsSupersetOf(new[] { 1, 2 }).Should().BeFalse();
      _sut.IsSupersetOf(new[] { 42, 42 }).Should().BeTrue();
      _sut.IsSupersetOf(new[] { 42, 43 }).Should().BeFalse();
   }

   [Fact]
   public void Should_have_Overlaps()
   {
      _sut.Overlaps(Enumerable.Empty<int>()).Should().BeFalse();
      _sut.Overlaps(Array.Empty<int>()).Should().BeFalse();
      _sut.Overlaps(new[] { 1 }).Should().BeFalse();
      _sut.Overlaps(new[] { 42 }).Should().BeTrue();
      _sut.Overlaps(new[] { 1, 2 }).Should().BeFalse();
      _sut.Overlaps(new[] { 42, 42 }).Should().BeTrue();
      _sut.Overlaps(new[] { 42, 43 }).Should().BeTrue();
   }

   [Fact]
   public void Should_have_SetEquals()
   {
      _sut.SetEquals(Enumerable.Empty<int>()).Should().BeFalse();
      _sut.SetEquals(Array.Empty<int>()).Should().BeFalse();
      _sut.SetEquals(new[] { 1 }).Should().BeFalse();
      _sut.SetEquals(new[] { 42 }).Should().BeTrue();
      _sut.SetEquals(new[] { 1, 2 }).Should().BeFalse();
      _sut.SetEquals(new[] { 42, 42 }).Should().BeTrue();
      _sut.SetEquals(new[] { 42, 43 }).Should().BeFalse();
   }
}
