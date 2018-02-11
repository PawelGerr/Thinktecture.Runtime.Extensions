using FluentAssertions;
using Thinktecture.EnumClassTests.Enums;
using Xunit;

namespace Thinktecture.EnumClassTests
{
	public class ToString
	{
		[Fact]
		public void Should_return_string_representation_of_string()
		{
			TestEnum.Item1.ToString().Should().Be("item1");
		}
	}
}
