Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/SourceProductionContextExtensions.cs

Summary
- Errors: 0
- Warnings: 3

Warnings
1) Missing explicit usings for Roslyn and Linq
- What: Types SourceProductionContext, Diagnostic, and Location, and LINQ (Select) are used without corresponding using directives in this file.
- Why it matters: The file implicitly depends on global usings; this is fragile and reduces portability. It may fail to compile if global usings are removed or altered.
- Recommendation:
  Add explicit usings to this file:
  using Microsoft.CodeAnalysis;
  using System.Linq;

2) Leading dot possible in hint name when namespace is empty string
- What: Only null is checked for typeNamespace. If an empty or whitespace string is passed, the hint name will start with a dot (".TypeName...").
- Why it matters: Produces inconsistent hint names; can make generated artifacts harder to navigate.
- Recommendation:
  Replace the namespace prefix computation with whitespace handling:
  var nsPrefix = String.IsNullOrWhiteSpace(typeNamespace) ? null : $"{typeNamespace}.";

  Then:
  var hintName = $"{nsPrefix}{containingTypeNames}{typeName}{(numberOfGenerics > 0 ? $"`{numberOfGenerics}" : null)}{fileNameSuffix}.g.cs";

3) Possible mismatch between DiagnosticDescriptor and provided arguments
- What: ReportError passes 2 message arguments (name, message) to Diagnostic.Create with DiagnosticsDescriptors.ErrorDuringGeneration.
- Why it matters: If the descriptor&#39;s MessageFormat does not include two placeholders (e.g., "{0}" and "{1}"), this will throw at runtime during generation.
- Recommendation:
  Ensure DiagnosticsDescriptors.ErrorDuringGeneration.MessageFormat expects exactly two arguments (e.g., "Error in {0}: {1}"). If not, adjust either the descriptor or the arguments passed here.
