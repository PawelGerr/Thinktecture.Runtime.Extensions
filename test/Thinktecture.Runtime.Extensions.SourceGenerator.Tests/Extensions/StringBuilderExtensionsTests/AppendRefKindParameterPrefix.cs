using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendRefKindParameterPrefix
{
   [Theory]
   [InlineData(RefKind.Out, "out ")]
   [InlineData(RefKind.Ref, "ref ")]
   [InlineData(RefKind.In, "in ")]
   [InlineData(RefKind.RefReadOnlyParameter, "ref readonly ")]
   [InlineData(RefKind.None, "")]
   public void Should_render_prefix_for_parameter(RefKind kind, string expected)
   {
      var sb = new StringBuilder();
      sb.AppendRefKindParameterPrefix(kind);
      sb.ToString().Should().Be(expected);
   }
}
