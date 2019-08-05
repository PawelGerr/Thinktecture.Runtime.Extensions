using System.Collections;
using FluentAssertions;
using Xunit;

namespace Thinktecture.EmptyCollectionTests
{
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
