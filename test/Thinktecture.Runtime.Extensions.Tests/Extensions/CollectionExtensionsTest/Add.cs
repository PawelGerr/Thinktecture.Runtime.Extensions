using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Runtime.Tests.Extensions.CollectionExtensionsTest
{
   public class Add
   {
      [Fact]
      public void Should_add_from_other_collection()
      {
         var collection = new List<int>();

         var result = collection.Add(new[] { 1, 2, 3 });

         result.Should().BeSameAs(collection);
         collection.Should().BeEquivalentTo(new[] { 1, 2, 3 });
      }
   }
}
