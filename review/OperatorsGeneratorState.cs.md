Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/OperatorsGeneratorState.cs

- Default(struct) produces invalid null state that breaks GetHashCode:
  - As a readonly struct, `default(OperatorsGeneratorState)` yields `Type == null`, `KeyMember == null`, `CreateFactoryMethodName == null`, and `GeneratorProvider == null`.
  - `GetHashCode()` dereferences all of them via:
    - `TypeInformationComparer.Instance.GetHashCode(Type)` (Type null → NRE)
    - `MemberInformationComparer.Instance.GetHashCode(KeyMember)` (KeyMember null → NRE)
    - `CreateFactoryMethodName.GetHashCode()` (null → NRE)
    - `GeneratorProvider.GetHashCode()` (null → NRE)
  - Recommendation: Prefer a class to avoid invalid default, or add `IsDefault/IsValid` semantics and guard `GetHashCode/Equals`. At minimum, null-guard these fields in `GetHashCode` similarly to `(ComparerAccessor?.GetHashCode() ?? 0)` patterns used elsewhere.

- Missing constructor argument validation:
  - No checks for `type`, `keyMember`, `createFactoryMethodName`, or `generatorProvider`. Misuse/reflection can create invalid instances that crash later.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`, `ThrowIfNull(keyMember);`, `ArgumentException.ThrowIfNullOrWhiteSpace(createFactoryMethodName);`, and `ThrowIfNull(generatorProvider);`. Optionally validate `OperatorsGeneration` is a defined enum value.

- Reference-based equality for GeneratorProvider but value of hash may be overridden:
  - Equality uses `ReferenceEquals(GeneratorProvider, other.GeneratorProvider)` (identity), but hashing uses `GeneratorProvider.GetHashCode()`. If a provider overrides `GetHashCode` to be value-based, the equality is still identity-based, which is fine for correctness (equal ⇒ equal hash still holds), but can be surprising.
  - Recommendation: Make intent explicit by using reference-identity hashing (e.g., `RuntimeHelpers.GetHashCode(GeneratorProvider)`) or document that provider identity participates in equality. Alternatively, compare on a stable provider identity (e.g., provider type or name) and hash the same identity.

- String equality/hash semantics implicit:
  - Uses `==` and `GetHashCode()` for `CreateFactoryMethodName`. This is ordinal and process-randomized; whitespace/casing differences can cause surprising inequality.
  - Recommendation: Normalize (trim/canonicalize) `CreateFactoryMethodName` on construction. If deterministic hashing across processes is required anywhere, compute a stable hash over a canonical representation and document ordinal semantics.

- Invariants not validated:
  - `OperatorsGeneration`, `KeyMemberOperators`, and `GeneratorProvider` combinations may be incompatible (e.g., operator generation requiring certain key member operators).
  - Recommendation: Validate or document required combinations to prevent inconsistent states entering the generator pipeline.

- Consistency with project conventions:
  - Many CodeAnalysis state types implement `IHashCodeComputable`; this one does not.
  - Recommendation: Implement `IHashCodeComputable` for consistency and testability.

- Operator overloads redundancy:
  - `==`/`!=` delegate to `Equals`. Acceptable but increases API surface without added guarantees.
  - Recommendation: Keep if ergonomics desired; otherwise rely on `Equals` only.
