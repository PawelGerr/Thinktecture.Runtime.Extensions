Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ContainingTypeState.cs

- Exposes a potentially mutable collection:
  - The constructor accepts and stores IReadOnlyList<GenericTypeParameterState> without defensive copying. If a mutable list (e.g., List<>) is passed and later modified by the caller, the observable state of the instance changes after construction. This can break value semantics and violate dictionary/set invariants when used as a key because Equals/GetHashCode depend on GenericParameters.
  - Recommendation: Require an immutable type (e.g., ImmutableArray<GenericTypeParameterState>) or defensively copy to an array to freeze state at construction.

- Potential equality/hash collision across namespaces:
  - Equality and GetHashCode consider only Name, IsReferenceType, IsRecord, and GenericParameters. Namespace of the containing type is not part of the state, so 2 containing types with the same simple name but different namespaces would compare equal and produce the same hash.
  - This is only safe if ContainingTypeState instances are never compared across different namespaces or the namespace is accounted for by an outer state. Otherwise, this is a correctness issue.
  - Recommendation: Include namespace (or fully-qualified name) into the state/equality/hash or document/enforce that comparisons are constrained to the same namespace context.
