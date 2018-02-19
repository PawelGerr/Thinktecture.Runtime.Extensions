using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTypeDescriptorTests
{
	public class GetConverter
	{
		[Fact]
		public void Should_return_converter_of_valid_enumclass()
		{
			var converter = new EnumClassTypeDescriptor(null, typeof(TestEnum)).GetConverter();

			converter.Should().BeOfType<EnumClassTypeConverter<TestEnum, string>>();
		}

		[Fact]
		public void Should_return_converter_of_invalid_enumclass()
		{
			var converter = new EnumClassTypeDescriptor(null, typeof(InvalidImplementationEnum)).GetConverter();

			converter.Should().BeOfType<EnumClassTypeConverter<TestEnum, string>>();
		}

		[Fact]
		public void Should_throw_if_type_is_not_enumclass()
		{
			Action action = () => new EnumClassTypeDescriptor(null, typeof(string)).GetConverter();

			action.Should().Throw<ArgumentException>();
		}
	}
}
