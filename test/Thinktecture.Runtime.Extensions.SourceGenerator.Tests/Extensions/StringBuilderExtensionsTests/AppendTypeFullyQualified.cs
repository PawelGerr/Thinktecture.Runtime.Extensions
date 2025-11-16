using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.StringBuilderExtensionsTests;

public class AppendTypeFullyQualified
{
   [Fact]
   public void Should_append_with_global_namespace_and_containing_types()
   {
      var sb = new StringBuilder();
      var type = new StringBuilderTestHelpers.FakeNamespaceAndName("MyNs", "MyType");
      var containing = ImmutableArray.CreateRange([
         new ContainingTypeState("Outer", true, false, []),
         new ContainingTypeState("Inner", true, false, [])
      ]);

      sb.AppendTypeFullyQualifiedWithoutGenerics(type, containing);
      sb.ToString().Should().Be("global::MyNs.Outer.Inner.MyType");
   }

   [Fact]
   public void Should_handle_null_namespace()
   {
      var sb = new StringBuilder();
      var type = new StringBuilderTestHelpers.FakeNamespaceAndName(null, "MyType");
      sb.AppendTypeFullyQualifiedWithoutGenerics(type, []);
      sb.ToString().Should().Be("global::MyType");
   }

   [Fact]
   public void Should_append_fully_qualified_name()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("global::System.String", "string");
      sb.AppendTypeFullyQualified(t);
      sb.ToString().Should().Be("global::System.String");
   }

   [Fact]
   public void Should_delegate_to_nullable_or_nonnullable()
   {
      var sb = new StringBuilder();
      var t = new StringBuilderTestHelpers.FakeTypeInfo("T", "T", isReferenceType: true, nullableAnnotation: NullableAnnotation.NotAnnotated);
      sb.AppendTypeFullyQualified(t, nullable: true);
      sb.ToString().Should().Be("T?");
   }
}
