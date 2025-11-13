using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.SymbolExtensionsTests;

public class HasAttribute
{
   [Fact]
   public void Returns_false_when_no_attributes()
   {
      var src = @"
namespace Test;

public class C;
";
      var type = GetTypeSymbol(src);

      var result = type.HasAttribute(_ => true);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_unknown_attribute_with_error_type()
   {
      var src = @"
namespace Test;

[UnknownAttribute] // not defined -> ErrorTypeSymbol
public class C;
";
      var type = GetTypeSymbol(src);

      var result = type.HasAttribute(_ => true);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_predicate_matches_known_attribute()
   {
      var src = @"
using System;

namespace Test;

[Obsolete]
public class C;
";
      var type = GetTypeSymbol(src);

      var result = type.HasAttribute(t => t.Name == nameof(ObsoleteAttribute));

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_if_any_attribute_matches_predicate_when_multiple_present()
   {
      var src = @"
using System;

namespace Test;
[CLSCompliant(false)]
[Obsolete]
public class C;
";
      var type = GetTypeSymbol(src);

      // Predicate matches only Obsolete which is second
      var result = type.HasAttribute(t => t.Name == nameof(ObsoleteAttribute));

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_if_no_attribute_matches_predicate()
   {
      var src = @"
using System;

namespace Test;

[Obsolete]
public class C;
";
      var type = GetTypeSymbol(src);

      var result = type.HasAttribute(t => t.Name == "SomeOtherAttribute");

      result.Should().BeFalse();
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName = "Test.C")
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "HasAttributeTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);
      if (type is null)
         throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");

      return type;
   }
}
