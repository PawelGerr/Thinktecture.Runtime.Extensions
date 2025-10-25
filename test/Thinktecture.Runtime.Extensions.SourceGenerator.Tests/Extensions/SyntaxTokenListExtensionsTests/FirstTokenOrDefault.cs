using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.SyntaxTokenListExtensionsTests;

public class FirstTokenOrDefault
{
   [Fact]
   public void Should_return_default_when_list_is_empty_without_predicate()
   {
      // Arrange
      var emptyList = SyntaxFactory.TokenList();

      // Act
      var result = emptyList.FirstTokenOrDefault();

      // Assert
      result.Kind().Should().Be(SyntaxKind.None);
      result.IsKind(SyntaxKind.None).Should().BeTrue();
   }

   [Fact]
   public void Should_return_first_token_when_list_has_single_token()
   {
      // Arrange
      var token = SyntaxFactory.Identifier("myVariable");
      var list = SyntaxFactory.TokenList(token);

      // Act
      var result = list.FirstTokenOrDefault();

      // Assert
      result.Should().Be(token);
      result.Text.Should().Be("myVariable");
   }

   [Fact]
   public void Should_return_first_token_when_list_has_multiple_tokens()
   {
      // Arrange
      var token1 = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
      var token2 = SyntaxFactory.Token(SyntaxKind.StaticKeyword);
      var token3 = SyntaxFactory.Token(SyntaxKind.VoidKeyword);
      var list = SyntaxFactory.TokenList(token1, token2, token3);

      // Act
      var result = list.FirstTokenOrDefault();

      // Assert
      result.Should().Be(token1);
      result.Kind().Should().Be(SyntaxKind.PublicKeyword);
   }

   [Fact]
   public void Should_work_with_various_token_types_without_predicate()
   {
      // Arrange
      var tokens = new[]
                   {
                      SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                      SyntaxFactory.Identifier("MyClass"),
                      SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                      SyntaxFactory.Token(SyntaxKind.CloseBraceToken)
                   };
      var list = SyntaxFactory.TokenList(tokens);

      // Act
      var result = list.FirstTokenOrDefault();

      // Assert
      result.Should().Be(tokens[0]);
      result.Kind().Should().Be(SyntaxKind.PublicKeyword);
   }

   [Theory]
   [InlineData(1)]
   [InlineData(5)]
   [InlineData(10)]
   [InlineData(100)]
   public void Should_always_return_first_token_regardless_of_list_size(int size)
   {
      // Arrange
      var firstToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
      var tokens = Enumerable.Range(0, size - 1)
                             .Select(_ => SyntaxFactory.Token(SyntaxKind.PrivateKeyword))
                             .Prepend(firstToken)
                             .ToArray();
      var list = SyntaxFactory.TokenList(tokens);

      // Act
      var result = list.FirstTokenOrDefault();

      // Assert
      result.Should().Be(firstToken);
      result.Kind().Should().Be(SyntaxKind.PublicKeyword);
   }
}
