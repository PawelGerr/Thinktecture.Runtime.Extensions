using System;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTypeConverterTests
{
	// ReSharper disable AssignNullToNotNullAttribute
	// ReSharper disable PossibleNullReferenceException
	public class ConvertFrom
	{
		private readonly EnumTypeConverter<TestEnum, string> _converter;
		private readonly EnumTypeConverter<IntegerEnum, int> _intEnumConverter;

		public ConvertFrom()
		{
			_converter = new TestEnum_EnumTypeConverter();
			_intEnumConverter = new IntegerEnum_EnumTypeConverter();
		}

		[Fact]
		public void Should_return_null_if_key_is_null()
		{
			_converter.ConvertFrom(null, null, null).Should().BeNull();
		}

		[Fact]
		public void Should_return_item_if_parameter_matches_the_key_type_and_item_exists()
		{
			_converter.ConvertFrom(null, null, "item1").Should().Be(TestEnum.Item1);
		}

		[Fact]
		public void Should_return_invalid_item_if_parameter_matches_the_key_type_but_item_dont_exist()
		{
			var item = (TestEnum)_converter.ConvertFrom(null, null, "item 1");
			item.Key.Should().Be("item 1");
			item.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_item_if_parameter_is_enum_already()
		{
			_converter.ConvertFrom(null, null, TestEnum.Item1).Should().Be(TestEnum.Item1);
		}

		[Fact]
		public void Should_throw_if_paramtere_type_doesnt_match_the_enum_and_key()
		{
			Action action = () => _converter.ConvertFrom(Guid.Empty);
			action.Should().Throw<NotSupportedException>();
		}

		[Fact]
		public void Should_return_true_if_type_doesnt_matches_the_key_but_there_is_default_conversion_of_key_from_string()
		{
			_intEnumConverter.ConvertFrom(null, null, "1").Should().Be(IntegerEnum.Item1);
		}
	}
}
