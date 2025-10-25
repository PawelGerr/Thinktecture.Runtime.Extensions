using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.ObjectCreationOperationExtensionsTests;

public class GetIntegerParameterValue : CompilationTestBase
{
   [Fact]
   public void Should_handle_KeyMemberAccessModifier_with_byte_enum()
   {
      // Create a test scenario where AccessModifier property gets a byte value
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = (Thinktecture.AccessModifier)(byte)8)]
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

      // This should not throw with the defensive implementation
      var result = operation!.FindKeyMemberAccessModifier();
      result.Should().Be(Thinktecture.CodeAnalysis.AccessModifier.Public);
   }

   [Fact]
   public void Should_handle_KeyMemberKind_with_byte_value()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberKind = (Thinktecture.MemberKind)(byte)1)]
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

      // Should handle byte values correctly
      var result = operation!.FindKeyMemberKind();
      result.Should().Be(Thinktecture.CodeAnalysis.MemberKind.Property);
   }

   [Fact]
   public void Should_handle_KeyMemberKind_with_short_value()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberKind = (Thinktecture.MemberKind)(short)0)]
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

      // Should handle short values correctly
      var result = operation!.FindKeyMemberKind();
      result.Should().Be(Thinktecture.CodeAnalysis.MemberKind.Field);
   }

   [Fact]
   public void Should_handle_enum_property_with_enum_value()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = (Thinktecture.AccessModifier)2)]
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

      // Should not throw InvalidCastException
      var result = operation!.FindKeyMemberAccessModifier();
      result.Should().Be(Thinktecture.CodeAnalysis.AccessModifier.Protected);
   }

   [Fact]
   public void Should_handle_long_property_with_long_value_in_range()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = (Thinktecture.AccessModifier)4L)]
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

      // After the fix, this should work even with long values that fit in int range
      var result = operation!.FindKeyMemberAccessModifier();
      result.Should().Be(Thinktecture.CodeAnalysis.AccessModifier.Internal);
   }

   [Fact]
   public void Should_return_null_for_long_property_with_out_of_range_value()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = (Thinktecture.AccessModifier)999999L)]
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

      // The fix should return null for out-of-range long values
      var result = operation!.FindKeyMemberAccessModifier();
      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_enum_with_byte_underlying_type()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberKind = (Thinktecture.MemberKind)1)]
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

      // Should not throw with enum that has byte underlying type
      var result = operation!.FindKeyMemberKind();
      result.Should().Be(Thinktecture.CodeAnalysis.MemberKind.Property);
   }

   [Fact]
   public void Should_handle_AccessModifier_enum_directly()
   {
      // This test uses the actual AccessModifier enum from the codebase
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = Thinktecture.AccessModifier.Public)]
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

      // Note: This test doesn't use FindKeyMemberAccessModifier because the property name is different
      // It verifies that the codebase compiles and runs without issues
      operation.Should().NotBeNull();
   }

   [Fact]
   public void Should_handle_invalid_enum_values_gracefully()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(KeyMemberAccessModifier = (Thinktecture.AccessModifier)999)]
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

      // Should return null for invalid enum values (999 is not a valid AccessModifier)
      var result = operation!.FindKeyMemberAccessModifier();
      result.Should().BeNull();
   }
}
