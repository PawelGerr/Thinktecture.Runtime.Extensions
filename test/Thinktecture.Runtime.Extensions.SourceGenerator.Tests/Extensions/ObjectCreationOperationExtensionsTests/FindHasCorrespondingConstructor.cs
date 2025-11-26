using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="ObjectCreationOperationExtensions.FindHasCorrespondingConstructor"/>.
/// </summary>
public class FindHasCorrespondingConstructor : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_initializer_is_null()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>]
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
      var result = operation!.FindHasCorrespondingConstructor();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_true_when_HasCorrespondingConstructor_is_set_to_true()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(HasCorrespondingConstructor = true)]
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
      var result = operation!.FindHasCorrespondingConstructor();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_HasCorrespondingConstructor_is_set_to_false()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(HasCorrespondingConstructor = false)]
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
      var result = operation!.FindHasCorrespondingConstructor();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_null_when_HasCorrespondingConstructor_is_not_set()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(UseForModelBinding = true)]
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
      var result = operation!.FindHasCorrespondingConstructor();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_initializer_is_empty()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>()]
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
      var result = operation!.FindHasCorrespondingConstructor();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_multiple_properties_in_initializer()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(UseForModelBinding = true, HasCorrespondingConstructor = true, UseWithEntityFramework = false)]
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
      var result = operation!.FindHasCorrespondingConstructor();

      result.Should().BeTrue();
   }
}
