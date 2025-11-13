using System;
using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindDefaultStringComparison : CompilationTestBase
{
   [Fact]
   public void Should_return_Ordinal_when_set()
   {
      // Arrange
      var source = """
         using System;
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(DefaultStringComparison = StringComparison.Ordinal)]
         public partial class ProductCode
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.ProductCode");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.Ordinal);
   }

   [Fact]
   public void Should_return_OrdinalIgnoreCase_when_set()
   {
      // Arrange
      var source = """
         using System;
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
         public partial class ProductCode
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.ProductCode");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.OrdinalIgnoreCase);
   }

   [Fact]
   public void Should_return_default_OrdinalIgnoreCase_when_parameter_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ComplexValueObject]
         public partial class ProductCode
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.ProductCode");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.OrdinalIgnoreCase);
   }

   [Fact]
   public void Should_return_CurrentCulture_when_set()
   {
      // Arrange
      var source = """
         using System;
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(DefaultStringComparison = StringComparison.CurrentCulture)]
         public partial class ProductCode
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.ProductCode");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.CurrentCulture);
   }

   [Fact]
   public void Should_return_CurrentCultureIgnoreCase_when_set()
   {
      // Arrange
      var source = """
         using System;
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(DefaultStringComparison = StringComparison.CurrentCultureIgnoreCase)]
         public partial class ProductCode
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.ProductCode");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.CurrentCultureIgnoreCase);
   }

   [Fact]
   public void Should_return_InvariantCulture_when_set()
   {
      // Arrange
      var source = """
         using System;
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(DefaultStringComparison = StringComparison.InvariantCulture)]
         public partial class ProductCode
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.ProductCode");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.InvariantCulture);
   }

   [Fact]
   public void Should_return_InvariantCultureIgnoreCase_when_set()
   {
      // Arrange
      var source = """
         using System;
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(DefaultStringComparison = StringComparison.InvariantCultureIgnoreCase)]
         public partial class ProductCode
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.ProductCode");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.InvariantCultureIgnoreCase);
   }

   [Fact]
   public void Should_work_with_ComplexValueObject()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ComplexValueObject(DefaultStringComparison = System.StringComparison.Ordinal)]
         public partial class MyComplexValueObject
         {
            public string Value { get; }
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyComplexValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindDefaultStringComparison();

      // Assert
      result.Should().Be(StringComparison.Ordinal);
   }
}
