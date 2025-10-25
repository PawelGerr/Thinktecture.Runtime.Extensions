using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendAccessibility
{
   [Theory]
   [InlineData(Accessibility.NotApplicable, "")]
   [InlineData(Accessibility.Private, "private")]
   [InlineData(Accessibility.ProtectedAndInternal, "private protected")]
   [InlineData(Accessibility.Protected, "protected")]
   [InlineData(Accessibility.Internal, "internal")]
   [InlineData(Accessibility.ProtectedOrInternal, "protected internal")]
   [InlineData(Accessibility.Public, "public")]
   public void Should_append_accessibility(Accessibility a, string expected)
   {
      var sb = new StringBuilder();
      sb.AppendAccessibility(a);
      sb.ToString().Should().Be(expected);
   }
}
