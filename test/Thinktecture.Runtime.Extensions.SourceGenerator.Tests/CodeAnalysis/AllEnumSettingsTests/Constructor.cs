using System.Linq;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.AllEnumSettingsTests;

public class Constructor : CompilationTestBase
{
   [Fact]
   public void Should_keep_both_false_when_both_are_false()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIParsable = false, SkipISpanParsable = false)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().BeFalse();
      settings.SkipISpanParsable.Should().BeFalse();
   }

   [Fact]
   public void Should_keep_SkipIParsable_false_and_SkipISpanParsable_true_when_only_SkipISpanParsable_is_true()
   {
      // Arrange - SkipISpanParsable = true has no effect on SkipIParsable
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIParsable = false, SkipISpanParsable = true)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().BeFalse();
      settings.SkipISpanParsable.Should().BeTrue();
   }

   [Fact]
   public void Should_force_SkipISpanParsable_true_when_SkipIParsable_is_true()
   {
      // Arrange - User sets SkipIParsable = true, SkipISpanParsable = false
      // Expected: SkipISpanParsable should be forced to true because IParsable<T> is required for ISpanParsable<T>
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIParsable = true, SkipISpanParsable = false)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().BeTrue();
      settings.SkipISpanParsable.Should().BeTrue("ISpanParsable<T> requires IParsable<T>, so SkipISpanParsable must be true when SkipIParsable is true");
   }

   [Fact]
   public void Should_keep_both_true_when_both_are_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIParsable = true, SkipISpanParsable = true)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().BeTrue();
      settings.SkipISpanParsable.Should().BeTrue();
   }

   [Fact]
   public void Should_default_both_to_false_when_not_specified()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().BeFalse();
      settings.SkipISpanParsable.Should().BeFalse();
   }

   [Fact]
   public void Should_force_SkipISpanParsable_true_when_only_SkipIParsable_true_is_set()
   {
      // Arrange - User sets only SkipIParsable = true, SkipISpanParsable defaults to false
      // Expected: SkipISpanParsable should be forced to true
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIParsable = true)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().BeTrue();
      settings.SkipISpanParsable.Should().BeTrue("SkipIParsable = true forces SkipISpanParsable = true");
   }

   [Fact]
   public void Should_respect_SkipISpanParsable_true_when_only_SkipISpanParsable_is_set()
   {
      // Arrange - User sets only SkipISpanParsable = true, SkipIParsable defaults to false
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipISpanParsable = true)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().BeFalse();
      settings.SkipISpanParsable.Should().BeTrue();
   }

   [Theory]
   [InlineData(false, false, false, false)] // Both false: no change
   [InlineData(false, true, false, true)]   // Only ISpanParsable skipped: no change
   [InlineData(true, false, true, true)]    // IParsable skipped forces ISpanParsable skipped
   [InlineData(true, true, true, true)]     // Both true: no change
   public void Should_apply_dependency_constraint_for_all_combinations(
      bool skipIParsable,
      bool skipISpanParsable,
      bool expectedSkipIParsable,
      bool expectedSkipISpanParsable)
   {
      // Arrange
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [SmartEnum<int>(SkipIParsable = {{skipIParsable.ToString().ToLowerInvariant()}}, SkipISpanParsable = {{skipISpanParsable.ToString().ToLowerInvariant()}})]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = new(1);

            private TestEnum(int key)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testEnumSymbol = GetTypeSymbol(compilation, "TestNamespace.TestEnum");
      var attributeData = testEnumSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("SmartEnumAttribute") == true);

      // Act
      var settings = new AllEnumSettings(attributeData);

      // Assert
      settings.SkipIParsable.Should().Be(expectedSkipIParsable);
      settings.SkipISpanParsable.Should().Be(expectedSkipISpanParsable);
   }
}
