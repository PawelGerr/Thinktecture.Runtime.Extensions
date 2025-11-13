using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class RenderArgumentsWithType
{
   [Fact]
   public void Should_render_types_and_names_with_commas_and_trailing_comma_and_nullable_hint()
   {
      var members = StringBuilderTestHelpers.CreateInstanceMembers(("int", "A"), ("string", "B"));
      var sb = new StringBuilder();
      sb.RenderArgumentsWithType(members, prefix: null, comma: ", ", leadingComma: false, trailingComma: true, useNullableTypes: true);

      sb.ToString().Should().Be("int? @a, string? @b, ");
   }
}
