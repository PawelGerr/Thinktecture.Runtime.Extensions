using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ParsableCodeGeneratorTests;

public class Constructor
{
   [Fact]
   public void ForValueObject_HasCorrectProperties()
   {
      // Arrange & Act
      var generator = ParsableCodeGenerator.ForValueObject;

      // Assert
      generator.CodeGeneratorName.Should().Be("Parsable-CodeGenerator");
      generator.FileNameSuffix.Should().Be(".Parsable");
   }

   [Fact]
   public void ForEnum_HasCorrectProperties()
   {
      // Arrange & Act
      var generator = ParsableCodeGenerator.ForEnum;

      // Assert
      generator.CodeGeneratorName.Should().Be("Parsable-CodeGenerator");
      generator.FileNameSuffix.Should().Be(".Parsable");
   }

   [Fact]
   public void ForValueObject_And_ForEnum_AreDifferentInstances()
   {
      // Assert
      ParsableCodeGenerator.ForValueObject.Should().NotBeSameAs(ParsableCodeGenerator.ForEnum);
   }
}
