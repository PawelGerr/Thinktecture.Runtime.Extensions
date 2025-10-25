Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParameterState.cs

- Missing constructor argument validation:
  - `name` and `type` are not validated. Misuse (reflection/external calls) can result in `NullReferenceException` in `GetHashCode()` (e.g., `Name.GetHashCode()`/`Type.GetHashCode()`).
  - Recommendation: Add `ArgumentException.ThrowIfNullOrWhiteSpace(name);` and `ArgumentException.ThrowIfNullOrWhiteSpace(type);`.

- Using raw string for type identity is brittle:
  - Equality/hash rely on textual `Type`. Formatting/casing/qualification differences (e.g., `int` vs `System.Int32`, `List<string>` vs `System.Collections.Generic.List<string>`, presence of `global::`, whitespace) will cause unequal results for semantically identical types.
  - Missing nullability/arity/modifier encoding in `Type` may also lead to collisions or false inequality depending on how `Type` is produced.
  - Recommendation: Store a canonical/stable identity (e.g., fully-qualified metadata name including generic arity and nullability) or use an `ITypeSymbol`-based identity with `SymbolEqualityComparer.IncludeNullability` and map to a stable string at construction.

- String comparison/hash intent not explicit:
  - Equality uses `==` and hashing uses `GetHashCode()` on strings (ordinal by default). The intent is implicit and hashing is randomized across processes.
  - Recommendation: Make intent explicit in code paths that use comparers (e.g., collections) by using `StringComparer.Ordinal` where applicable. Ensure hashes are not persisted/cross-run; if they are, use a deterministic hashing strategy on a normalized representation.

- No validation/constraints for `RefKind`:
  - Although an enum, there is no check that only supported values are used (e.g., `None`, `Ref`, `Out`, `In`). Unexpected values could slip through if the enum evolves.
  - Recommendation: Validate or document accepted values at creation sites.
