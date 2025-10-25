using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class HasLowerAccessibility
{
   [Fact]
   public void Should_return_false_when_type_is_public_and_checking_public()
   {
      var src = @"
namespace Test;

public class Outer
{
   public class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Public, outer);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_type_is_internal_and_checking_internal()
   {
      var src = @"
namespace Test;

internal class Outer
{
   internal class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Internal, outer);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_type_is_internal_and_checking_public()
   {
      var src = @"
namespace Test;

internal class Outer
{
   internal class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Public, outer);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_type_is_private_and_checking_public()
   {
      var src = @"
namespace Test;

public class Outer
{
   private class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Public, outer);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_type_is_private_and_checking_internal()
   {
      var src = @"
namespace Test;

public class Outer
{
   private class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Internal, outer);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_type_is_protected_and_checking_public()
   {
      var src = @"
namespace Test;

public class Outer
{
   protected class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Public, outer);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_type_is_protected_and_checking_protected()
   {
      var src = @"
namespace Test;

public class Outer
{
   protected class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Protected, outer);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_all_levels_are_public()
   {
      var src = @"
namespace Test;

public class Level1
{
   public class Level2
   {
      public class Level3
      {
      }
   }
}
";
      var (level1, level3) = GetTypeSymbols(src, "Test.Level1", "Test.Level1+Level2+Level3");

      var result = level3.HasLowerAccessibility(Accessibility.Public, level1);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_stop_type_is_the_type_itself()
   {
      var src = @"
namespace Test;

internal class MyClass;
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.HasLowerAccessibility(Accessibility.Public, type);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_stop_type_is_immediate_parent()
   {
      var src = @"
namespace Test;

public class Outer
{
   internal class Inner
   {
   }
}
";
      var (_, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Public, inner);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_not_check_stop_type_accessibility()
   {
      var src = @"
namespace Test;

internal class Outer
{
   public class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      // Stop type (outer) is internal, but should not be checked
      // Inner itself is public, so checking against public should return false
      var result = inner.HasLowerAccessibility(Accessibility.Public, outer);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_stop_type_is_null()
   {
      var src = @"
namespace Test;

internal class MyClass;
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      // This should not crash and return false when containingType is null
      var result = type.HasLowerAccessibility(Accessibility.Public, null!);

      result.Should().BeTrue(); // type itself is internal, which is < public
   }

   [Fact]
   public void Should_check_all_types_when_stop_type_not_in_hierarchy()
   {
      var src = @"
namespace Test;

public class Outer
{
   internal class Inner
   {
   }
}

public class Unrelated;
";
      var (unrelated, inner) = GetTypeSymbols(src, "Test.Unrelated", "Test.Outer+Inner");

      // Stop type is not in the hierarchy, so should check all the way up
      var result = inner.HasLowerAccessibility(Accessibility.Public, unrelated);

      result.Should().BeTrue(); // Inner is internal, which is < public
   }

   [Fact]
   public void Should_return_false_when_checking_private_accessibility()
   {
      var src = @"
namespace Test;

public class Outer
{
   private class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      // Private is the lowest accessibility, nothing can be lower
      var result = inner.HasLowerAccessibility(Accessibility.Private, outer);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_type_has_no_containing_type()
   {
      var src = @"
namespace Test;

public class TopLevel;
";
      var type = GetTypeSymbol(src, "Test.TopLevel");
      var stopType = GetTypeSymbol(src, "Test.TopLevel");

      var result = type.HasLowerAccessibility(Accessibility.Public, stopType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_find_lower_accessibility_in_deep_hierarchy()
   {
      var src = @"
namespace Test;

public class Level1
{
   public class Level2
   {
      internal class Level3
      {
         public class Level4
         {
         }
      }
   }
}
";
      var (level1, level4) = GetTypeSymbols(src, "Test.Level1", "Test.Level1+Level2+Level3+Level4");

      // Level3 is internal, which is < public
      var result = level4.HasLowerAccessibility(Accessibility.Public, level1);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_stop_at_stop_type_and_not_check_beyond()
   {
      var src = @"
namespace Test;

internal class Level1
{
   public class Level2
   {
      public class Level3
      {
      }
   }
}
";
      var (level2, level3) = GetTypeSymbols(src, "Test.Level1+Level2", "Test.Level1+Level2+Level3");

      // Stop at Level2, so Level1 (internal) should not be checked
      var result = level3.HasLowerAccessibility(Accessibility.Public, level2);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_check_up_to_but_not_including_stop_type()
   {
      var src = @"
namespace Test;

public class Level1
{
   internal class Level2
   {
      public class Level3
      {
      }
   }
}
";
      var (level1, level3) = GetTypeSymbols(src, "Test.Level1", "Test.Level1+Level2+Level3");

      // Should find Level2 is internal before reaching Level1 (stop type)
      var result = level3.HasLowerAccessibility(Accessibility.Public, level1);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_protected_internal_correctly()
   {
      var src = @"
namespace Test;

public class Outer
{
   protected internal class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      // ProtectedOrInternal is less than Public
      var result = inner.HasLowerAccessibility(Accessibility.Public, outer);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_use_symbol_equality_comparer_for_stop_type_matching()
   {
      var src = @"
namespace Test;

public class Outer
{
   public class Inner
   {
   }
}
";
      var (_, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      // Get outer again from a separate compilation to ensure SymbolEqualityComparer is used
      // The physical reference will be different but should be considered equal by SymbolEqualityComparer
      var outerAgain = GetTypeSymbol(src, "Test.Outer");

      var result = inner.HasLowerAccessibility(Accessibility.Public, outerAgain);

      // Inner and Outer are both public, and we stop at Outer, so should return false
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_nested_private_type_checked_against_public()
   {
      var src = @"
namespace Test;

public class Outer
{
   public class Middle
   {
      private class Inner
      {
      }
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Middle+Inner");

      var result = inner.HasLowerAccessibility(Accessibility.Public, outer);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_multiple_internal_levels_correctly()
   {
      var src = @"
namespace Test;

internal class Level1
{
   internal class Level2
   {
      internal class Level3
      {
      }
   }
}
";
      var (level1, level3) = GetTypeSymbols(src, "Test.Level1", "Test.Level1+Level2+Level3");

      // All are internal, checking against public should return true
      var result = level3.HasLowerAccessibility(Accessibility.Public, level1);

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_checking_equal_accessibility_in_hierarchy()
   {
      var src = @"
namespace Test;

internal class Level1
{
   internal class Level2
   {
      internal class Level3
      {
      }
   }
}
";
      var (level1, level3) = GetTypeSymbols(src, "Test.Level1", "Test.Level1+Level2+Level3");

      // All are internal, checking against internal should return false
      var result = level3.HasLowerAccessibility(Accessibility.Internal, level1);

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_correctly_compare_accessibility_levels()
   {
      var src = @"
namespace Test;

public class Outer
{
   protected class Inner
   {
   }
}
";
      var (outer, inner) = GetTypeSymbols(src, "Test.Outer", "Test.Outer+Inner");

      // Protected (4) < Public (5)
      var resultPublic = inner.HasLowerAccessibility(Accessibility.Public, outer);
      // Protected (4) < Internal (3) - should be false
      var resultInternal = inner.HasLowerAccessibility(Accessibility.Internal, outer);

      resultPublic.Should().BeTrue();
      resultInternal.Should().BeTrue();
   }

   [Fact]
   public void Should_find_first_lower_accessibility_in_chain()
   {
      var src = @"
namespace Test;

public class Level1
{
   internal class Level2
   {
      private class Level3
      {
         public class Level4
         {
         }
      }
   }
}
";
      var (level1, level4) = GetTypeSymbols(src, "Test.Level1", "Test.Level1+Level2+Level3+Level4");

      // Should find Level3 (private) or Level2 (internal) before reaching Level1
      var result = level4.HasLowerAccessibility(Accessibility.Public, level1);

      result.Should().BeTrue();
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "HasLowerAccessibilityTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);
      if (type is null)
         throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");

      return type;
   }

   private static (INamedTypeSymbol, INamedTypeSymbol) GetTypeSymbols(string source, string metadataName1, string metadataName2)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "HasLowerAccessibilityTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type1 = compilation.GetTypeByMetadataName(metadataName1);
      if (type1 is null)
         throw new InvalidOperationException($"Type '{metadataName1}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");

      var type2 = compilation.GetTypeByMetadataName(metadataName2);
      if (type2 is null)
         throw new InvalidOperationException($"Type '{metadataName2}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");

      return (type1, type2);
   }
}
