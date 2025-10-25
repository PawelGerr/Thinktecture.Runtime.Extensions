Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/NamedTypeSymbolExtensions.cs

- Severity: Warning
  Area: Constructor filtering logic (internal visibility)
  Details: Internal constructors are included only when baseType?.IsSameAssembly != true:
    (ctor.DeclaredAccessibility == Accessibility.Internal && baseType?.IsSameAssembly != true)
  This ties inclusion of the current type&#39;s internal constructors to whether its base type is in the same assembly. Source-generated code is emitted into the same assembly as the type under analysis, so internal constructors should generally be usable regardless of the base type&#39;s assembly. This condition risks silently excluding valid constructors when the base type happens to be in the same assembly.
  Suggestion: Reconsider the condition. If the intent is to exclude certain constructors, document the behavior and/or base it on the analyzed type&#39;s own assembly context rather than the base type&#39;s assembly.

- Severity: Warning
  Area: Suppression of implicit default constructor
  Details: The implicit default constructor is excluded only if baseType?.IsSameAssembly == true due to:
    && (!ctor.IsImplicitlyDeclared || baseType?.IsSameAssembly != true)
  The comment states: "default-ctor will be replaced by ctor implemented by this generator", which suggests the implicit default constructor should be excluded unconditionally to avoid duplicates. As written, the implicit default constructor is included when the base type is not in the same assembly (or null), which may yield duplicate constructors or inconsistent behavior depending on assembly composition.
  Suggestion: If the generator always replaces the default ctor, use "&& !ctor.IsImplicitlyDeclared" (without tying it to baseType.IsSameAssembly). If there&#39;s a nuanced reason, clarify in comments and add tests documenting the expected behavior.

- Severity: Warning
  Area: Potential null dereference reliance in GetBaseType
  Details: The code accesses type.BaseType.Kind after checking type.BaseType.IsNullOrObject():
    if (type.BaseType.IsNullOrObject() || type.BaseType.Kind == SymbolKind.ErrorType)
  This assumes IsNullOrObject() safely handles a null receiver. If that extension changes or isn&#39;t available, type.BaseType.Kind may dereference null.
  Suggestion: Cache baseType into a local and use explicit null check and SpecialType check:
    var baseType = type.BaseType;
    if (baseType is null || baseType.SpecialType == SpecialType.System_Object || baseType.Kind == SymbolKind.ErrorType) return null;

- Severity: Warning
  Area: Accessibility comparison semantics
  Details: HasLowerAccessibility compares enum values with "<":
    if (containingType.DeclaredAccessibility < accessibility)
  Roslyn’s Accessibility enum does not strictly define a total order that captures the lattice of access rights (e.g., Protected, Internal, ProtectedOrInternal, ProtectedAndInternal). Numeric "less than" works for many cases but can be misleading in edge cases.
  Suggestion: Consider a helper computing effective accessibility along the containing-type chain (or use Roslyn helpers, if available) to avoid relying on enum ordering.

- Severity: Info
  Area: Dependency on global usings
  Details: File uses Microsoft.CodeAnalysis symbols and ImmutableArray without explicit using directives. This relies on global usings (or project-wide Usings in MSBuild). If those are removed/changed, this file will fail to compile.
  Suggestion: Ensure global usings are defined for Microsoft.CodeAnalysis, Microsoft.CodeAnalysis.CSharp, System.Collections.Immutable, and System.Collections.Generic.

- Severity: Info
  Area: Language version features
  Details: Uses collection expressions (return [];) which require C# 12+. Ensure LangVersion is set accordingly (likely fine with .NET 9/C# 13).
  Suggestion: Confirm project LangVersion to avoid surprises on older compilers.
