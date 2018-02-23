using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTypeDescriptorTests
{
	public class GetConverter
	{
		[Fact]
		public void Should_return_converter_of_valid_enumc()
		{
			var converter = new EnumTypeDescriptor(null, typeof(TestEnum)).GetConverter();

			converter.Should().BeOfType<EnumTypeConverter<TestEnum, string>>();
		}

		[Fact]
		public void Should_return_converter_of_invalid_enum()
		{
			var converter = new EnumTypeDescriptor(null, typeof(InvalidImplementationEnum)).GetConverter();

			converter.Should().BeOfType<EnumTypeConverter<TestEnum, string>>();
		}

		[Fact]
		public void Should_throw_if_type_is_not_enum()
		{
			Action action = () => new EnumTypeDescriptor(null, typeof(string)).GetConverter();

			action.Should().Throw<ArgumentException>();
		}
	}
}
