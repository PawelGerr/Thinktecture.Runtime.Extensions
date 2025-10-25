using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindSwitchMapStateParameterName : CompilationTestBase
{
   [Fact]
   public void Should_return_custom_value_when_parameter_is_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>(SwitchMapStateParameterName = "context")]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("context");
   }

   [Fact]
   public void Should_return_default_state_when_parameter_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("state");
   }

   [Fact]
   public void Should_work_with_keyless_smart_enums()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum(SwitchMapStateParameterName = "data")]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new();
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("data");
   }

   [Fact]
   public void Should_work_with_unions()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>(SwitchMapStateParameterName = "ctx")]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("ctx");
   }

   [Fact]
   public void Should_trim_whitespace_from_value()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>(SwitchMapStateParameterName = "  context  ")]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("context");
   }

   [Fact]
   public void Should_return_default_state_when_value_is_empty_string()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>(SwitchMapStateParameterName = "")]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("state");
   }

   [Fact]
   public void Should_return_default_state_when_value_is_whitespace_only()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>(SwitchMapStateParameterName = "   ")]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("state");
   }

   [Fact]
   public void Should_handle_special_characters_and_unicode()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>(SwitchMapStateParameterName = "äbcß")]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSwitchMapStateParameterName();

      // Assert
      result.Should().Be("äbcß");
   }
}
