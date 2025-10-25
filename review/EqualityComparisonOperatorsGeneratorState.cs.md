Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityComparisonOperatorsGeneratorState.cs

- Default(struct) produces invalid null state that breaks GetHashCode:
  - As a readonly struct, `default(EqualityComparisonOperatorsGeneratorState)` yields `Type == null`. `GetHashCode()` calls `TypeInformationComparer.Instance.GetHashCode(Type)`, which will throw for null.
  - Although `KeyMember` is nullable and guarded in hash (`? 0 : ...`), `Type` is not.
  - Recommendation: Prefer a class to avoid invalid default, or add `IsDefault/IsValid` semantics and guard usage in `GetHashCode/Equals`. At minimum, null-guard `Type` in `GetHashCode`.

- Missing constructor argument validation:
  - No checks for `type`. Misuse/reflection can create invalid instances that fail later.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`. Optionally validate that `OperatorsGeneration` is a defined enum value.

- Brittle comparer payload:
  - `EqualityComparer` is a nullable `ComparerInfo` (record struct). Prior findings indicate `ComparerInfo` (as record struct) allows default state where its internal `Comparer` string can be null, which can lead to invalid downstream codegen assumptions.
  - Recommendation: Normalize/validate `ComparerInfo` on construction (e.g., via factory) so that when present it always represents a well-formed comparer expression and intent.

- Invariants not validated:
  - Certain combinations may be inconsistent (e.g., `OperatorsGeneration.DefaultWithKeyTypeOverloads` with `KeyMember == null`; or `OperatorsGeneration.None` with a non-null `EqualityComparer` which will be ignored).
  - Recommendation: Validate or document required combinations to prevent inconsistent states from entering generators.

- Consistency with project conventions:
  - Many CodeAnalysis state types implement `IHashCodeComputable` for standardized hashing; this one does not.
  - Recommendation: Implement `IHashCodeComputable` for consistency and testability.

- Operator overloads redundancy:
  - `==`/`!=` delegate to `Equals`. Acceptable but increases API surface without additional guarantees.
  - Recommendation: Keep if ergonomics desired; otherwise rely on `Equals`.
