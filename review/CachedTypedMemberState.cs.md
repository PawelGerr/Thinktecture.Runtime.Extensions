Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/CachedTypedMemberState.cs

- Missing constructor argument validation:
  - The constructor does not validate `typedMemberState`. If null is passed (reflection/misuse), dereferencing will throw immediately.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(typedMemberState);`.

- Identity-based equality breaks expected value semantics:
  - `Equals(object)`, `Equals(ITypedMemberState?)`, and `Equals(CachedTypedMemberState?)` all use `ReferenceEquals`. Two instances with identical copied values are considered not equal unless they are the same reference.
  - Impact: When used where value semantics are expected (e.g., de-duplication, set membership), logically equal states won’t compare equal. This diverges from other `ITypedMemberState` implementations that appear to use value-based equality (`TypedMemberState`).
  - Recommendation: Align equality with `ITypedMemberState`’s value semantics (compare the copied fields) or clearly document that this type is identity-based and should not be used in value-based collections. If identity-based is intended, consider not providing the `Equals(ITypedMemberState?)` overload to avoid misleading cross-type equality checks.

- Hash vs equality consistency may be surprising in mixed contexts:
  - `_hashCode` is computed from the original `typedMemberState` (value-based), but `Equals` is reference-based. Different instances with identical values can have identical hash codes yet not be equal, leading to dense hash buckets and performance degradation in dictionaries/sets.
  - Recommendation: If keeping identity-based equality, compute `_hashCode` from an identity-based strategy (e.g., `RuntimeHelpers.GetHashCode(this)`) to better match the equality semantics. Prefer value-based equality/hash for predictable behavior unless identity-based caching is absolutely required.

- Stale hash source risk if upstream equality logic changes:
  - `_hashCode` mirrors whatever `typedMemberState.GetHashCode()` uses at the time of construction. If the upstream type’s hash changes (e.g., inclusion/exclusion of fields), it can cause subtle inconsistencies relative to consumers expecting stable behavior from cached instances.
  - Recommendation: Derive hash from the copied fields in this class instead of the upstream instance, ensuring the hash matches exactly what this instance represents.

- Lack of explicit immutability guarantees:
  - The class copies values and doesn’t expose mutation, but immutability is not documented. If future changes add setters or mutable members, cached hash consistency could be violated.
  - Recommendation: Document immutability and keep the type `sealed` (already done). Optionally add XML docs to state that instances are immutable and safe for use as dictionary keys with the chosen semantics.
