using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="ObjectCreationOperationExtensions.FindSkipKeyMember"/>.
/// </summary>
public class FindSkipKeyMember : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_initializer_is_null()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot()
         .DescendantNodes()
         .OfType<AttributeSyntax>()
         .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindSkipKeyMember();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_true_when_SkipKeyMember_is_set_to_true()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipKeyMember = true)]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot()
         .DescendantNodes()
         .OfType<AttributeSyntax>()
         .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindSkipKeyMember();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_SkipKeyMember_is_set_to_false()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipKeyMember = false)]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot()
         .DescendantNodes()
         .OfType<AttributeSyntax>()
         .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindSkipKeyMember();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_null_when_SkipKeyMember_is_not_set()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(KeyMemberName = "CustomValue")]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot()
         .DescendantNodes()
         .OfType<AttributeSyntax>()
         .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindSkipKeyMember();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_initializer_is_empty()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>()]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot()
         .DescendantNodes()
         .OfType<AttributeSyntax>()
         .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindSkipKeyMember();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_multiple_properties_in_initializer()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(KeyMemberName = "Value", SkipKeyMember = true, SkipToString = false)]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot()
         .DescendantNodes()
         .OfType<AttributeSyntax>()
         .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindSkipKeyMember();

      result.Should().BeTrue();
   }
}
