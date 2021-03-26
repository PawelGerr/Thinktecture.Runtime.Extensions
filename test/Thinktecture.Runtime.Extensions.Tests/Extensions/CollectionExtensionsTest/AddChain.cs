using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Runtime.Tests.Extensions.CollectionExtensionsTest
{
   public class AddChain
   {
      [Fact]
      public void Should_add_item_and_return_collection()
      {
         var collection = new List<int>();

         var result = collection.AddChain(1);

         result.Should().BeSameAs(collection);
         collection.Should().BeEquivalentTo(1);
      }
   }
}
