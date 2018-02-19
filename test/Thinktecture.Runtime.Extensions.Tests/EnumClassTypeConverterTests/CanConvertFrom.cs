using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTypeConverterTests
{
	public class CanConvertFrom
	{
		private readonly EnumClassTypeConverter<TestEnum, string> _converter;
		private readonly EnumClassTypeConverter<IntegerEnum, int> _intEnumConverter;

		public CanConvertFrom()
		{
			_converter = new EnumClassTypeConverter<TestEnum, string>();
			_intEnumConverter = new EnumClassTypeConverter<IntegerEnum, int>();
		}

		[Fact]
		public void Should_return_true_if_type_matches_the_key()
		{
			_converter.CanConvertFrom(typeof(string)).Should().BeTrue();
		}

		[Fact]
		public void Should_return_true_if_type_matches_the_enum()
		{
			_converter.CanConvertFrom(typeof(TestEnum)).Should().BeTrue();
		}

		[Fact]
		public void Should_return_false_if_type_doesnt_match_the_enum_and_key()
		{
			_converter.CanConvertFrom(typeof(Guid)).Should().BeFalse();
		}

		[Fact]
		public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
		{
			_intEnumConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
		}
	}
}
