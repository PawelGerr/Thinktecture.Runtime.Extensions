using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.Runtime.Tests.SyntaxListExtensionsTests;

public class FirstNodeOrDefault
{
   [Fact]
   public void Should_return_default_when_list_is_empty()
   {
      // Arrange
      var emptyList = SyntaxFactory.List<StatementSyntax>();

      // Act
      var result = emptyList.FirstNodeOrDefault(_ => true);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_no_element_matches_predicate()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "string y = \"test\";", "var z = 3.14;");

      // Act
      var result = list.FirstNodeOrDefault(s => s is ReturnStatementSyntax);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_first_element_when_single_element_matches()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;");

      // Act
      var result = list.FirstNodeOrDefault(s => s is LocalDeclarationStatementSyntax);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<LocalDeclarationStatementSyntax>();
   }

   [Fact]
   public void Should_return_null_when_single_element_does_not_match()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;");

      // Act
      var result = list.FirstNodeOrDefault(s => s is ReturnStatementSyntax);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_first_matching_element_when_first_element_matches()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "return x;", "string y = \"test\";");

      // Act
      var result = list.FirstNodeOrDefault(s => s is LocalDeclarationStatementSyntax);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<LocalDeclarationStatementSyntax>();
      var declaration = (LocalDeclarationStatementSyntax)result!;
      declaration.Declaration.Variables[0].Identifier.Text.Should().Be("x");
   }

   [Fact]
   public void Should_return_matching_element_when_middle_element_matches()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "return x;", "string y = \"test\";");

      // Act
      var result = list.FirstNodeOrDefault(s => s is ReturnStatementSyntax);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<ReturnStatementSyntax>();
   }

   [Fact]
   public void Should_return_matching_element_when_last_element_matches()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "string y = \"test\";", "return x;");

      // Act
      var result = list.FirstNodeOrDefault(s => s is ReturnStatementSyntax);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<ReturnStatementSyntax>();
   }

   [Fact]
   public void Should_return_first_match_when_multiple_elements_match()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "string y = \"test\";", "var z = 3.14;");

      // Act
      var result = list.FirstNodeOrDefault(s => s is LocalDeclarationStatementSyntax);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<LocalDeclarationStatementSyntax>();
      var declaration = (LocalDeclarationStatementSyntax)result!;
      declaration.Declaration.Variables[0].Identifier.Text.Should().Be("x");
   }

   [Fact]
   public void Should_return_first_element_when_predicate_always_returns_true()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "string y = \"test\";", "var z = 3.14;");

      // Act
      var result = list.FirstNodeOrDefault(_ => true);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<LocalDeclarationStatementSyntax>();
      var declaration = (LocalDeclarationStatementSyntax)result!;
      declaration.Declaration.Variables[0].Identifier.Text.Should().Be("x");
   }

   [Fact]
   public void Should_return_null_when_predicate_always_returns_false()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "string y = \"test\";", "var z = 3.14;");

      // Act
      var result = list.FirstNodeOrDefault(_ => false);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_large_list_efficiently()
   {
      // Arrange
      var statements = Enumerable.Range(0, 1000)
                                 .Select(i => $"int x{i} = {i};")
                                 .Append("return 42;")
                                 .ToArray();
      var list = CreateStatementList(statements);

      // Act
      var result = list.FirstNodeOrDefault(s => s is ReturnStatementSyntax);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<ReturnStatementSyntax>();
   }

   [Fact]
   public void Should_stop_iteration_after_first_match()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "return x;", "string y = \"test\";");
      var callCount = 0;

      // Act
      var result = list.FirstNodeOrDefault(s =>
      {
         callCount++;
         return s is ReturnStatementSyntax;
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
      var statements = Enumerable.Range(0, size)
                                 .Select(i => $"int x{i} = {i};")
                                 .ToArray();
      var list = CreateStatementList(statements);

      // Act
      var result = list.FirstNodeOrDefault(s => s is LocalDeclarationStatementSyntax);

      // Assert
      if (size > 0)
      {
         result.Should().NotBeNull();
         result.Should().BeOfType<LocalDeclarationStatementSyntax>();
      }
      else
      {
         result.Should().BeNull();
      }
   }

   [Fact]
   public void Should_work_with_different_syntax_node_types()
   {
      // Arrange
      var code = @"
class TestClass
{
   void Method1() { }
   int Method2() => 42;
   string Property { get; set; }
}";
      var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
      var root = tree.GetRoot(TestContext.Current.CancellationToken);
      var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
      var members = classDeclaration.Members;

      // Act
      var result = members.FirstNodeOrDefault(m => m is PropertyDeclarationSyntax);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<PropertyDeclarationSyntax>();
      var property = (PropertyDeclarationSyntax)result!;
      property.Identifier.Text.Should().Be("Property");
   }

   [Fact]
   public void Should_handle_complex_predicate_logic()
   {
      // Arrange
      var list = CreateStatementList("int x = 1;", "string y = \"test\";", "var z = 3.14;", "return x;");

      // Act
      var result = list.FirstNodeOrDefault(s =>
                                              s is LocalDeclarationStatementSyntax local &&
                                              local.Declaration.Type.ToString().Contains("string"));

      // Assert
      result.Should().NotBeNull();
      result.Should().BeOfType<LocalDeclarationStatementSyntax>();
      var declaration = (LocalDeclarationStatementSyntax)result!;
      declaration.Declaration.Variables[0].Identifier.Text.Should().Be("y");
   }

   private static SyntaxList<StatementSyntax> CreateStatementList(params string[] statements)
   {
      if (statements.Length == 0)
         return SyntaxFactory.List<StatementSyntax>();

      var code = $@"
class TestClass
{{
   void TestMethod()
   {{
      {string.Join("\n      ", statements)}
   }}
}}";

      var tree = CSharpSyntaxTree.ParseText(code, cancellationToken: TestContext.Current.CancellationToken);
      var root = tree.GetRoot(TestContext.Current.CancellationToken);
      var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
      var block = method.Body!;

      return block.Statements;
   }
}
