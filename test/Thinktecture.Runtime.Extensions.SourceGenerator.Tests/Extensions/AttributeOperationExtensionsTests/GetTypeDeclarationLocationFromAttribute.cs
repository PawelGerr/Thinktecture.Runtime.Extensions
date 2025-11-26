using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Thinktecture.Runtime.Tests.AttributeOperationExtensionsTests;

/// <summary>
/// Tests for <see cref="AttributeOperationExtensions.GetTypeDeclarationLocationFromAttribute"/>.
/// </summary>
public class GetTypeDeclarationLocationFromAttribute : CompilationTestBase
{
   [Fact]
   public void Should_return_class_identifier_location_for_attribute_on_class()
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("TestClass");
   }

   [Fact]
   public void Should_return_struct_identifier_location_for_attribute_on_struct()
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

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("TestStruct");
   }

   [Fact]
   public void Should_return_record_identifier_location_for_attribute_on_record()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [Union]
         public abstract partial record TestRecord
         {
            public sealed partial record Success : TestRecord;
            public sealed partial record Failure : TestRecord;
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("TestRecord");
   }

   [Fact]
   public void Should_return_record_struct_identifier_location_for_attribute_on_record_struct()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>]
         public partial record struct TestRecordStruct
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("TestRecordStruct");
   }

   [Fact]
   public void Should_return_interface_identifier_location_for_attribute_on_interface()
   {
      var source = """
         using System;

         namespace TestNamespace;

         [Obsolete]
         public partial interface ITestInterface
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("ITestInterface");
   }

   [Fact]
   public void Should_return_inner_type_identifier_location_for_nested_type()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         public partial class OuterClass
         {
            [SmartEnum<int>]
            public partial class InnerClass
            {
            }
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("InnerClass");
   }

   [Fact]
   public void Should_return_correct_location_for_deeply_nested_type()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         public partial class Level1
         {
            public partial class Level2
            {
               [ValueObject<string>]
               public partial class Level3
               {
               }
            }
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("Level3");
   }

   [Fact]
   public void Should_return_correct_location_for_generic_type()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ComplexValueObject]
         public partial class GenericType<T>
         {
            public T Value { get; }
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("GenericType");
   }

   [Fact]
   public void Should_return_attribute_location_when_no_type_declaration_found()
   {
      var source = """
         using System.Reflection;

         [assembly: AssemblyVersion("1.0.0.0")]

         namespace TestNamespace;

         public class TestClass
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      // Should return the entire attribute location since there's no type declaration
      locationText.Should().Be("AssemblyVersion(\"1.0.0.0\")");
   }

   [Fact]
   public void Should_handle_partial_class_correctly()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>]
         public partial class PartialTestClass
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("PartialTestClass");
   }

   [Fact]
   public void Should_return_type_identifier_when_multiple_attributes_on_same_type()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>]
         [Obsolete]
         public partial class MultiAttributeClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      // Get the first attribute (SmartEnum)
      var firstAttributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                           .DescendantNodes()
                                           .OfType<AttributeSyntax>()
                                           .First();

      var firstAttributeOperation = semanticModel.GetOperation(firstAttributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      firstAttributeOperation.Should().NotBeNull();

      var firstResult = firstAttributeOperation!.GetTypeDeclarationLocationFromAttribute();

      // Get the second attribute (Obsolete)
      var secondAttributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                            .DescendantNodes()
                                            .OfType<AttributeSyntax>()
                                            .Skip(1)
                                            .First();

      var secondAttributeOperation = semanticModel.GetOperation(secondAttributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      secondAttributeOperation.Should().NotBeNull();

      var secondResult = secondAttributeOperation!.GetTypeDeclarationLocationFromAttribute();

      // Both should return the same type identifier location
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var firstLocationText = sourceText.Substring(firstResult.SourceSpan.Start, firstResult.SourceSpan.Length);
      var secondLocationText = sourceText.Substring(secondResult.SourceSpan.Start, secondResult.SourceSpan.Length);

      firstLocationText.Should().Be("MultiAttributeClass");
      secondLocationText.Should().Be("MultiAttributeClass");
      firstResult.SourceSpan.Should().Be(secondResult.SourceSpan);
   }

   [Fact]
   public void Should_return_correct_location_for_attribute_with_multiple_arguments()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(ComparisonOperators = OperatorsGeneration.Default)]
         public partial class ConfiguredEnum
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
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("ConfiguredEnum");
   }

   [Theory]
   [InlineData("class")]
   [InlineData("struct")]
   [InlineData("record")]
   [InlineData("record class")]
   [InlineData("record struct")]
   public void Should_return_type_identifier_for_various_type_kinds(string typeKeyword)
   {
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>]
         public partial {{typeKeyword}} VariousTypeTest
         {
         }
         """;

      var compilation = CreateCompilation(source, expectedCompilerErrors: ["Attribute 'SmartEnum<>' is not valid on this declaration type. It is only valid on 'class' declarations."]);
      var syntaxTree = compilation.SyntaxTrees.First();
      var semanticModel = compilation.GetSemanticModel(syntaxTree);

      var attributeSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                                      .DescendantNodes()
                                      .OfType<AttributeSyntax>()
                                      .First();

      var attributeOperation = semanticModel.GetOperation(attributeSyntax, TestContext.Current.CancellationToken) as IAttributeOperation;
      attributeOperation.Should().NotBeNull();

      var result = attributeOperation!.GetTypeDeclarationLocationFromAttribute();

      result.Should().NotBeNull();
      var sourceText = syntaxTree.GetText(TestContext.Current.CancellationToken).ToString();
      var locationText = sourceText.Substring(result.SourceSpan.Start, result.SourceSpan.Length);

      locationText.Should().Be("VariousTypeTest");
   }
}
