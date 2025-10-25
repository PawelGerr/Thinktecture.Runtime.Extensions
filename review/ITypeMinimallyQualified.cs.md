Issues found in ITypeMinimallyQualified.cs

1) Warning: Undefined basis for “minimally qualified”
- Problem: The term “minimally qualified” is context-dependent (depends on target namespace, available using directives/aliases, alias conflicts, file-scoped usings). The interface exposes only a property without any context, so it’s unclear relative to which scope the minimal name is computed. Different consumers may compute different outputs for the same symbol.
- Risk: Non-deterministic names and inconsistent code generation across files; collisions that compile in one context and fail in another.
- Fix options:
  - Define the basis explicitly in XML docs (e.g., “relative to the file namespace X with these using directives”).
  - Prefer an API that accepts context, e.g., `string GetTypeName(NameQualificationKind kind, in NameResolutionContext ctx)` or `string GetMinimallyQualifiedName(SymbolDisplayFormat format)`.
  - If used for source emission, consider avoiding “minimal” entirely and rely on fully qualified names plus controlled usings.

2) Warning: Unsafe for code emission
- Problem: Minimal names require specific using directives and can still be ambiguous if consumers introduce conflicting types/aliases.
- Risk: Generated code may break when integrated into solutions with different usings or type aliases.
- Fix options:
  - Document that `TypeMinimallyQualified` must not be used for code emission; use fully-qualified names with `global::` instead.
  - If minimal names are intended for emission, pair with a mechanism that emits and owns the required using directives for every generated file deterministically.

3) Warning: Ambiguous formatting semantics
- Problem: The contract doesn’t state how to format:
  - Generics (arity ticks vs. type arguments, nested generic args minimally/fully?)
  - Nested types (`Outer.Inner` vs `Outer+Inner`)
  - Arrays, pointers, by-ref (ref/out/in), function pointers, tuples
  - Nullable reference annotations (`string?`) and nullable value types (`int?`)
- Risk: Divergent implementations and fragile consumers.
- Fix options:
  - Define precise invariants in XML docs, ideally aligned with a Roslyn `SymbolDisplayFormat`.
  - Provide examples covering tricky cases.

4) Warning: Naming consistency and discoverability
- Problem: `TypeMinimallyQualified` uses unusual word order; common patterns are `MinimallyQualifiedTypeName` or `MinimallyQualifiedName`.
- Risk: Reduced readability and asymmetric naming versus `ITypeFullyQualified` (previously flagged).
- Fix options:
  - Rename to `MinimallyQualifiedTypeName` (or `MinimallyQualifiedName`) for consistency with .NET naming and the “FullyQualifiedTypeName” counterpart.

5) Warning: Contract does not guarantee non-null/non-empty
- Problem: The property type is non-nullable `string` but the contract doesn’t state non-empty/whitespace constraints.
- Risk: Callers must defensively check; implementations may return empty strings.
- Fix options:
  - Document “never null/empty; throws if not representable,” or change to `string?` with defined semantics.

6) Suggestion: Performance/caching guidance
- Problem: Minimal name computation via Roslyn can allocate; recomputing on every access is wasteful.
- Fix options:
  - Document expectation that implementations cache the computed value per symbol/format.
  - Provide helper/factory to compute once and reuse.

7) Suggestion: Consolidate naming APIs
- Problem: Having separate interfaces for minimal vs fully qualified can lead to duplication and drift.
- Fix options:
  - Introduce a single API that returns names for multiple formats, e.g.:
    - `string GetTypeName(SymbolDisplayFormat format)` or
    - `string GetTypeName(NameQualificationKind kind)` where `kind ∈ { FullyQualified, MinimallyQualified, Code, Metadata }`.
  - Keep convenience properties if needed, but base them on the single underlying formatter.

8) Warning: Type parameters and unbound generics
- Problem: The contract does not define expectations for type parameters (`T`) with/without constraints and unbound generics (`List<>`).
- Risk: Inconsistent string outputs for generic contexts.
- Fix options:
  - Specify expected forms (e.g., `T` for type parameters; for unbound generics either disallow or define `List<>` vs `List<T>` conventions).
