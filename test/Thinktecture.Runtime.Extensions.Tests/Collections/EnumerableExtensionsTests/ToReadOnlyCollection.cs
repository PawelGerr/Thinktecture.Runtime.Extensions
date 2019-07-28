using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Collections.EnumerableExtensionsTests
{
   public class ToReadOnlyCollection
   {
      [Fact]
      public void Shoud_throw_if_items_is_null()
      {
         ((IEnumerable<int>)null).Invoking(e => e.ToReadOnlyCollection(42))
                                 .Should().Throw<ArgumentNullException>();
      }

      [Fact]
      public void Shoud_throw_if_count_is_negative()
      {
         Enumerable.Empty<int>().Invoking(e => e.ToReadOnlyCollection(-1))
                   .Should().Throw<ArgumentException>();
      }

      [Theory]
      [InlineData(0)]
      [InlineData(1)]
      [InlineData(42)]
      public void Shoud_return_collection_returning_provided_count(int count)
      {
         var collection = Enumerable.Empty<int>().ToReadOnlyCollection(count);

         collection.Count.Should().Be(count);
      }

      [Theory]
      [InlineData(new int[0])]
      [InlineData(0)]
      [InlineData(1, 2)]
      [InlineData(40, 41, 42)]
      public void Shoud_return_collection_returning_provided_items(params int[] items)
      {
         var collection = items.ToReadOnlyCollection(0);

         collection.Should().BeEquivalentTo(items);
      }
   }
}
