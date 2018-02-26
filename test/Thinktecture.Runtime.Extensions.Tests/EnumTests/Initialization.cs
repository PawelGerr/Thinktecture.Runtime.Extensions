using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTests
{
	public class Initialization
	{
		[Fact]
		public void Should_throw_if_enum_has_duplicate_key()
		{
			Action action = () => EnumWithDuplicateKey.GetAll();
			action.Should().Throw<ArgumentException>()
			      .WithMessage($"The enumeration of type \"{typeof(EnumWithDuplicateKey).FullName}\" has multiple items with the key \"item\".");
		}

		[Fact]
		public void Should_not_throw_if_enum_has_2_keys_that_differs_in_casing_only_if_comparer_honors_casing()
		{
			Action action = () => TestEnumWithNonDefaultComparer.GetAll();
			action.Should().NotThrow<Exception>();
		}
	}
}
