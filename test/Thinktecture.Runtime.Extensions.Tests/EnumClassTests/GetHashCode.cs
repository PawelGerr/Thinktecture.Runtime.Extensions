using System;
using FluentAssertions;
using Thinktecture.EnumClassTests.Enums;
using Xunit;

namespace Thinktecture.EnumClassTests
{
	public class GetHashCode
	{
		[Fact]
		public void Should_return_hashcode_of_the_type_plus_key()
		{
			var hashCode = TestEnum.Item1.GetHashCode();

			var typeHashCode = typeof(TestEnum).GetHashCode();
			var keyHashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(TestEnum.Item1.Key);
			hashCode.Should().Be((typeHashCode * 397) ^ keyHashCode);
		}
	}
}
