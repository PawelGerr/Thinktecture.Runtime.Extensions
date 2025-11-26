using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="ObjectCreationOperationExtensions.FindKeyMemberAccessModifier"/>.
/// </summary>
public class FindKeyMemberAccessModifier : CompilationTestBase
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

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindKeyMemberAccessModifier();

      result.Should().BeNull();
   }

   [Theory]
   [InlineData(1, Thinktecture.CodeAnalysis.AccessModifier.Private)]
   [InlineData(2, Thinktecture.CodeAnalysis.AccessModifier.Protected)]
   [InlineData(4, Thinktecture.CodeAnalysis.AccessModifier.Internal)]
   [InlineData(8, Thinktecture.CodeAnalysis.AccessModifier.Public)]
   [InlineData(3, Thinktecture.CodeAnalysis.AccessModifier.PrivateProtected)]
   [InlineData(6, Thinktecture.CodeAnalysis.AccessModifier.ProtectedInternal)]
   public void Should_return_valid_AccessModifier_when_set_to_valid_value(int value, Thinktecture.CodeAnalysis.AccessModifier expectedModifier)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = (Thinktecture.AccessModifier){{value}})]
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
      var result = operation!.FindKeyMemberAccessModifier();

      result.Should().Be(expectedModifier);
   }

   [Theory]
   [InlineData(0)]
   [InlineData(5)]
   [InlineData(7)]
   [InlineData(9)]
   [InlineData(15)]
   [InlineData(16)]
   [InlineData(100)]
   [InlineData(-1)]
   public void Should_return_null_when_set_to_invalid_value(int invalidValue)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = ((AccessModifier)({{invalidValue}})))]
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
      var result = operation!.FindKeyMemberAccessModifier();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_KeyMemberAccessModifier_is_not_set()
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

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindKeyMemberAccessModifier();

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

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      var operation = attributeOperation?.Operation as IObjectCreationOperation;

      operation.Should().NotBeNull();
      var result = operation!.FindKeyMemberAccessModifier();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_multiple_properties_in_initializer()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberName = "Key", KeyMemberAccessModifier = Thinktecture.AccessModifier.Public, SkipToString = false)]
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
      var result = operation!.FindKeyMemberAccessModifier();

      result.Should().Be(Thinktecture.CodeAnalysis.AccessModifier.Public);
   }
}
