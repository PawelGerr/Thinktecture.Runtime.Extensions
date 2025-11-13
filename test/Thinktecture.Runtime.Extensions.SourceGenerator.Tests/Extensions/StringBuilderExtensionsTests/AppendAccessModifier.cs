using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendAccessModifier
{
   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.UnionConstructorAccessModifier.Private, "private")]
   [InlineData(Thinktecture.CodeAnalysis.UnionConstructorAccessModifier.Internal, "internal")]
   [InlineData(Thinktecture.CodeAnalysis.UnionConstructorAccessModifier.Public, "public")]
   public void Should_append_access_modifier(Thinktecture.CodeAnalysis.UnionConstructorAccessModifier m, string expected)
   {
      var sb = new StringBuilder();
      sb.AppendAccessModifier(m);
      sb.ToString().Should().Be(expected);
   }
}
