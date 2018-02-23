using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTypeConverterTests
{
	public class ConvertTo
	{
		private readonly EnumTypeConverter<TestEnum, string> _converter;
		private readonly EnumTypeConverter<IntegerEnum, int> _intEnumConverter;

		public ConvertTo()
		{
			_converter = new EnumTypeConverter<TestEnum, string>();
			_intEnumConverter = new EnumTypeConverter<IntegerEnum, int>();
		}

		[Fact]
		public void Should_return_default_of_provided_destinationtype_if_null_is_provided_and_type_is_valuetype()
		{
			_intEnumConverter.ConvertTo(null, null, null, typeof(int)).Should().Be(0);
		}

		[Fact]
		public void Should_return_default_of_provided_destinationtype_if_null_is_provided_and_type_is_referencetype()
		{
			_intEnumConverter.ConvertTo(null, null, null, typeof(string)).Should().BeNull();
		}

		[Fact]
		public void Should_return_default_of_the_key_if_null_is_provided()
		{
			_intEnumConverter.ConvertTo(null, null, null, typeof(TestEnum)).Should().BeNull();
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
