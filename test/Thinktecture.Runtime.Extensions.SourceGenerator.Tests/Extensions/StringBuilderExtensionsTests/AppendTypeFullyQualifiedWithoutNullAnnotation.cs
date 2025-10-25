using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendTypeFullyQualifiedWithoutNullAnnotation
{
   [Fact]
   public void Should_remove_trailing_question_mark_for_reference_types_with_annotated_nullability()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::System.String?", "string?", isReferenceType: true, nullableAnnotation: NullableAnnotation.Annotated);
      sb.AppendTypeFullyQualifiedWithoutNullAnnotation(t);
      sb.ToString().Should().Be("global::System.String");
   }

   [Fact]
   public void Should_not_modify_when_not_annotated()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::System.String", "string", isReferenceType: true, nullableAnnotation: NullableAnnotation.NotAnnotated);
      sb.AppendTypeFullyQualifiedWithoutNullAnnotation(t);
      sb.ToString().Should().Be("global::System.String");
   }
}
