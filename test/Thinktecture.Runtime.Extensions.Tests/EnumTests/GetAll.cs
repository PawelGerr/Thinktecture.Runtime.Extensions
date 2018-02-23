using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTests
{
	public class GetAll
	{
		[Fact]
		public void Should_return_empty_collection_if_enum_has_no_items()
		{
			EmptyEnum.GetAll().Should().BeEmpty();
		}

		[Fact]
		public void Should_throw_if_public_static_field_is_not_readonly()
		{
			Action action = () => EnumWithNonReadOnlyField.GetAll();
			action.Should().Throw<Exception>()
			      .WithMessage($"The field \"{nameof(EnumWithNonReadOnlyField.Item)}\" of enumeration type \"{typeof(EnumWithNonReadOnlyField).FullName}\" must be read-only.");
		}

		[Fact]
		public void Should_behave_like_the_enum_given_to_baseclass()
		{
			InvalidImplementationEnum.GetAll().Should().HaveCount(2);
		}

		[Fact]
		public void Should_return_public_fields_only()
		{
			var enums = TestEnum.GetAll();
			enums.Should().HaveCount(2);
			enums.Should().Contain(TestEnum.Item1);
			enums.Should().Contain(TestEnum.Item2);
		}
	}
}
