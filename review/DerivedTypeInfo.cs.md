Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DerivedTypeInfo.cs

- Incorrect/default equality semantics for Roslyn symbols:
  - The readonly record struct relies on compiler-generated equality/hash for fields `INamedTypeSymbol Type` and `INamedTypeSymbol TypeDef`. That uses reference equality and object.GetHashCode, which is not appropriate for Roslyn symbols across compilations or symbol instances.
  - Risk: Two symbol instances representing the same type will compare unequal and hash differently; dictionary/set behavior and de-duplication will be incorrect.
  - Recommendation: Provide an explicit equality and hash implementation using `SymbolEqualityComparer.IncludeNullability` (or desired variant) and `SymbolEqualityComparer.GetHashCode(symbol)`. Alternatively, store stable identity (e.g., fully qualified metadata name + nullability + arity) rather than raw symbols.

- Default(record struct) allows null symbol fields:
  - As a struct, `default(DerivedTypeInfo)` yields `Type`/`TypeDef` = null. While generated equality/hash are null-safe, consumers accessing properties may hit NREs.
  - Recommendation: Either (a) convert to a class to avoid invalid default, (b) keep as struct but add an `IsDefault/IsValid` convention and guard usage at call sites, or (c) redesign to store non-nullable stable identities.

- Potentially long-lived symbol retention:
  - Holding `INamedTypeSymbol` fields can retain large portions of the Roslyn graph if these states are cached. Other parts of the codebase avoid this via capture flags.
  - Recommendation: Prefer storing lightweight, stable identifiers (e.g., namespace + name + arity, or fully-qualified metadata name) and re-resolve symbols when needed.

- Unconstrained Level:
  - `int Level` has no validation or documented constraints (e.g., non-negative). Negative or inconsistent values could propagate silently.
  - Recommendation: Enforce non-negative levels or document/validate semantics at creation sites.

- Inconsistency with IHashCodeComputable:
  - Many CodeAnalysis state types implement `IHashCodeComputable` for consistency and testability. This type does not, relying on generated record struct hash, which is problematic with symbol fields (see first point).
  - Recommendation: Implement `IHashCodeComputable` with symbol-aware hashing to align with other state types and ensure deterministic behavior.
