using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="ObjectCreationOperationExtensions.FindKeyMemberName"/>.
/// </summary>
public class FindKeyMemberName : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_initializer_is_null()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>]
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
      var result = operation!.FindKeyMemberName();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_value_when_KeyMemberName_is_set_to_valid_string()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "MyKey")]
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
      var result = operation!.FindKeyMemberName();

      result.Should().Be("MyKey");
   }

   [Fact]
   public void Should_return_null_when_KeyMemberName_is_set_to_empty_string()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "")]
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
      var result = operation!.FindKeyMemberName();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_KeyMemberName_is_set_to_whitespace_only()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "   ")]
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
      var result = operation!.FindKeyMemberName();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_KeyMemberName_is_not_set()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIComparable = true)]
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
      var result = operation!.FindKeyMemberName();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_trim_leading_and_trailing_whitespace()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "  MyKey  ")]
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
      var result = operation!.FindKeyMemberName();

      result.Should().Be("MyKey");
   }

   [Fact]
   public void Should_return_null_when_initializer_is_empty()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>()]
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
      var result = operation!.FindKeyMemberName();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_multiple_properties_in_initializer()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIComparable = true, KeyMemberName = "CustomKey", SkipToString = false)]
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
      var result = operation!.FindKeyMemberName();

      result.Should().Be("CustomKey");
   }

   [Theory]
   [InlineData("\t")]
   public void Should_return_null_for_various_whitespace_strings(string whitespace)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "{{whitespace}}")]
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
      var result = operation!.FindKeyMemberName();

      result.Should().BeNull();
   }
}
