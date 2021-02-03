using System.ComponentModel;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests
{
	public class GetConverter
	{
		[Fact]
		public void Should_return_enum_typeconverter_via_typedescriptor()
		{
			var type = typeof(TestEnum);
			var converter = TypeDescriptor.GetConverter(type);

			converter.Should().BeOfType(typeof(TestEnum_EnumTypeConverter));
		}
	}
}
