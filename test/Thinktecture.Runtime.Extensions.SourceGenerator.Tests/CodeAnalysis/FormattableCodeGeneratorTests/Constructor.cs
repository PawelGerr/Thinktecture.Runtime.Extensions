using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.FormattableCodeGeneratorTests;

public class Constructor
{
   [Fact]
   public void Instance_Property_ReturnsNonNullInstance()
   {
      // Arrange & Act
      var generator = FormattableCodeGenerator.Instance;

      // Assert
      generator.Should().NotBeNull();
   }

   [Fact]
   public void Instance_Property_ReturnsSameInstance()
   {
      // Arrange & Act
      var generator1 = FormattableCodeGenerator.Instance;
      var generator2 = FormattableCodeGenerator.Instance;

      // Assert
      generator1.Should().BeSameAs(generator2);
   }

   [Fact]
   public void CodeGeneratorName_Property_ReturnsCorrectValue()
   {
      // Arrange
      var generator = FormattableCodeGenerator.Instance;

      // Act
      var name = generator.CodeGeneratorName;

      // Assert
      name.Should().Be("Formattable-CodeGenerator");
   }

   [Fact]
   public void FileNameSuffix_Property_ReturnsCorrectValue()
   {
      // Arrange
      var generator = FormattableCodeGenerator.Instance;

      // Act
      var suffix = generator.FileNameSuffix;

      // Assert
      suffix.Should().Be(".Formattable");
   }
}
