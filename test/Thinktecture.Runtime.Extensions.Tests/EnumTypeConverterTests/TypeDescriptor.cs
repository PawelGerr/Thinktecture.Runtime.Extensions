using System.ComponentModel;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumTypeConverterTests
{
	public class GetConverter
	{
		[Fact]
		public void Should_return_enum_typeconverter_via_typedescriptor()
		{
			var type = typeof(TestEnum);
			var converter = TypeDescriptor.GetConverter(type);

			converter.Should().BeOfType(typeof(EnumTypeConverter<TestEnum, string>));
		}
	}
}
