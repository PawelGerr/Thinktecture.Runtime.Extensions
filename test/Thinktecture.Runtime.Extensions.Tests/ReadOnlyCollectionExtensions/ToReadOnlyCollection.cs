using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

// ReSharper disable once CheckNamespace
namespace Thinktecture.Runtime.Tests.ReadOnlyCollectionExtensions;

public class ToReadOnlyCollection
{
   [Fact]
   public void Should_throw_if_items_is_null()
   {
      FluentActions.Invoking(() => ((IReadOnlyCollection<int>)null!).ToReadOnlyCollection(i => i))
                   .Should().Throw<ArgumentNullException>();
   }

   [Fact]
   public void Should_throw_if_count_is_negative()
   {
      Array.Empty<int>().Invoking(e => e.ToReadOnlyCollection((Func<int, int>?)null!))
           .Should().Throw<ArgumentException>();
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(2)]
   public void Should_return_collection_returning_provided_count(int count)
   {
      var input = Enumerable.Range(0, count).Select(i => new { Value = i }).ToArray();

      var collection = input.ToReadOnlyCollection(i => i.Value);

      collection.Count.Should().Be(count);
      collection.Should().BeEquivalentTo(input.Select(i => i.Value).ToArray());
   }
}
