using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendTypeKind
{
   [Theory]
   [InlineData(true, false, "class")]
   [InlineData(true, true, "record")]
   [InlineData(false, false, "struct")]
   [InlineData(false, true, "record struct")]
   public void Should_render_type_kind(bool isRef, bool isRecord, string expected)
   {
      var sb = new StringBuilder();
      var type = new StringBuilderTestHelpers.FakeTypeInfo("T", "T", isReferenceType: isRef, isValueType: !isRef, isRecord: isRecord);
      sb.AppendTypeKind(type);
      sb.ToString().Should().Be(expected);
   }
}
