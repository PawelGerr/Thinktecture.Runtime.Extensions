using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Thinktecture.EmptyCollectionTests
{
   // ReSharper disable once InconsistentNaming
   public class Empty_Collection
   {
      private IReadOnlyList<object> SUT => Empty.Collection<object>();

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
      public void Should_throw_if_using_indexer()
      {
         // ReSharper disable once AssignmentIsFullyDiscarded
         Action action = () => _ = SUT[0];
         action.Should().Throw<ArgumentOutOfRangeException>();
      }
   }
}
