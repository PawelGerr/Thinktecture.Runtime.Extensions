# Review: TypeParameterSymbolExtensions.cs

Issues found (errors and warnings only):

- Warning: Implicit dependency on global usings
  - The file references ImmutableArray and ITypeParameterSymbol without local using directives (System.Collections.Immutable and Microsoft.CodeAnalysis). It also uses IReadOnlyList and List without an explicit using for System.Collections.Generic.
  - If the project does not provide global usings for these namespaces, this will fail to compile.
  - Recommendation: Either ensure corresponding global usings exist project-wide, or add:
    - using System.Collections.Generic;
    - using System.Collections.Immutable;
    - using Microsoft.CodeAnalysis;

- Warning: Language version dependency — C# 12 collection expression
  - The return [] relies on C# 12 collection expressions with target-typed conversion to IReadOnlyList<GenericTypeParameterState>.
  - If LangVersion is below 12 for this project, this will not compile.
  - Recommendation: Prefer Array.Empty<GenericTypeParameterState>() for broader compatibility and to avoid relying on C# 12:
    - if (generics.IsDefaultOrEmpty)
        return Array.Empty<GenericTypeParameterState>();
