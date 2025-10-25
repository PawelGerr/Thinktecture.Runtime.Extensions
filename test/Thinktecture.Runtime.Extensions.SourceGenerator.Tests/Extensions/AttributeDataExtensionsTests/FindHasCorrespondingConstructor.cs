using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindHasCorrespondingConstructor : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_set_to_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>(HasCorrespondingConstructor = true)]
         public partial class MyClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyClass");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindHasCorrespondingConstructor();

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

         [ObjectFactory<string>(HasCorrespondingConstructor = false)]
         public partial class MyClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyClass");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindHasCorrespondingConstructor();

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

         [ObjectFactory<string>]
         public partial class MyClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyClass");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindHasCorrespondingConstructor();

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_work_with_struct()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>(HasCorrespondingConstructor = true)]
         public partial struct MyStruct
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyStruct");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindHasCorrespondingConstructor();

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_work_with_different_type_parameters()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<int>(HasCorrespondingConstructor = true)]
         public partial class MyIntClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyIntClass");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindHasCorrespondingConstructor();

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_work_with_complex_type_parameter()
   {
      // Arrange
      var source = """
         using Thinktecture;
         using System.Collections.Generic;

         namespace Test;

         [ObjectFactory<List<string>>(HasCorrespondingConstructor = false)]
         public partial class MyComplexClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyComplexClass");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindHasCorrespondingConstructor();

      // Assert
      result.Should().BeFalse();
   }
}
