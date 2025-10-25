using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendTypeMinimallyQualified
{
   [Fact]
   public void Should_append_minimally_qualified_name()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::System.String", "string");
      sb.AppendTypeMinimallyQualified(t);
      sb.ToString().Should().Be("string");
   }
}
