using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindTxIsNullableReferenceType : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_T1IsNullableReferenceType_is_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T1IsNullableReferenceType = true)]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxIsNullableReferenceType(1);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_parameter_not_set()
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
      var result = attributeData.FindTxIsNullableReferenceType(1);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_different_type_parameters()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int, double>(T1IsNullableReferenceType = true, T2IsNullableReferenceType = false)]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var t1Result = attributeData.FindTxIsNullableReferenceType(1);
      var t2Result = attributeData.FindTxIsNullableReferenceType(2);
      var t3Result = attributeData.FindTxIsNullableReferenceType(3);

      // Assert
      t1Result.Should().BeTrue();
      t2Result.Should().BeFalse();
      t3Result.Should().BeFalse(); // Default when not set
   }
}
