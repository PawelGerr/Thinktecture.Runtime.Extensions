using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindUseWithEntityFramework : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_set_to_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<int>(UseWithEntityFramework = true)]
         public partial class EFValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.EFValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseWithEntityFramework();

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_set_to_false()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<int>(UseWithEntityFramework = false)]
         public partial class EFValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.EFValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseWithEntityFramework();

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_parameter_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<int>]
         public partial class EFValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.EFValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseWithEntityFramework();

      // Assert
      result.Should().BeFalse();
   }
}
