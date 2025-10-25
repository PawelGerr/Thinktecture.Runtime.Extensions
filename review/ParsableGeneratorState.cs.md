Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ParsableGeneratorState.cs

- Default(struct) permits invalid null state:
  - As a readonly struct, `default(ParsableGeneratorState)` yields `Type == null` and `ValidationError` in its default state. `GetHashCode()` dereferences both (`Type.GetHashCode()` and `ValidationError.GetHashCode()`), which will throw NullReferenceException due to their internal implementations (e.g., `ValidationErrorState.GetHashCode` uses `TypeFullyQualified.GetHashCode()`).
  - Recommendation: Prefer a class to avoid invalid default, or add an `IsDefault/IsValid` pattern and guard usage. Alternatively ensure defaults never escape (e.g., private constructor + factories) and avoid calling `GetHashCode()`/`Equals` on defaults.

- Missing constructor argument validation:
  - The constructor does not validate `type`. Misuse (reflection/external) can produce invalid instances leading to NREs (e.g., in `GetHashCode`).
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`. Optional: validate invariants when `keyMember` is null (depending on generator expectations).

- Equality/hash rely on downstream value semantics:
  - Equality defers to `Type.Equals` and `MemberInformationComparer` for `KeyMember`. If these types’ equality semantics change or are not strictly value-based, this state’s equality may become unstable.
  - Recommendation: Document dependency and ensure `IParsableTypeInformation`/`IMemberInformation` implement consistent value-based equality/hash. Consider comparing canonical identities directly if feasible.

- Inconsistent use of explicit comparer in hash:
  - For equality, `MemberInformationComparer.Instance.Equals` is used; for hashing, `MemberInformationComparer.Instance.GetHashCode(KeyMember)` is used correctly. For `Type`, direct `Type.GetHashCode()` is used; if there is a corresponding comparer for type information (e.g., `TypeInformationComparer`), consider aligning for clarity and robustness.

- Consistency with project conventions:
  - Many state types implement `IHashCodeComputable` to standardize hashing; this one does not.
  - Recommendation: Implement `IHashCodeComputable` for consistency and testability.

- Operator overloads are redundant:
  - `==`/`!=` simply delegate to `Equals`. Acceptable but increases API surface without additional guarantees.
  - Recommendation: Keep if needed for ergonomics; otherwise relying on `Equals` is sufficient.
