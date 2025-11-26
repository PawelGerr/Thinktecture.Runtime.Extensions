using System.Linq;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.AllValueObjectSettingsTests;

public class Constructor : CompilationTestBase
{
   [Fact]
   public void Should_keep_both_false_when_both_are_false()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipIParsable = false, SkipISpanParsable = false)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

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

         [ValueObject<int>(SkipIParsable = false, SkipISpanParsable = true)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

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

         [ValueObject<int>(SkipIParsable = true, SkipISpanParsable = false)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

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

         [ValueObject<int>(SkipIParsable = true, SkipISpanParsable = true)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

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

         [ValueObject<int>]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

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

         [ValueObject<int>(SkipIParsable = true)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

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

         [ValueObject<int>(SkipISpanParsable = true)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

      // Assert
      settings.SkipIParsable.Should().BeFalse();
      settings.SkipISpanParsable.Should().BeTrue();
   }

   [Fact]
   public void Should_skip_both_when_SkipFactoryMethods_is_true()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipFactoryMethods = true, SkipIParsable = false, SkipISpanParsable = false)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

      // Assert
      settings.SkipIParsable.Should().BeTrue("SkipFactoryMethods forces SkipIParsable to true");
      settings.SkipISpanParsable.Should().BeTrue("SkipFactoryMethods forces SkipISpanParsable to true");
   }

   [Fact]
   public void Should_skip_both_when_SkipFactoryMethods_is_true_regardless_of_explicit_values()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipFactoryMethods = true, SkipIParsable = false, SkipISpanParsable = false)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

      // Assert
      settings.SkipIParsable.Should().BeTrue();
      settings.SkipISpanParsable.Should().BeTrue();
      settings.SkipFactoryMethods.Should().BeTrue();
   }

   [Theory]
   [InlineData(false, false, false, false, false)] // Both false: no change
   [InlineData(false, false, true, false, true)]   // Only ISpanParsable skipped: no change
   [InlineData(false, true, false, true, true)]    // IParsable skipped forces ISpanParsable skipped
   [InlineData(false, true, true, true, true)]     // Both true: no change
   [InlineData(true, false, false, true, true)]    // SkipFactoryMethods overrides everything
   [InlineData(true, true, true, true, true)]      // SkipFactoryMethods with both true
   public void Should_apply_dependency_constraint_for_all_combinations(
      bool skipFactoryMethods,
      bool skipIParsable,
      bool skipISpanParsable,
      bool expectedSkipIParsable,
      bool expectedSkipISpanParsable)
   {
      // Arrange
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>(SkipFactoryMethods = {{skipFactoryMethods.ToString().ToLowerInvariant()}}, SkipIParsable = {{skipIParsable.ToString().ToLowerInvariant()}}, SkipISpanParsable = {{skipISpanParsable.ToString().ToLowerInvariant()}})]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: false);

      // Assert
      settings.SkipIParsable.Should().Be(expectedSkipIParsable);
      settings.SkipISpanParsable.Should().Be(expectedSkipISpanParsable);
   }

   [Fact]
   public void Should_handle_string_key_with_dependency_constraint()
   {
      // Arrange - With string key and dependency constraint
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<string>(SkipIParsable = true, SkipISpanParsable = false)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testValueObjectSymbol = GetTypeSymbol(compilation, "TestNamespace.TestValueObject");
      var attributeData = testValueObjectSymbol.GetAttributes().First(a => a.AttributeClass?.Name.StartsWith("ValueObjectAttribute") == true);

      // Act
      var settings = new AllValueObjectSettings(attributeData, hasStringKey: true);

      // Assert
      settings.SkipIParsable.Should().BeTrue();
      settings.SkipISpanParsable.Should().BeTrue("Dependency constraint applies: SkipIParsable = true forces SkipISpanParsable = true");
   }
}
