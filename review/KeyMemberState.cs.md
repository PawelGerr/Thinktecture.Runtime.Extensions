Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyMemberState.cs

- Fragile reliance on ITypedMemberState equality/hash:
  - Equality and GetHashCode defer to _typedMemberState.Equals/GetHashCode. If an implementation of ITypedMemberState defaults to reference equality or changes its equality semantics, KeyMemberState equality/hash will become incorrect or unstable.
  - Recommendation: Ensure ITypedMemberState is guaranteed (by interface contract) to implement consistent value-based equality/hash (e.g., ITypedMemberState : IEquatable<ITypedMemberState>, IHashCodeComputable). Alternatively, compare the specific primitive properties of _typedMemberState that contribute to identity instead of relying on its Equals/GetHashCode.

- Potential NRE due to missing constructor argument validation:
  - The constructor does not validate typedMemberState or name. Despite NRT annotations, misuse (e.g., from reflection or external calls) would cause NullReferenceException in property accessors and GetHashCode (Name.GetHashCode()).
  - Recommendation: Add argument null checks (ArgumentNullException.ThrowIfNull(typedMemberState); ArgumentException.ThrowIfNullOrEmpty(name);) or clearly document/internalize the type to prevent misuse pathways.

- String comparison/hash consistency not explicit:
  - Equality uses Name == other.Name (ordinal by default) while hash uses Name.GetHashCode(). This is consistent in .NET, but the intent is implicit.
  - Recommendation: Consider documenting that comparisons are ordinal and rely on default string hashing, or use StringComparer.Ordinal where appropriate in custom comparers to make intent explicit.

- Equals(IMemberState?) overload depends on runtime type:
  - The overload returns false for IMemberState instances that are not KeyMemberState even if semantically identical (e.g., another implementation representing an equivalent key member). If cross-type equality is desirable at the IMemberState level, this may be too strict.
  - Recommendation: Confirm IMemberState equality semantics. If cross-implementation equality is intended, implement a common equality path based on interface-specified identity rather than concrete type.
