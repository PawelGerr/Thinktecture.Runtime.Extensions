using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendEscaped
{
   [Fact]
   public void Should_prefix_with_at_for_argumentname()
   {
      var sb = new StringBuilder();
      sb.AppendEscaped(ArgumentName.Create("Value"));
      sb.ToString().Should().Be("@value");
   }

   [Fact]
   public void Should_prefix_with_at_for_string()
   {
      var sb = new StringBuilder();
      sb.AppendEscaped("value");
      sb.ToString().Should().Be("@value");
   }
}
