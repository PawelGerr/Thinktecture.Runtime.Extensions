using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendGenericTypeParameters
{
   [Fact]
   public void Should_render_nothing_if_no_generics()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeHasGenerics();
      sb.AppendGenericTypeParameters(t, constructOpenGeneric: false);
      sb.ToString().Should().BeEmpty();
   }

   [Fact]
   public void Should_render_type_parameter_names_when_constructOpenGeneric_false()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeHasGenerics(
         new GenericTypeParameterState("T", []),
         new GenericTypeParameterState("U", []));

      sb.AppendGenericTypeParameters(t, constructOpenGeneric: false);
      sb.ToString().Should().Be("<T, U>");
   }

   [Fact]
   public void Should_render_commas_only_when_constructOpenGeneric_true()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeHasGenerics(
         new GenericTypeParameterState("T", []),
         new GenericTypeParameterState("U", []));

      sb.AppendGenericTypeParameters(t, constructOpenGeneric: true);
      sb.ToString().Should().Be("<,>");
   }

   [Fact]
   public void Should_render_empty_angles_when_single_param_and_open_generic()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeHasGenerics(
         new GenericTypeParameterState("T", []));

      sb.AppendGenericTypeParameters(t, constructOpenGeneric: true);
      sb.ToString().Should().Be("<>");
   }
}
