using System.ComponentModel;
using FluentAssertions;
using Thinktecture.TestEnums;
using Xunit;

namespace Thinktecture.EnumClassTypeDescriptionProviderTests
{
	public class GetConverter
	{
		[Fact]
		public void Should_return_enumclass_typeconverter_via_typedescriptor()
		{
			var type = typeof(TestEnum);
			var converter = TypeDescriptor.GetConverter(type);

			converter.Should().BeOfType(typeof(EnumClassTypeConverter<TestEnum, string>));
		}
	}
}
