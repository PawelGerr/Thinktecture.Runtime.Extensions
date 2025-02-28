using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.Runtime.Tests.EnumerableExtensionsTests;

// ReSharper disable once CheckNamespace
public class ToReadOnlyCollection
{
   [Fact]
   public void Should_throw_if_items_is_null()
   {
      0.Invoking(_ => ((IEnumerable<int>)null)!.ToReadOnlyCollection(42))
       .Should().Throw<ArgumentNullException>();
   }

   [Fact]
   public void Should_throw_if_count_is_negative()
   {
      Enumerable.Empty<int>().Invoking(e => e.ToReadOnlyCollection(-1))
                .Should().Throw<ArgumentException>();
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(42)]
   public void Should_return_collection_returning_provided_count(int count)
   {
      var collection = Enumerable.Empty<int>().ToReadOnlyCollection(count);

      collection.Count.Should().Be(count);
   }

   [Theory]
   [InlineData(new int[0])]
   [InlineData(new[] { 0 })]
   [InlineData(new[] { 1, 2 })]
   [InlineData(new[] { 40, 41, 42 })]
   public void Should_return_collection_returning_provided_items(int[] items)
   {
      var collection = items.ToReadOnlyCollection(0);

      collection.Should().BeEquivalentTo(items);
   }
}
