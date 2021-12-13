using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Runtime.Tests.Extensions.CollectionExtensionsTest
{
   public class AddReturn
   {
      [Fact]
      public void Should_add_and_return_provided_item()
      {
         var collection = new List<int>();

         var result = collection.AddReturn(1);

         result.Should().Be(1);
         collection.Should().BeEquivalentTo(new[] { 1 });
      }
   }
}
