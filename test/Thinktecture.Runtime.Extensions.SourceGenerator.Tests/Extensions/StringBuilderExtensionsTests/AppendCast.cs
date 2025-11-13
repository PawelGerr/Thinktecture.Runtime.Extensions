using System.Text;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendCast
{
   [Fact]
   public void Should_append_cast_when_condition_true()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::System.String", "string");
      sb.AppendCast(t, condition: true);
      sb.ToString().Should().Be("(global::System.String)");
   }

   [Fact]
   public void Should_not_append_cast_when_condition_false()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::System.String", "string");
      sb.AppendCast(t, condition: false);
      sb.ToString().Should().BeEmpty();
   }
}
