using System.Text;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendDelegateType
{
   [Fact]
   public void Should_use_custom_delegate_name_when_provided()
   {
      var sb = new StringBuilder();
      var m = new DelegateMethodState(Accessibility.Public, "Do", null, [], "MyDelegate");
      sb.AppendDelegateType(m);
      sb.ToString().Should().Be("MyDelegate");
   }

   [Fact]
   public void Should_generate_custom_delegate_name_when_ref_parameter_present()
   {
      var sb = new StringBuilder();
      var m = new DelegateMethodState(Accessibility.Public, "Compute", null, [new ParameterState("x", "int", RefKind.Ref)], null);
      sb.AppendDelegateType(m);
      sb.ToString().Should().Be("ComputeDelegate");
   }

   [Fact]
   public void Should_use_action_without_type_arguments_for_void_no_params()
   {
      var sb = new StringBuilder();
      var m = new DelegateMethodState(Accessibility.Public, "Run", null, [], null);
      sb.AppendDelegateType(m);
      sb.ToString().Should().Be("global::System.Action");
   }

   [Fact]
   public void Should_use_action_with_parameters_for_void_with_params()
   {
      var sb = new StringBuilder();
      var m = new DelegateMethodState(Accessibility.Public, "Run", null, [new ParameterState("a", "int", RefKind.None), new ParameterState("b", "string", RefKind.None)], null);
      sb.AppendDelegateType(m);
      sb.ToString().Should().Be("global::System.Action<int, string>");
   }

   [Fact]
   public void Should_use_func_with_return_type_and_params()
   {
      var sb = new StringBuilder();
      var m = new DelegateMethodState(Accessibility.Public, "Compute", "global::System.String", [new ParameterState("x", "int", RefKind.None)], null);
      sb.AppendDelegateType(m);
      sb.ToString().Should().Be("global::System.Func<int, global::System.String>");
   }

   [Fact]
   public void Should_use_func_with_only_return_type_when_no_params()
   {
      var sb = new StringBuilder();
      var m = new DelegateMethodState(Accessibility.Public, "Get", "int", [], null);
      sb.AppendDelegateType(m);
      sb.ToString().Should().Be("global::System.Func<int>");
   }
}
