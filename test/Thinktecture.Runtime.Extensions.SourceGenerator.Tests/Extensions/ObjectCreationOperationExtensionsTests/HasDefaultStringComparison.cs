using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="ObjectCreationOperationExtensions.HasDefaultStringComparison"/>.
/// </summary>
public class HasDefaultStringComparison : CompilationTestBase
{
   [Fact]
   public void Should_return_false_when_initializer_is_null()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ComplexValueObject]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.HasDefaultStringComparison();

      result.Should().BeFalse();
   }

   [Theory]
   [InlineData("System.StringComparison.CurrentCulture")]
   [InlineData("System.StringComparison.CurrentCultureIgnoreCase")]
   [InlineData("System.StringComparison.InvariantCulture")]
   [InlineData("System.StringComparison.InvariantCultureIgnoreCase")]
   [InlineData("System.StringComparison.Ordinal")]
   [InlineData("System.StringComparison.OrdinalIgnoreCase")]
   public void Should_return_true_when_DefaultStringComparison_is_set_to_valid_enum_value(string enumValue)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [ComplexValueObject(DefaultStringComparison = {{enumValue}})]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.HasDefaultStringComparison();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_DefaultStringComparison_is_not_set()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ComplexValueObject(SkipToString = true)]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.HasDefaultStringComparison();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_initializer_is_empty()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ComplexValueObject()]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.HasDefaultStringComparison();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_multiple_properties_in_initializer()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ComplexValueObject(SkipToString = true, DefaultStringComparison = System.StringComparison.CurrentCultureIgnoreCase, SkipFactoryMethods = false)]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.HasDefaultStringComparison();

      result.Should().BeTrue();
   }

   [Theory]
   [InlineData(-1)]
   [InlineData(100)]
   [InlineData(999)]
   public void Should_return_false_when_DefaultStringComparison_is_set_to_invalid_enum_value(int value)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [ComplexValueObject(DefaultStringComparison = (System.StringComparison)({{value}}))]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.HasDefaultStringComparison();

      result.Should().BeFalse();
   }
}
