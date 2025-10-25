Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DefaultMemberState.cs

- Fragile reliance on ITypedMemberState equality/hash:
  - Equality and GetHashCode delegate to _typedMemberState.Equals/GetHashCode. If ITypedMemberState implementations use reference equality or change semantics, DefaultMemberState value semantics become unstable.
  - Recommendation: Enforce value-based equality on ITypedMemberState (via interface contract like IEquatable<ITypedMemberState> and IHashCodeComputable) or compare the specific primitive identity properties instead of delegating.

- Missing constructor argument validation:
  - No null/empty checks for typedMemberState or name. Misuse (reflection, external code paths) can result in NullReferenceException (e.g., Name.GetHashCode()) or invalid instances.
  - Recommendation: Add ArgumentNullException.ThrowIfNull(typedMemberState); and ArgumentException.ThrowIfNullOrWhiteSpace(name);.

- IMemberState equality limited to concrete type:
  - Equals(IMemberState?) returns false for other IMemberState implementations even if semantically equivalent. This may be fine if equality is intentionally type-specific, but it prevents cross-implementation equality at the interface level.
  - Recommendation: Clarify IMemberState equality semantics. If cross-implementation equality is desired, base it on interface-defined identity (type information + name + argument name), not the concrete type.

- String comparison/hash intent not explicit:
  - Equality uses Name == other.Name and hash uses Name.GetHashCode(). While consistent in .NET (ordinal), intent is implicit.
  - Recommendation: Document ordinal semantics or employ StringComparer.Ordinal where a comparer is required to make intent explicit.
