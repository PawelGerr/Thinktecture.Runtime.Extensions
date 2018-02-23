using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTests
{
	public class EnsureValid
	{
		[Fact]
		public void Should_not_throw_if_item_is_valid()
		{
			TestEnum.Item1.EnsureValid();
		}

		[Fact]
		public void Should_throw_if_item_is_invalid()
		{
			Action action = () => TestEnum.Get("invalid").EnsureValid();

			action.Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type {typeof(TestEnum).FullName} with key invalid is not valid.");
		}
	}
}
