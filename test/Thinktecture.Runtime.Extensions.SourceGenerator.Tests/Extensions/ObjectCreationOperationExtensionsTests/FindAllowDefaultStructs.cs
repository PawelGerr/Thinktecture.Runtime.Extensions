using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="ObjectCreationOperationExtensions.FindAllowDefaultStructs"/>.
/// </summary>
public class FindAllowDefaultStructs : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_initializer_is_null()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>]
         public partial struct TestStruct
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
      var result = operation!.FindAllowDefaultStructs();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_true_when_AllowDefaultStructs_is_set_to_true()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(AllowDefaultStructs = true)]
         public partial struct TestStruct
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
      var result = operation!.FindAllowDefaultStructs();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_AllowDefaultStructs_is_set_to_false()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(AllowDefaultStructs = false)]
         public partial struct TestStruct
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
      var result = operation!.FindAllowDefaultStructs();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_null_when_AllowDefaultStructs_is_not_set()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(KeyMemberName = "Value")]
         public partial struct TestStruct
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
      var result = operation!.FindAllowDefaultStructs();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_initializer_is_empty()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>()]
         public partial struct TestStruct
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
      var result = operation!.FindAllowDefaultStructs();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_multiple_properties_in_initializer()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(KeyMemberName = "Value", AllowDefaultStructs = true, SkipToString = false)]
         public partial struct TestStruct
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
      var result = operation!.FindAllowDefaultStructs();

      result.Should().BeTrue();
   }
}
