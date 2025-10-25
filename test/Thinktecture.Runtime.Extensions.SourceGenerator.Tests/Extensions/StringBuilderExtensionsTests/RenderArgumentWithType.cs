using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class RenderArgumentWithType
{
   [Fact]
   public void Should_render_type_and_name_with_nullable_question_mark_based_on_flag()
   {
      var sb = new StringBuilder();
      var member = new StringBuilderTestHelpers.FakeMemberState(
         name: "Value",
         typeFullyQualified: "global::System.Int32",
         isReferenceType: false,
         isValueType: true,
         isRecord: false,
         isTypeParameter: false,
         nullableAnnotation: NullableAnnotation.NotAnnotated,
         isNullableStruct: false);

      sb.RenderArgumentWithType(member, prefix: null, useNullableTypes: true);
      sb.ToString().Should().Be("global::System.Int32? @value");
   }
}
