using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class RenderAccessModifier
{
   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Private, "private")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Protected, "protected")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Internal, "internal")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Public, "public")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.PrivateProtected, "private protected")]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.ProtectedInternal, "protected internal")]
   public void Should_render_all_access_modifiers(Thinktecture.CodeAnalysis.AccessModifier m, string expected)
   {
      var sb = new StringBuilder();
      sb.RenderAccessModifier(m);
      sb.ToString().Should().Be(expected);
   }
}
