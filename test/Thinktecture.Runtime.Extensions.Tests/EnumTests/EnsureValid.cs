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
		public void Should_not_throw_for_derived_types()
		{
			EnumWithDerivedType.Item1.EnsureValid();
			EnumWithDerivedType.ItemOfDerivedType.EnsureValid();
		}

      [Fact]
		public void Should_not_throw_for_valid_struct()
		{
         StructIntegerEnum.Item1.EnsureValid();
		}

		[Fact]
		public void Should_throw_if_default_struct_is_invalid()
		{
			Action action = () => new StructIntegerEnum().EnsureValid();

			action.Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(StructIntegerEnum)}' with key '0' is not valid.");
		}

      [Fact]
		public void Should_throw_if_item_is_invalid()
		{
			Action action = () => TestEnum.Get("invalid").EnsureValid();

			action.Should().Throw<InvalidOperationException>().WithMessage($"The current enumeration item of type '{nameof(TestEnum)}' with key 'invalid' is not valid.");
		}
	}
}
