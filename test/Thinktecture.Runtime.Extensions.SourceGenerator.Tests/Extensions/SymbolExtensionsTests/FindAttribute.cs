using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.SymbolExtensionsTests;

public class FindAttribute
{
   [Fact]
   public void Returns_null_when_no_attributes()
   {
      var src = @"
namespace Test;

public class C;
";
      var type = GetTypeSymbol(src);

      var attr = type.FindAttribute(_ => true);

      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_null_for_unknown_attribute_with_error_type()
   {
      var src = @"
namespace Test;

[UnknownAttribute] // not defined - Roslyn will create an ErrorTypeSymbol
public class C;
";
      var type = GetTypeSymbol(src);

      // Even if predicate would return true, the extension should skip Error type attributes.
      var attr = type.FindAttribute(_ => true);

      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_attribute_when_predicate_matches_known_attribute()
   {
      var src = @"
using System;

namespace Test;

[Obsolete]
public class C;
";
      var type = GetTypeSymbol(src);

      var attr = type.FindAttribute(t => t.Name == nameof(ObsoleteAttribute));

      attr.Should().NotBeNull();
      attr!.AttributeClass.Should().NotBeNull();
      attr.AttributeClass!.Name.Should().Be(nameof(ObsoleteAttribute));
   }

   [Fact]
   public void Returns_first_matching_attribute_when_multiple_present()
   {
      var src = @"
using System;

namespace Test;

[CLSCompliant(false)]
[Obsolete]
public class C;
";
      var type = GetTypeSymbol(src);

      // Match either CLSCompliantAttribute or ObsoleteAttribute - the first applied is CLSCompliant
      var attr = type.FindAttribute(t => t.Name is nameof(CLSCompliantAttribute) or nameof(ObsoleteAttribute));

      attr.Should().NotBeNull();
      attr!.AttributeClass!.Name.Should().Be(nameof(CLSCompliantAttribute));
   }

   [Fact]
   public void Returns_null_if_no_attribute_matches_predicate()
   {
      var src = @"
using System;

namespace Test;

[Obsolete]
public class C;
";
      var type = GetTypeSymbol(src);

      var attr = type.FindAttribute(t => t.Name == "SomeOtherAttribute");

      attr.Should().BeNull();
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName = "Test.C")
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "FindAttributeTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);

      return type ?? throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");
   }
}
