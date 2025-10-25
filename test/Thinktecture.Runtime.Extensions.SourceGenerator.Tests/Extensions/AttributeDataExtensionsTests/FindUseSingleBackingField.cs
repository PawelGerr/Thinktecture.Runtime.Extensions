using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindUseSingleBackingField : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_set_to_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(UseSingleBackingField = true)]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseSingleBackingField();

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

         [Union<string, int>(UseSingleBackingField = false)]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseSingleBackingField();

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_null_when_parameter_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseSingleBackingField();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_work_with_struct_union()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(UseSingleBackingField = true)]
         public partial struct TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseSingleBackingField();

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_work_with_three_type_parameters()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int, double>(UseSingleBackingField = true)]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseSingleBackingField();

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_work_with_five_type_parameters()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int, double, bool, decimal>(UseSingleBackingField = false)]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUseSingleBackingField();

      // Assert
      result.Should().BeFalse();
   }
}
