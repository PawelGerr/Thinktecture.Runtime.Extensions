using System.Collections.Immutable;
using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendTypeForXmlComment
{
   [Fact]
   public void Should_render_generic_type_as_c_when_has_angle_brackets()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("System.Collections.Generic.List<global::System.Int32>", "List<int>");
      sb.AppendTypeFullyQualifiedForXmlComment((ITypeFullyQualified)t);
      sb.ToString().Should().Be("<c>System.Collections.Generic.List&lt;global::System.Int32&gt;</c>");
   }

   [Fact]
   public void Should_render_as_c_tag_for_problematic_chars_and_escape_angle_brackets()
   {
      var sb = new StringBuilder();
      var member = new StringBuilderTestHelpers.FakeMemberState("Value", "System.Collections.Generic.List<int>");
      // member.IsTypeParameter == false by default in FakeMemberState, call from member overload uses member.TypeFullyQualified and isGenericTypeParameter=false
      sb.AppendMemberTypeForXmlComment(member);
      sb.ToString().Should().Be("<c>System.Collections.Generic.List&lt;int&gt;</c>");
   }

   [Fact]
   public void Should_render_see_cref_for_simple_types_and_append_suffix_if_provided()
   {
      var sb = new StringBuilder();
      var member = new StringBuilderTestHelpers.FakeMemberState("Value", "global::System.String");
      sb.AppendMemberTypeForXmlComment(member, ("as text", " "));
      sb.ToString().Should().Be("<see cref=\"global::System.String as text\"/>");
   }

   [Fact]
   public void Should_use_minimally_qualified_if_no_containing_types()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::System.String", "string");
      sb.AppendTypeForXmlComment(t);
      sb.ToString().Should().Be("<see cref=\"string\"/>");
   }

   [Fact]
   public void Should_use_fully_qualified_if_has_containing_types()
   {
      var sb = new StringBuilder();
      var containing = ImmutableArray.Create(new ContainingTypeState("Outer", isReferenceType: true, isRecord: false, genericParameters: []));
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::MyNs.Outer.Inner", "Inner", "MyNs", "Inner", containingTypes: containing);
      sb.AppendTypeForXmlComment(t);
      sb.ToString().Should().Be("<see cref=\"global::MyNs.Outer.Inner\"/>");
   }
}
