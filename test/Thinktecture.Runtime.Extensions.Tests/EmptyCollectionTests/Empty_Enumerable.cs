using System.Collections;
using System.Linq;

namespace Thinktecture.Runtime.Tests.EmptyCollectionTests;

// ReSharper disable once InconsistentNaming
public class Empty_Enumerable
{
   // ReSharper disable once MemberCanBeMadeStatic.Local
   // ReSharper disable once InconsistentNaming
   private IEnumerable SUT => Empty.Collection();

   [Fact]
   public void Should_not_be_null()
   {
      SUT.Should().NotBeNull();
   }

   [Fact]
   public void Should_be_empty()
   {
      SUT.Cast<object>().Should().BeEmpty();
   }
}
