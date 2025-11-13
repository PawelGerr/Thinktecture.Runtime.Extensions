using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindAllowDefaultStructs : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_set_to_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ValueObject<int>(AllowDefaultStructs = true)]
         public partial struct StructValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.StructValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindAllowDefaultStructs();

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

         [ValueObject<int>(AllowDefaultStructs = false)]
         public partial struct StructValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.StructValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindAllowDefaultStructs();

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

         [ValueObject<int>]
         public partial struct StructValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.StructValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindAllowDefaultStructs();

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_work_with_ComplexValueObject()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(AllowDefaultStructs = true)]
         public partial struct MyStruct
         {
            public int Value { get; }
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyStruct");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindAllowDefaultStructs();

      // Assert
      result.Should().BeTrue();
   }
}
