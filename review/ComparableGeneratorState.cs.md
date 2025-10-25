Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparableGeneratorState.cs

- Default(struct) produces invalid null state that breaks GetHashCode:
  - As a readonly struct, `default(ComparableGeneratorState)` yields `Type == null`, `KeyMember == null`, and `CreateFactoryMethodName == null`. `GetHashCode()` dereferences all three via:
    - `TypeInformationComparer.Instance.GetHashCode(Type)` (Type null → NRE)
    - `MemberInformationComparer.Instance.GetHashCode(KeyMember)` (KeyMember null → NRE)
    - `CreateFactoryMethodName.GetHashCode()` (null → NRE)
  - Recommendation: Prefer a class to avoid invalid default, or add `IsDefault/IsValid` semantics and guard usage in `GetHashCode/Equals`. At minimum, null-guard each field in `GetHashCode` similar to `ComparerAccessor?.GetHashCode() ?? 0`.

- Missing constructor argument validation:
  - No checks for `type`, `keyMember`, or `createFactoryMethodName`. Misuse/reflection can create invalid instances that crash later.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`, `ArgumentNullException.ThrowIfNull(keyMember);`, and `ArgumentException.ThrowIfNullOrWhiteSpace(createFactoryMethodName);`.

- Raw string for ComparerAccessor is brittle:
  - `ComparerAccessor` is a raw string; downstream code (ComparableCodeGenerator) assumes a specific shape (e.g., `.Comparer` property). This is fragile and easy to misuse.
  - Recommendation: Replace with `ComparerInfo` (string + IsAccessor) to encode intent and generate correct code paths without guesswork. Normalize/trim input to avoid equality/hash surprises.

- String equality/hash semantics implicit:
  - Equality uses `==` and hashing uses `string.GetHashCode()` on `CreateFactoryMethodName` and `ComparerAccessor`. While fine for in-memory collections, the ordinal nature is implicit and defaults can differ by runtime.
  - Recommendation: Normalize strings (trim) and consider canonicalization. If persisted/deterministic hashing is required, compute a stable hash over a canonical form; otherwise document ordinal semantics.

- Invariant checks not enforced:
  - Fields like `SkipIComparable` and `IsKeyMemberComparable` imply invariants (e.g., when `SkipIComparable` is true, comparable generation should not occur). The state doesn’t validate incompatible combinations.
  - Recommendation: Validate or document expected combinations to prevent inconsistent states entering generation pipelines.

- Consistency with project conventions:
  - Many CodeAnalysis state types implement `IHashCodeComputable`; this type does not.
  - Recommendation: Implement `IHashCodeComputable` for consistency and testability.

- Operator overloads redundancy:
  - `==`/`!=` delegate to `Equals`. Acceptable but increases surface area without added guarantees.
  - Recommendation: Keep only if ergonomic value outweighs maintenance; otherwise rely on `Equals`.
