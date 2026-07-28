using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class RenderContainingTypesEnd
{
   [Fact]
   public void Should_render_matching_number_of_closing_braces()
   {
      var sb = new StringBuilder();
      sb.RenderContainingTypesEnd([
         new ContainingTypeState("A", true, false, false, []),
         new ContainingTypeState("B", true, false, false, []),
      ]);

      var s = sb.ToString();
      s.Should().Be("\n}\n}");
   }
}
