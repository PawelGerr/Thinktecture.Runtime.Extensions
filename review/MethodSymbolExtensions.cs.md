- Unsafe cast and Single() usage in GetIdentifier:
  - Problem: Casts the syntax node to MethodDeclarationSyntax and uses DeclaringSyntaxReferences.Single(). This will throw for:
    - Non-ordinary methods (constructors use ConstructorDeclarationSyntax, operators use OperatorDeclarationSyntax, local functions, etc.).
    - Partial methods (multiple declarations) or metadata-only methods (zero declarations).
  - Impact: Can crash source generator/analyzers at analysis time when encountering such methods, leading to failed builds.
  - Recommendation:
    - Guard: if (method.MethodKind != MethodKind.Ordinary) return default or handle accordingly.
    - Use method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(...) and OfType<MethodDeclarationSyntax>().FirstOrDefault() to avoid InvalidCastException.
    - Consider falling back to method.Locations[0] when no MethodDeclarationSyntax is available, or change return type to SyntaxToken? and return default.

- Operator helpers lack method-kind/static checks:
  - Problem: IsComparisonOperator/IsArithmeticOperator only check return/parameter types but not:
    - method.MethodKind == MethodKind.UserDefinedOperator
    - method.IsStatic
  - Impact: Potential false positives if these helpers are reused for non-operator methods that coincidentally match the signature shape.
  - Recommendation: Add guards for MethodKind.UserDefinedOperator and method.IsStatic (and possibly check method.Name starts with "op_") to make the intent explicit and robust.

- Potential nullability strictness causing false negatives:
  - Problem: Uses SymbolEqualityComparer.IncludeNullability for return/parameter type checks. When the operator is defined on non-nullable T but the analysis compares to a nullable T? (or vice versa), the match will fail.
  - Impact: Operators may be missed if the caller passes a symbol with different nullability annotation even though it's the same underlying type.
  - Recommendation: Consider SymbolEqualityComparer.Default unless strict nullability matching is required by design; or normalize to the non-nullable underlying type before comparison.

- Reliance on external/global using for Microsoft.CodeAnalysis:
  - Problem: File references Roslyn symbols (IMethodSymbol, ITypeSymbol, SyntaxToken, SpecialType) but has no explicit using Microsoft.CodeAnalysis.
  - Impact: Compilation depends on a global using elsewhere; without it this file will not compile.
  - Recommendation: Add using Microsoft.CodeAnalysis; locally to reduce coupling to global usings and improve file portability.
