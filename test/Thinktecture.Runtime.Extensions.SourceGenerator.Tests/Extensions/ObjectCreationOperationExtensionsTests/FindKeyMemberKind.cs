using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="ObjectCreationOperationExtensions.FindKeyMemberKind"/>.
/// </summary>
public class FindKeyMemberKind : CompilationTestBase
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
      var result = operation!.FindKeyMemberKind();

      result.Should().BeNull();
   }

   [Theory]
   [InlineData(0, Thinktecture.CodeAnalysis.MemberKind.Field)]
   [InlineData(1, Thinktecture.CodeAnalysis.MemberKind.Property)]
   public void Should_return_valid_MemberKind_when_set_to_valid_value(int value, Thinktecture.CodeAnalysis.MemberKind expectedKind)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberKind = (Thinktecture.MemberKind){{value}})]
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
      var result = operation!.FindKeyMemberKind();

      result.Should().Be(expectedKind);
   }

   [Theory]
   [InlineData(2)]
   [InlineData(3)]
   [InlineData(10)]
   [InlineData(100)]
   [InlineData(-1)]
   [InlineData(-10)]
   public void Should_return_null_when_set_to_invalid_value(int invalidValue)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberKind = (MemberKind)({{invalidValue}}))]
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
      var result = operation!.FindKeyMemberKind();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_KeyMemberKind_is_not_set()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "Key")]
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
      var result = operation!.FindKeyMemberKind();

      result.Should().BeNull();
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
      var result = operation!.FindKeyMemberKind();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_multiple_properties_in_initializer()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "Key", KeyMemberKind = Thinktecture.MemberKind.Property, SkipToString = false)]
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
      var result = operation!.FindKeyMemberKind();

      result.Should().Be(Thinktecture.CodeAnalysis.MemberKind.Property);
   }
}
