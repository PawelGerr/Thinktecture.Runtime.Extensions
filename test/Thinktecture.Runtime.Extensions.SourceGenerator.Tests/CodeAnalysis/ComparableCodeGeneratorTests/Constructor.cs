using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ComparableCodeGeneratorTests;

public class Constructor
{
   [Fact]
   public void Constructor_WithNullComparerAccessor_CreatesInstance()
   {
      // Arrange & Act
      var generator = new ComparableCodeGenerator(null);

      // Assert
      generator.Should().NotBeNull();
      generator.CodeGeneratorName.Should().Be("Comparable-CodeGenerator");
      generator.FileNameSuffix.Should().Be(".Comparable");
   }

   [Fact]
   public void Constructor_WithComparerAccessor_CreatesInstance()
   {
      // Arrange & Act
      var generator = new ComparableCodeGenerator("MyComparer");

      // Assert
      generator.Should().NotBeNull();
      generator.CodeGeneratorName.Should().Be("Comparable-CodeGenerator");
      generator.FileNameSuffix.Should().Be(".Comparable");
   }

   [Fact]
   public void Constructor_WithEmptyStringComparerAccessor_CreatesInstance()
   {
      // Arrange & Act
      var generator = new ComparableCodeGenerator(string.Empty);

      // Assert
      generator.Should().NotBeNull();
   }

   [Fact]
   public void Constructor_WithWhitespaceComparerAccessor_CreatesInstance()
   {
      // Arrange & Act
      var generator = new ComparableCodeGenerator("   ");

      // Assert
      generator.Should().NotBeNull();
   }

   [Fact]
   public void Default_Property_ReturnsInstanceWithNullComparer()
   {
      // Arrange & Act
      var generator = ComparableCodeGenerator.Default;

      // Assert
      generator.Should().NotBeNull();
      generator.CodeGeneratorName.Should().Be("Comparable-CodeGenerator");
      generator.FileNameSuffix.Should().Be(".Comparable");
   }

   [Fact]
   public void Default_Property_ReturnsSameInstance()
   {
      // Arrange & Act
      var generator1 = ComparableCodeGenerator.Default;
      var generator2 = ComparableCodeGenerator.Default;

      // Assert
      generator1.Should().BeSameAs(generator2);
   }

   [Fact]
   public void CodeGeneratorName_Property_ReturnsCorrectValue()
   {
      // Arrange
      var generator = new ComparableCodeGenerator("Test");

      // Act
      var name = generator.CodeGeneratorName;

      // Assert
      name.Should().Be("Comparable-CodeGenerator");
   }

   [Fact]
   public void FileNameSuffix_Property_ReturnsCorrectValue()
   {
      // Arrange
      var generator = new ComparableCodeGenerator("Test");

      // Act
      var suffix = generator.FileNameSuffix;

      // Assert
      suffix.Should().Be(".Comparable");
   }
}
