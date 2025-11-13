using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class RenderArguments
{
   [Fact]
   public void Should_render_comma_separated_list_with_optional_leading_comma_and_prefix()
   {
      var members = StringBuilderTestHelpers.CreateInstanceMembers(("int", "A"), ("string", "B"));
      var sb = new StringBuilder();
      sb.RenderArguments(members, prefix: null, comma: ", ", leadingComma: true);
      sb.ToString().Should().Be(", @a, @b");
   }
}
