Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/MemberInformationComparer.cs

- Potentially insufficient identity for members:
  - Equality uses only `Name`, `TypeFullyQualified`, `IsReferenceType`, and `SpecialType`. It ignores other characteristics that can distinguish members:
    - Static vs instance, accessibility, abstract/virtual/override, readonly, const, etc.
    - Nullability (e.g., `string` vs `string?`, `T` vs `T?`), array rank, generic arity/arguments if not encoded in `TypeFullyQualified`.
  - If this comparer is used across different contexts (beyond within a single containing type), it can consider different members equal.
  - Recommendation: Confirm the intended scope. If used for global sets/deduplication, extend identity to include containing type (or ensure callers constrain by container) and relevant modifiers. Ensure `TypeFullyQualified` is canonical and encodes nullability/arrays/generics; otherwise include additional flags (e.g., `NullableAnnotation`, `IsNullableStruct`).

- Missing containing type/namespace in equality:
  - Two members from different types/namespaces with the same `Name` and type signature will compare equal and hash the same.
  - Recommendation: Include containing type identity (e.g., fully qualified containing type name) in equality/hash or clearly document that this comparer must only be used within a single containing type context.

- String comparison/hash intent not explicit:
  - Uses `==` and `GetHashCode()` on strings (ordinal, process-randomized). Works for in-memory collections, but the semantics are implicit.
  - Recommendation: Use `StringComparer.Ordinal.Equals(...)` for clarity where applicable. If deterministic/persisted hashing is required anywhere, compute a stable hash over a normalized representation instead.

- No null guard in GetHashCode:
  - `GetHashCode(IMemberInformation obj)` assumes `obj` and its properties are non-null. Misuse could cause NREs (e.g., `obj.Name.GetHashCode()`).
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(obj);` (and if necessary, guard/normalize the strings) to fail fast with a clear error.

- Consistency with other comparers:
  - `TypeInformationComparer` and `TypeOnlyComparer` have slightly different identity considerations. Switching comparers across similar collections may lead to unexpected behavior.
  - Recommendation: Document intended use and differences. Consider aligning on a common canonical identity approach if feasible.
