using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTests
{
	public class Equals
	{
		[Fact]
		public void Should_return_false_if_item_is_null()
		{
			TestEnum.Item1.Equals(null).Should().BeFalse();
		}

		[Fact]
		public void Should_return_false_if_item_is_of_different_type()
		{
			// ReSharper disable once SuspiciousTypeConversion.Global
			TestEnum.Item1.Equals(TestEnumWithNonDefaultComparer.Item1).Should().BeFalse();
		}

		[Fact]
		public void Should_return_true_on_reference_equality()
		{
			TestEnum.Item1.Equals(TestEnum.Item1).Should().BeTrue();
		}

		[Fact]
		public void Should_return_true_if_both_items_are_invalid_and_have_same_key()
		{
			TestEnum.Get("unknown").Equals(TestEnum.Get("Unknown")).Should().BeTrue();
		}

		[Fact]
		public void Should_return_false_if_both_items_are_invalid_and_have_different_keys()
		{
			TestEnum.Get("unknown").Equals(TestEnum.Get("other")).Should().BeFalse();
		}
	}
}
