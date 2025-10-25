using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendGenericConstraints
{
   [Fact]
   public void Should_join_constraints_with_comma()
   {
      var sb = new StringBuilder();
      sb.AppendGenericConstraints(["class", "new()"]);
      sb.ToString().Should().Be("class, new()");
   }

   [Fact]
   public void Should_render_nothing_for_empty_list()
   {
      var sb = new StringBuilder();
      sb.AppendGenericConstraints([]);
      sb.ToString().Should().BeEmpty();
   }

   [Fact]
   public void Should_render_where_clauses_with_prefix_and_multiple_constraints()
   {
      var sb = new StringBuilder();
      var gp1 = new GenericTypeParameterState("T", ["class", "new()"]);
      var gp2 = new GenericTypeParameterState("U", ["global::System.IDisposable"]);

      var t = new StringBuilderTestHelpers.FakeHasGenerics(gp1, gp2);
      sb.AppendGenericConstraints(t, prefix: "   ");
      sb.ToString().Should().Be("""

                                   where T : class, new()
                                   where U : global::System.IDisposable
                                """);
   }

   [Fact]
   public void Should_render_nothing_if_no_constraints()
   {
      var sb = new StringBuilder();
      var genericTypeParam = new GenericTypeParameterState("T", []);
      var t = new StringBuilderTestHelpers.FakeHasGenerics(genericTypeParam);
      sb.AppendGenericConstraints(t, prefix: "");
      sb.ToString().Should().BeEmpty();
   }
}
