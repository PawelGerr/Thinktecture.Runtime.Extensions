using FluentAssertions;
using Thinktecture.TestEnums;
using Thinktecture.TestEnums.Isolated;
using Xunit;

namespace Thinktecture.EnumTests
{
	public class TryGet
	{
		[Fact]
		public void Should_return_false_if_null_is_provided()
		{
			EmptyEnum.TryGet(null, out var item)
			         .Should().BeFalse();

			item.Should().BeNull();
		}

		[Fact]
		public void Should_return_false_if_enum_dont_have_any_items()
		{
			EmptyEnum.TryGet("unknown", out var item)
				.Should().BeFalse();

			item.Should().BeNull();
		}

		[Fact]
		public void Should_return_false_if_struct_dont_have_any_items()
		{
         StructIntegerEnum.TryGet(42, out var item)
                          .Should().BeFalse();

			item.Should().Be(new StructIntegerEnum());
		}

		[Fact]
		public void Should_return_false_if_enum_dont_have_item_with_provided_key()
		{
			TestEnum.TryGet("unknown", out var item)
			        .Should().BeFalse();

			item.Should().BeNull();
		}

		[Fact]
		public void Should_return_true_if_item_with_provided_key_exists()
		{
			TestEnum.TryGet("item2", out var item)
				.Should().BeTrue();

			item.Should().Be(TestEnum.Item2);
		}

		[Fact]
		public void Should_return_true_if_item_with_provided_key_exists_ignoring_casing()
		{
			StaticCtorTestEnum_TryGet.TryGet("Item", out var item)
				.Should().BeTrue();

			item.Should().Be(StaticCtorTestEnum_TryGet.Item);
		}

		[Fact]
		public void Should_return_false_if_the_casing_does_not_match_accoring_to_comparer()
		{
			TestEnumWithNonDefaultComparer.TryGet("Item2", out var item)
				.Should().BeFalse();

			item.Should().BeNull();
		}
	}
}
