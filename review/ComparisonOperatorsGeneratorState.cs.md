Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparisonOperatorsGeneratorState.cs

- Default(struct) produces invalid null state that breaks GetHashCode:
  - As a readonly struct, `default(ComparisonOperatorsGeneratorState)` yields `Type == null`, `KeyMember == null`, and `CreateFactoryMethodName == null`. `GetHashCode()` dereferences all three via:
    - `TypeInformationComparer.Instance.GetHashCode(Type)` (Type null → NRE)
    - `MemberInformationComparer.Instance.GetHashCode(KeyMember)` (KeyMember null → NRE)
    - `CreateFactoryMethodName.GetHashCode()` (null → NRE)
  - Recommendation: Prefer a class to avoid invalid default, or add `IsDefault/IsValid` semantics and guard usage in `GetHashCode/Equals`. At minimum, null-guard these fields similarly to `(ComparerAccessor?.GetHashCode() ?? 0)`.

- Missing constructor argument validation:
  - No checks for `type`, `keyMember`, or `createFactoryMethodName`. Misuse/reflection can create invalid instances that fail later (hash/equality).
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`, `ArgumentNullException.ThrowIfNull(keyMember);`, and `ArgumentException.ThrowIfNullOrWhiteSpace(createFactoryMethodName);`.

- Raw string for `ComparerAccessor` is brittle:
  - Equality and hashing use raw string comparison/hash for `ComparerAccessor`, while code generators assume a specific contract (often `... .Comparer` appended).
  - Recommendation: Replace raw string with `ComparerInfo` (string + IsAccessor) to encode intent and enable correct codegen paths; normalize input (trim/canonicalize) to stabilize equality/hash.

- String equality/hash semantics implicit:
  - `CreateFactoryMethodName` and `ComparerAccessor` use default string `==` and `GetHashCode()`. Ordinal nature is implicit; casing/whitespace differences will impact identity; cross-process determinism is not guaranteed if persisted anywhere.
  - Recommendation: Normalize strings on construction (trim). If determinism across processes is ever needed, compute a canonical form or document ordinal semantics clearly.

- Invariants not validated:
  - `OperatorsGeneration`, `KeyMemberOperators`, and `ComparerAccessor` combinations may be incompatible (e.g., `DefaultWithKeyTypeOverloads` when key member does not support operators and no comparer is provided).
  - Recommendation: Validate or document expected combinations (e.g., when operators are required vs when a comparer is mandatory) to prevent inconsistent states from entering the generator pipeline.

- Consistency with project conventions:
  - Many state types implement `IHashCodeComputable` for standardized hashing; this one does not.
  - Recommendation: Implement `IHashCodeComputable` for consistency and testability.

- Operator overloads redundancy:
  - `==`/`!=` delegate to `Equals`. Acceptable but adds surface without additional guarantees.
  - Recommendation: Keep if ergonomics are required; otherwise rely on `Equals`.
