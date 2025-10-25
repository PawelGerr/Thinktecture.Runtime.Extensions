using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindTxName : CompilationTestBase
{
   [Fact]
   public void Should_return_custom_name_when_T1Name_is_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T1Name = "TextValue")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxName(1);

      // Assert
      result.Should().Be("TextValue");
   }

   [Fact]
   public void Should_return_custom_name_when_T2Name_is_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T2Name = "NumericValue")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxName(2);

      // Assert
      result.Should().Be("NumericValue");
   }

   [Fact]
   public void Should_handle_multiple_custom_names()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T1Name = "TextValue", T2Name = "NumericValue")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var t1Name = attributeData.FindTxName(1);
      var t2Name = attributeData.FindTxName(2);

      // Assert
      t1Name.Should().Be("TextValue");
      t2Name.Should().Be("NumericValue");
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
      var result = attributeData.FindTxName(1);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_T1Name_not_set_but_T2Name_is_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T2Name = "NumericValue")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxName(1);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_all_five_type_parameters()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int, double, bool, decimal>(
            T1Name = "First",
            T2Name = "Second",
            T3Name = "Third",
            T4Name = "Fourth",
            T5Name = "Fifth")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert
      attributeData.FindTxName(1).Should().Be("First");
      attributeData.FindTxName(2).Should().Be("Second");
      attributeData.FindTxName(3).Should().Be("Third");
      attributeData.FindTxName(4).Should().Be("Fourth");
      attributeData.FindTxName(5).Should().Be("Fifth");
   }

   [Fact]
   public void Should_return_null_for_invalid_index()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T1Name = "TextValue")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxName(10);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_empty_string_value()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T1Name = "")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxName(1);

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_trim_whitespace_from_value()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T1Name = "  TextValue  ")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxName(1);

      // Assert
      result.Should().Be("TextValue");
   }

   [Fact]
   public void Should_return_null_when_value_is_whitespace_only()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(T1Name = "   ")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindTxName(1);

      // Assert
      result.Should().BeNull();
   }
}
