Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DelegateMethodState.cs

- Missing constructor argument validation:
  - `methodName` and `parameters` are not validated. Passing null (e.g., via reflection) will lead to NullReferenceException in `GetHashCode()` (`MethodName.GetHashCode()`) or while enumerating `Parameters`.
  - Recommendation: Add `ArgumentException.ThrowIfNullOrWhiteSpace(methodName);` and `ArgumentNullException.ThrowIfNull(parameters);`.

- Exposes potentially mutable collection:
  - The constructor accepts and stores `IReadOnlyList<ParameterState>` without defensive copying. If a mutable list is passed and modified later, equality/hash can change post-construction because `Equals` uses `Parameters.SequenceEqual(...)` and `GetHashCode()` uses `Parameters.ComputeHashCode()`.
  - Recommendation: Require an immutable type (e.g., `ImmutableArray<ParameterState>`) or defensively copy to an array to freeze state.

- Textual return type identity may be brittle:
  - `ReturnType` is a `string?` and is compared via string equality. Differences in formatting/qualification (e.g., `int` vs `System.Int32`, `global::` prefix, nullability annotations) can cause false inequality for semantically identical types.
  - Recommendation: Normalize to a canonical fully-qualified representation at construction, or derive from `ITypeSymbol` and store a stable identity.

- Inconsistent consideration of ArgumentName in equality/hash:
  - `ArgumentName` is derived (`delegateName ?? methodName`) but not included in `Equals`/`GetHashCode`. Other member-state types sometimes include `ArgumentName`, leading to inconsistency and potential future collisions if `ArgumentName` generation rules change independently of `MethodName`.
  - Recommendation: Standardize equality identity across member-state types (either always include `ArgumentName` or always omit it) and document the rationale.

- Null/empty handling of `DelegateName` not normalized:
  - `DelegateName` is compared directly; `null` and `""` will be considered different, which might be unintended if the semantics are “no custom name”.
  - Recommendation: Normalize empty/whitespace `delegateName` to `null` in the constructor to stabilize equality/hash.
