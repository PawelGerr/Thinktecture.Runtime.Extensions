Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ObjectFactoryState.cs

- Missing constructor argument validation:
  - `type` is not validated for null. Misuse (reflection/external calls) will cause `NullReferenceException` when accessing `type.SpecialType` or `type.ToFullyQualifiedDisplayString()`.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`.

- Equality/hash ignore `SpecialType` while exposing it:
  - The `SpecialType` property is not part of `Equals`/`GetHashCode`. Two instances with identical `TypeFullyQualified` but different `SpecialType` (theoretically possible if symbol resolution changes) would compare equal and hash the same despite different exposed state.
  - Recommendation: Either include `SpecialType` in equality/hash or document that it is derived, non-identity metadata and may differ without affecting equality.

- Unnecessary cast to int for enum comparison:
  - `Equals(ObjectFactoryState?)` uses `(int)UseForSerialization == (int)other.UseForSerialization`. Direct enum equality is clearer (`UseForSerialization == other.UseForSerialization`) and prevents accidental reliance on underlying size.
  - Recommendation: Compare enums directly.

- String-based identity concerns:
  - Equality relies on `TypeFullyQualified` string. If `ToFullyQualifiedDisplayString()` is not canonicalized consistently (e.g., presence/absence of `global::`, aliasing), identity may be brittle across contexts.
  - Recommendation: Ensure `ToFullyQualifiedDisplayString()` returns a canonical, stable representation; otherwise normalize the string or consider storing additional identity (e.g., metadata name + nullability) for robustness.

- String hashing randomness:
  - `GetHashCode()` uses `TypeFullyQualified.GetHashCode()` which is randomized per process. This is fine for in-memory collections but unsuitable for persisted hashes or cross-process comparisons.
  - Recommendation: If hashes are ever persisted/serialized, compute a deterministic hash over a normalized representation instead.
