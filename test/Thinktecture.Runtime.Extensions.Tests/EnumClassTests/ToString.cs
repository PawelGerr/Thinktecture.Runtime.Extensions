using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTests
{
	public class ToString
	{
		[Fact]
		public void Should_return_string_representation_of_the_key()
		{
			TestEnum.Item1.ToString().Should().Be("item1");
		}
	}
}
