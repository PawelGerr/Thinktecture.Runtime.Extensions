using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendTypeFullyQualifiedNullAnnotated
{
   [Fact]
   public void Should_append_question_mark_for_reference_type_not_annotated()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("T", "T", isReferenceType: true, nullableAnnotation: NullableAnnotation.NotAnnotated);
      sb.AppendTypeFullyQualifiedNullAnnotated(t);
      sb.ToString().Should().Be("T?");
   }

   [Fact]
   public void Should_append_question_mark_for_type_parameter_non_struct()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("T", "T", isReferenceType: false, isValueType: false, isTypeParameter: true, nullableAnnotation: NullableAnnotation.NotAnnotated);
      sb.AppendTypeFullyQualifiedNullAnnotated(t);
      sb.ToString().Should().Be("T?");
   }

   [Fact]
   public void Should_not_append_for_reference_type_already_annotated()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("T?", "T?", isReferenceType: true, nullableAnnotation: NullableAnnotation.Annotated);
      sb.AppendTypeFullyQualifiedNullAnnotated(t);
      sb.ToString().Should().Be("T?");
   }
}
