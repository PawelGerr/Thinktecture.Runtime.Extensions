using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTypeConverterTests
{
	public class ConvertTo
	{
		private readonly EnumClassTypeConverter<TestEnum, string> _converter;
		private readonly EnumClassTypeConverter<IntegerEnum, int> _intEnumConverter;

		public ConvertTo()
		{
			_converter = new EnumClassTypeConverter<TestEnum, string>();
			_intEnumConverter = new EnumClassTypeConverter<IntegerEnum, int>();
		}

		[Fact]
		public void Should_return_key_if_type_matches_the_key()
		{
			_converter.ConvertTo(null, null, TestEnum.Item1, typeof(string)).Should().Be("item1");
		}

		[Fact]
		public void Should_return_item_if_type_matches_the_enum()
		{
			_converter.ConvertTo(null, null, TestEnum.Item1, typeof(TestEnum)).Should().Be(TestEnum.Item1);
		}

		[Fact]
		public void Should_throw_if_paramtere_type_doesnt_match_the_enum_and_key()
		{
			Action action = () => _converter.ConvertTo(null, null, TestEnum.Item1, typeof(Guid));
			action.Should().Throw<NotSupportedException>();
		}

		[Fact]
		public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_to_string()
		{
			_intEnumConverter.ConvertTo(null, null, IntegerEnum.Item1, typeof(string)).Should().Be("1");
		}
	}
}
