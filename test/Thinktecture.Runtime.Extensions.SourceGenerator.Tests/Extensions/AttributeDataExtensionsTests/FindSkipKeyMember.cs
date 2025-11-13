using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindSkipKeyMember : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_parameter_is_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipKeyMember = true)]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindSkipKeyMember();

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_parameter_is_false()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipKeyMember = false)]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindSkipKeyMember();

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_null_when_parameter_is_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindSkipKeyMember();

      // Assert
      result.Should().BeNull();
   }
}
