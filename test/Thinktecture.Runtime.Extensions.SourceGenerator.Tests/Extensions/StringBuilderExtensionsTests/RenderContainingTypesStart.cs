using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class RenderContainingTypesStart
{
   [Fact]
   public void Should_render_nothing_for_empty_list()
   {
      var sb = new StringBuilder();
      sb.RenderContainingTypesStart([]);
      sb.ToString().Should().BeEmpty();
   }

   [Theory]
   [InlineData(true, true, "record")]
   [InlineData(true, false, "class")]
   [InlineData(false, true, "record struct")]
   [InlineData(false, false, "struct")]
   public void Should_render_type_kinds_and_names(bool isReference, bool isRecord, string kind)
   {
      var sb = new StringBuilder();
      var ct = new ContainingTypeState("Outer", isReference, isRecord, []);
      sb.RenderContainingTypesStart([ct]);
      sb.ToString().Should().Be($@"
partial {kind} Outer
{{");
   }

   [Fact]
   public void Should_render_generics_of_containing_types()
   {
      var sb = new StringBuilder();
      var gen = new GenericTypeParameterState("T", []);
      var ct = new ContainingTypeState("Outer", isReferenceType: true, isRecord: false, genericParameters: [gen]);
      sb.RenderContainingTypesStart([ct]);
      sb.ToString().Should().Be(@"
partial class Outer<T>
{");
   }
}
