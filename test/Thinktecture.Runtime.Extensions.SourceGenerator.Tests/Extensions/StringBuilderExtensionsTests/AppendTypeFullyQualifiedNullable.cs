using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendTypeFullyQualifiedNullable
{
   [Theory]
   [InlineData(true, NullableAnnotation.NotAnnotated, false)]
   [InlineData(true, NullableAnnotation.Annotated, false)]
   [InlineData(false, NullableAnnotation.NotAnnotated, false)]
   [InlineData(false, NullableAnnotation.NotAnnotated, true)]
   public void Should_apply_nullable_question_mark_rules(bool isRef, NullableAnnotation ann, bool isNullableStruct)
   {
      var sb = new StringBuilder();
      var typeName = isNullableStruct || ann == NullableAnnotation.Annotated ? "T?" : "T";
      var t = new StringBuilderTestHelpers.FakeTypeInfo(typeName, typeName, isReferenceType: isRef, isValueType: !isRef, nullableAnnotation: ann, isNullableStruct: isNullableStruct);
      sb.AppendTypeFullyQualifiedNullable(t);

      sb.ToString().Should().Be("T?");
   }
}
