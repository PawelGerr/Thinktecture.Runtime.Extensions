using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendRefKindArgumentPrefix
{
   [Theory]
   [InlineData(RefKind.Out, "out ")]
   [InlineData(RefKind.Ref, "ref ")]
   [InlineData(RefKind.In, "in ")]
   [InlineData(RefKind.RefReadOnlyParameter, "in ")]
   [InlineData(RefKind.None, "")]
   public void Should_render_prefix_for_argument(RefKind kind, string expected)
   {
      var sb = new StringBuilder();
      sb.AppendRefKindArgumentPrefix(kind);
      sb.ToString().Should().Be(expected);
   }
}
