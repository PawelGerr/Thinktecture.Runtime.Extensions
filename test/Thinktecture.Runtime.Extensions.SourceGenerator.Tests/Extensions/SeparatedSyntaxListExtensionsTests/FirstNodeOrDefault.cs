using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Runtime.Tests.SeparatedSyntaxListExtensionsTests;

public class FirstNodeOrDefault
{
   [Fact]
   public void Should_return_default_when_list_is_empty()
   {
      // Arrange
      var emptyList = SyntaxFactory.SeparatedList<ParameterSyntax>();

      // Act
      var result = emptyList.FirstNodeOrDefault(_ => true);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_no_element_matches_predicate()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "double z");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Identifier.Text == "notFound");

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_first_element_when_single_element_matches()
   {
      // Arrange
      var list = CreateParameterList("int x");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "int");

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("x");
   }

   [Fact]
   public void Should_return_null_when_single_element_does_not_match()
   {
      // Arrange
      var list = CreateParameterList("int x");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "string");

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_first_matching_element_when_first_element_matches()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "double z");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "int");

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("x");
      result.Type!.ToString().Should().Be("int");
   }

   [Fact]
   public void Should_return_matching_element_when_middle_element_matches()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "double z");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "string");

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("y");
      result.Type!.ToString().Should().Be("string");
   }

   [Fact]
   public void Should_return_matching_element_when_last_element_matches()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "double z");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "double");

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("z");
      result.Type!.ToString().Should().Be("double");
   }

   [Fact]
   public void Should_return_first_match_when_multiple_elements_match()
   {
      // Arrange
      var list = CreateParameterList("int x", "int y", "string z", "int w");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "int");

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("x", "should return the first matching element");
   }

   [Fact]
   public void Should_return_first_element_when_predicate_always_returns_true()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "double z");

      // Act
      var result = list.FirstNodeOrDefault(_ => true);

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("x");
   }

   [Fact]
   public void Should_return_null_when_predicate_always_returns_false()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "double z");

      // Act
      var result = list.FirstNodeOrDefault(_ => false);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_large_list_efficiently()
   {
      // Arrange
      var parameters = Enumerable.Range(0, 1000)
         .Select(i => $"int param{i}")
         .Append("string targetParam")
         .ToArray();
      var list = CreateParameterList(parameters);

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "string");

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("targetParam");
   }

   [Fact]
   public void Should_stop_iteration_after_first_match()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "double z", "bool w");
      var callCount = 0;

      // Act
      var result = list.FirstNodeOrDefault(p =>
      {
         callCount++;
         return p.Type!.ToString() == "string";
      });

      // Assert
      result.Should().NotBeNull();
      callCount.Should().Be(2, "predicate should only be called until first match is found");
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(5)]
   [InlineData(10)]
   public void Should_handle_various_list_sizes(int size)
   {
      // Arrange
      var parameters = Enumerable.Range(0, size)
         .Select(i => $"int param{i}")
         .ToArray();
      var list = CreateParameterList(parameters);

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString() == "int");

      // Assert
      if (size > 0)
      {
         result.Should().NotBeNull();
         result!.Type!.ToString().Should().Be("int");
      }
      else
      {
         result.Should().BeNull();
      }
   }

   [Fact]
   public void Should_work_with_argument_lists()
   {
      // Arrange
      var code = @"
class TestClass
{
   void TestMethod()
   {
      SomeMethod(1, ""test"", 3.14, true);
   }
}";
      var tree = CSharpSyntaxTree.ParseText(code);
      var root = tree.GetRoot();
      var invocation = root.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
      var arguments = invocation.ArgumentList.Arguments;

      // Act
      var result = arguments.FirstNodeOrDefault(a => a.Expression is LiteralExpressionSyntax { Token.Value: string });

      // Assert
      result.Should().NotBeNull();
      result!.Expression.Should().BeOfType<LiteralExpressionSyntax>();
      ((LiteralExpressionSyntax)result.Expression).Token.ValueText.Should().Be("test");
   }

   [Fact]
   public void Should_work_with_complex_predicate_logic()
   {
      // Arrange
      var list = CreateParameterList("int x", "string y", "ref int z", "out bool w");

      // Act
      var result = list.FirstNodeOrDefault(p =>
         p.Modifiers.Any(m => m.IsKind(SyntaxKind.RefKeyword)));

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("z");
      result.Modifiers.Should().Contain(m => m.IsKind(SyntaxKind.RefKeyword));
   }

   [Fact]
   public void Should_handle_parameters_with_default_values()
   {
      // Arrange
      var code = @"
class TestClass
{
   void TestMethod(int x, string y = ""default"", bool z = false)
   {
   }
}";
      var tree = CSharpSyntaxTree.ParseText(code);
      var root = tree.GetRoot();
      var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
      var parameters = method.ParameterList.Parameters;

      // Act
      var result = parameters.FirstNodeOrDefault(p => p.Default != null);

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("y", "should return first parameter with default value");
   }

   [Fact]
   public void Should_work_with_attributes_on_parameters()
   {
      // Arrange
      var code = @"
class TestClass
{
   void TestMethod(int x, [SomeAttribute] string y, double z)
   {
   }
}";
      var tree = CSharpSyntaxTree.ParseText(code);
      var root = tree.GetRoot();
      var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
      var parameters = method.ParameterList.Parameters;

      // Act
      var result = parameters.FirstNodeOrDefault(p => p.AttributeLists.Any());

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("y");
   }

   [Fact]
   public void Should_handle_nullable_reference_types()
   {
      // Arrange
      var list = CreateParameterList("int x", "string? y", "double z");

      // Act
      var result = list.FirstNodeOrDefault(p => p.Type!.ToString().Contains("?"));

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("y");
      result.Type!.ToString().Should().Be("string?");
   }

   [Fact]
   public void Should_work_with_type_checking_predicates()
   {
      // Arrange
      var code = @"
class TestClass
{
   void TestMethod(int x, List<string> y, Dictionary<int, string> z)
   {
   }
}";
      var tree = CSharpSyntaxTree.ParseText(code);
      var root = tree.GetRoot();
      var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
      var parameters = method.ParameterList.Parameters;

      // Act
      var result = parameters.FirstNodeOrDefault(p => p.Type is GenericNameSyntax { Identifier.Text: "List" });

      // Assert
      result.Should().NotBeNull();
      result!.Identifier.Text.Should().Be("y");
   }

   private static SeparatedSyntaxList<ParameterSyntax> CreateParameterList(params string[] parameters)
   {
      if (parameters.Length == 0)
         return SyntaxFactory.SeparatedList<ParameterSyntax>();

      var paramList = string.Join(", ", parameters);
      var code = $@"
class TestClass
{{
   void TestMethod({paramList})
   {{
   }}
}}";

      var tree = CSharpSyntaxTree.ParseText(code);
      var root = tree.GetRoot();
      var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

      return method.ParameterList.Parameters;
   }
}
