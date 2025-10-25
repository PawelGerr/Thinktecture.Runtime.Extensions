Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/GenericTypeParameterState.cs

- Warning: Mutability leak via Constraints property
  - Constraints is accepted and exposed as IReadOnlyList<string> without a defensive copy. If a mutable list instance is passed in and later modified externally, equality and hash code invariants can be violated (e.g., after insertion into a Dictionary/HashSet).
  - Recommendation: Store an immutable copy (e.g., ImmutableArray<string> or ToArray wrapped in ReadOnlyCollection) and/or change the property type accordingly.

- Warning: Null-safety of constructor parameters and elements
  - The constructor doesn’t guard against nulls for name or constraints. With Nullable Reference Types this is usually fine, but if called from oblivious contexts/suppressed null warnings, null can flow in. Consequences:
    - GetHashCode will throw NullReferenceException if Name is null.
    - Constraints hashing uses StringComparer.Ordinal in ComputeHashCode; many comparers throw for null elements, so a null element in Constraints can cause an exception; also creates inconsistency with equality where SequenceEqual tolerates null elements.
  - Recommendation: Validate name and constraints != null and ensure no null elements in constraints (or use a null-tolerant hashing approach).

- Warning: Hashing consistency and explicitness
  - Equals uses ordinal semantics; GetHashCode uses Name.GetHashCode(), which is ordinal but implicit. For clarity and consistency with Constraints hashing, prefer StringComparer.Ordinal.GetHashCode(Name).
  - Recommendation: Replace Name.GetHashCode() with StringComparer.Ordinal.GetHashCode(Name) to mirror the explicit comparer used for Constraints.

- Minor: Optional equality operators
  - Consider adding operator ==/!= consistent with Equals/GetHashCode if instances are compared frequently. Not required but improves ergonomics.
