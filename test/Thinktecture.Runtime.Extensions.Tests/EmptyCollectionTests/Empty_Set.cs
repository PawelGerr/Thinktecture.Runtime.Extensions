using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.EmptyCollectionTests
{
   // ReSharper disable once InconsistentNaming
   public class Empty_Set
   {
      // ReSharper disable once MemberCanBeMadeStatic.Local
      // ReSharper disable once InconsistentNaming
      private IReadOnlySet<object> SUT => Empty.Set<object>();

      // ReSharper disable once CollectionNeverUpdated.Local
      private readonly HashSet<object> _realEmptySet = new();
      private readonly HashSet<object> _realNonEmptySet = new() { 1 };

      [Fact]
      public void Should_not_be_null()
      {
         SUT.Should().NotBeNull();
      }

      [Fact]
      public void Should_be_empty()
      {
         SUT.Should().BeEmpty();
      }

      [Fact]
      public void Should_have_count_of_0()
      {
         SUT.Count.Should().Be(0);
      }

      [Fact]
      public void Should_have_implementation_for_IsProperSubsetOf()
      {
         SUT.IsProperSubsetOf(_realEmptySet).Should().Be(_realEmptySet.IsProperSubsetOf(_realEmptySet));
         SUT.IsProperSubsetOf(_realNonEmptySet).Should().Be(_realEmptySet.IsProperSubsetOf(_realNonEmptySet));
      }

      [Fact]
      public void Should_have_implementation_for_IsSubsetOf()
      {
         SUT.IsSubsetOf(_realEmptySet).Should().Be(_realEmptySet.IsSubsetOf(_realEmptySet));
         SUT.IsSubsetOf(_realNonEmptySet).Should().Be(_realEmptySet.IsSubsetOf(_realNonEmptySet));
      }

      [Fact]
      public void Should_have_implementation_for_IsProperSupersetOf()
      {
         SUT.IsProperSupersetOf(_realEmptySet).Should().Be(_realEmptySet.IsProperSupersetOf(_realEmptySet));
         SUT.IsProperSupersetOf(_realNonEmptySet).Should().Be(_realEmptySet.IsProperSupersetOf(_realNonEmptySet));
      }

      [Fact]
      public void Should_have_implementation_for_IsSupersetOf()
      {
         SUT.IsSupersetOf(_realEmptySet).Should().Be(_realEmptySet.IsSupersetOf(_realEmptySet));
         SUT.IsSupersetOf(_realNonEmptySet).Should().Be(_realEmptySet.IsSupersetOf(_realNonEmptySet));
      }

      [Fact]
      public void Should_have_implementation_for_Overlaps()
      {
         SUT.Overlaps(_realEmptySet).Should().Be(_realEmptySet.Overlaps(_realEmptySet));
         SUT.Overlaps(_realNonEmptySet).Should().Be(_realEmptySet.Overlaps(_realNonEmptySet));
      }

      [Fact]
      public void Should_have_implementation_for_SetEquals()
      {
         SUT.SetEquals(_realEmptySet).Should().Be(_realEmptySet.SetEquals(_realEmptySet));
         SUT.SetEquals(_realNonEmptySet).Should().Be(_realEmptySet.SetEquals(_realNonEmptySet));
      }
   }
}
