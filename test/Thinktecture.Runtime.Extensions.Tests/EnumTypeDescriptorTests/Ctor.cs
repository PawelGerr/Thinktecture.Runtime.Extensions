using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTypeDescriptorTests
{
	// ReSharper disable ObjectCreationAsStatement
	// ReSharper disable AssignNullToNotNullAttribute
	public class Ctor
	{
		[Fact]
		public void Should_not_throw_if_parent_is_null()
		{
			Action action = () => new EnumTypeDescriptor(null, typeof(TestEnum));
			action.Should().NotThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_object_type_is_null()
		{
			Action action = () => new EnumTypeDescriptor(null, null);
			action.Should().Throw<ArgumentNullException>();
		}
	}
}
