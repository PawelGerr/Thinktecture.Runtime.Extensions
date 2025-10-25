using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class RenderArgument
{
   [Fact]
   public void Should_render_prefix_and_escaped_argument_name()
   {
      var sb = new StringBuilder();
      var member = new StringBuilderTestHelpers.FakeMemberState("URL", "global::System.String");
      sb.RenderArgument(member, "ref ");
      sb.ToString().Should().Be("ref @url");
   }
}
