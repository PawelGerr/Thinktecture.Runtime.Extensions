using System.Collections;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Runtime.Tests.EmptyCollectionTests
{
   // ReSharper disable once InconsistentNaming
   public class Empty_Enumerable
   {
      private IEnumerable SUT => Empty.Collection();

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
   }
}
