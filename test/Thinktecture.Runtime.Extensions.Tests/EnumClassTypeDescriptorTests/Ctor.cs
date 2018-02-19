using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTypeDescriptorTests
{
	public class Ctor
	{
		[Fact]
		public void Should_not_throw_if_parent_is_null()
		{
			Action action = () => new EnumClassTypeDescriptor(null, typeof(TestEnum));
			action.Should().NotThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_object_type_is_null()
		{
			Action action = () => new EnumClassTypeDescriptor(null, null);
			action.Should().Throw<ArgumentNullException>();
		}
	}
}
