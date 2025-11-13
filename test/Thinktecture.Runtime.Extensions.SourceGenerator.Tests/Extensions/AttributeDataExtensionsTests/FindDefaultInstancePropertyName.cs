using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindDefaultInstancePropertyName : CompilationTestBase
{
   [Fact]
   public void Should_return_value_when_parameter_is_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(DefaultInstancePropertyName = "MyProperty")]
         public partial struct TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindDefaultInstancePropertyName();

      // Assert
      result.Should().Be("MyProperty");
   }

   [Fact]
   public void Should_trim_whitespace_from_value()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(DefaultInstancePropertyName = "  TrimMe  ")]
         public partial struct TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindDefaultInstancePropertyName();

      // Assert
      result.Should().Be("TrimMe");
   }

   [Fact]
   public void Should_return_null_when_parameter_is_empty_string()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(DefaultInstancePropertyName = "")]
         public partial struct TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindDefaultInstancePropertyName();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_parameter_is_whitespace_only()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(DefaultInstancePropertyName = "   ")]
         public partial struct TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindDefaultInstancePropertyName();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_parameter_is_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>]
         public partial struct TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindDefaultInstancePropertyName();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_parameter_is_explicitly_null()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(DefaultInstancePropertyName = null)]
         public partial struct TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindDefaultInstancePropertyName();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_special_characters_and_unicode()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(DefaultInstancePropertyName = "Property_123_αβγ")]
         public partial struct TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var result = attributeData.FindDefaultInstancePropertyName();

      // Assert
      result.Should().Be("Property_123_αβγ");
   }
}
