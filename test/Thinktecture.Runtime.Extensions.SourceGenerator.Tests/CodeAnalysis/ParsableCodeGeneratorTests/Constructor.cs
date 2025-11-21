using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ParsableCodeGeneratorTests;

public class Constructor
{
   [Fact]
   public void HasCorrectProperties()
   {
      // Arrange & Act
      var generator = ParsableCodeGenerator.Instance;

      // Assert
      generator.CodeGeneratorName.Should().Be("Parsable-CodeGenerator");
      generator.FileNameSuffix.Should().Be(".Parsable");
   }
}
