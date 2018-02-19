using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTests
{
	public class ImplicitConversionToKey
	{
		[Fact]
		public void Should_return_null_if_item_is_null()
		{
			string key = (TestEnum)null;

			key.Should().BeNull();
		}

		[Fact]
		public void Should_return_key()
		{
			string key = TestEnum.Item1;

			key.Should().Be(TestEnum.Item1.Key);
		}
	}
}
