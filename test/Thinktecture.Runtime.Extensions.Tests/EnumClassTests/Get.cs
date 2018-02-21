using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTests
{
	public class Get
	{
		[Fact]
		public void Should_return_null_if_null_is_provided()
		{
			var item = EmptyEnum.Get(null);

			item.Should().BeNull();
		}

		[Fact]
		public void Should_return_invalid_item_if_enum_dont_have_any_items()
		{
			var item = EmptyEnum.Get("unknown");

			item.IsValid.Should().BeFalse();
			item.Key.Should().Be("unknown");
		}

		[Fact]
		public void Should_return_invalid_item_if_enum_dont_have_item_with_provided_key()
		{
			var item = TestEnum.Get("unknown");

			item.IsValid.Should().BeFalse();
			item.Key.Should().Be("unknown");
		}

		[Fact]
		public void Should_return_item_with_provided_key()
		{
			var item = TestEnum.Get("Item2");
			item.Should().Be(TestEnum.Item2);
		}

		[Fact]
		public void Should_return_item_with_provided_key_ignoring_casing()
		{
			var item = TestEnum.Get("item2");
			item.Should().Be(TestEnum.Item2);
		}

		[Fact]
		public void Should_return_invalid_item_if_the_casing_does_not_match_accoring_to_comparer()
		{
			var item = TestEnumWithNonDefaultComparer.Get("Item2");
			item.Key.Should().Be("Item2");
			item.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_item_with_provided_key_of_type_provided_to_baseclass()
		{
			var item = InvalidImplementationEnum.Get(TestEnum.Item2.Key);
			item.Should().Be(TestEnum.Item2);
		}

		[Fact]
		public void Should_throw_if_createInvalid_is_returning_null()
		{
			Action action = () => InvalidCreateInvalidImplementationEnum.Get("unknown");
			action.Should().Throw<Exception>().WithMessage($"The method CreateInvalid of enumeration type {typeof(InvalidCreateInvalidImplementationEnum).FullName} returned null.");
		}
	}
}
