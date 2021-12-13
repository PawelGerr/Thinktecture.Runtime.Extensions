using System.ComponentModel;
using FluentAssertions;
using Thinktecture.Runtime.Tests.TestEnums;
using Xunit;

namespace Thinktecture.Runtime.Tests.EnumTypeConverterTests;

public class GetConverter
{
   [Fact]
   public void Should_return_enum_typeconverter_via_typedescriptor()
   {
      TypeDescriptor.GetConverter(typeof(TestEnum)).Should().BeOfType(typeof(TestEnum_EnumTypeConverter));

      TypeDescriptor.GetConverter(typeof(ExtensibleTestEnum)).Should().BeOfType(typeof(ExtensibleTestEnum_EnumTypeConverter));
      TypeDescriptor.GetConverter(typeof(ExtendedTestEnum)).Should().BeOfType(typeof(ExtendedTestEnum_EnumTypeConverter));
      TypeDescriptor.GetConverter(typeof(DifferentAssemblyExtendedTestEnum)).Should().BeOfType(typeof(DifferentAssemblyExtendedTestEnum_EnumTypeConverter));
   }
}