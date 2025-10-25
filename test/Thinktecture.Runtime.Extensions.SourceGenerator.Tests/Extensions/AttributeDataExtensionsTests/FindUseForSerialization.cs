using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindUseForSerialization : CompilationTestBase
{
   [Fact]
   public void Should_return_SystemTextJson_when_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         public partial class MyValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyValueObject");
      var comparerAttribute = typeSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "ObjectFactoryAttribute");

      // Act
      var result = comparerAttribute.FindUseForSerialization();

      // Assert
      result.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson);
   }

   [Fact]
   public void Should_return_NewtonsoftJson_when_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         public partial class MyValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyValueObject");
      var comparerAttribute = typeSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "ObjectFactoryAttribute");

      // Act
      var result = comparerAttribute.FindUseForSerialization();

      // Assert
      result.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson);
   }

   [Fact]
   public void Should_return_MessagePack_when_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         public partial class MyValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyValueObject");
      var comparerAttribute = typeSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "ObjectFactoryAttribute");

      // Act
      var result = comparerAttribute.FindUseForSerialization();

      // Assert
      result.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack);
   }

   [Fact]
   public void Should_return_All_when_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         public partial class MyValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyValueObject");
      var comparerAttribute = typeSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "ObjectFactoryAttribute");

      // Act
      var result = comparerAttribute.FindUseForSerialization();

      // Assert
      result.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.All);
   }

   [Fact]
   public void Should_return_default_None_when_parameter_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>]
         public partial class MyValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyValueObject");
      var comparerAttribute = typeSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "ObjectFactoryAttribute");

      // Act
      var result = comparerAttribute.FindUseForSerialization();

      // Assert
      result.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.None);
   }

   [Fact]
   public void Should_handle_flags_combination()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson | SerializationFrameworks.NewtonsoftJson)]
         public partial class MyValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyValueObject");
      var comparerAttribute = typeSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "ObjectFactoryAttribute");

      // Act
      var result = comparerAttribute.FindUseForSerialization();

      // Assert
      var expected = Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson | Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson;
      result.Should().Be(expected);
   }
}
