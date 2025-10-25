Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypeInformationComparer.cs

- Potentially insufficient identity: ignores nullability and value-type nuances
  - Equality is based on `TypeFullyQualified` and `IsReferenceType` only. If `TypeFullyQualified` does not encode nullability (NRT `?`) or other identity aspects (generic arity/arguments, array ranks), distinct types may compare equal.
  - Additionally, value-type nullability (`Nullable<T>`) and `IsNullableStruct` are not considered. Two types with same FQN but different nullability/state could collide depending on how `TypeFullyQualified` is produced.
  - Recommendation: Ensure `TypeFullyQualified` is a canonical representation encoding nullability, generics, and array ranks; or incorporate additional properties (e.g., `NullableAnnotation`, `IsNullableStruct`, arity) into equality/hash.

- Redundant/unclear use of IsReferenceType with FQN
  - If `TypeFullyQualified` already encodes nullable reference annotations (e.g., `string?` vs `string`), using `IsReferenceType` is redundant and may still miss differences for value-type nullability.
  - Recommendation: Either rely on a fully-canonical FQN exclusively, or extend the comparer to capture all required distinctions explicitly.

- String comparison/hash intent not explicit
  - Equality uses `==` and hash uses `GetHashCode()` on strings, which are ordinal and randomized per process. While fine for in-memory collections, this should not be relied upon for persisted/cross-process hashing.
  - Recommendation: Document ordinal semantics. If a comparer instance is ever serialized or used across app domains, consider computing a deterministic hash over a normalized representation.

- No guard for null in GetHashCode
  - `GetHashCode(ITypeInformation obj)` assumes `obj` is non-null. Although `IEqualityComparer` is typically only called with non-null keys by collections, a defensive null-check could prevent misuse.
  - Recommendation: Optionally add `ArgumentNullException.ThrowIfNull(obj);` for robustness.

- Consistency with other comparers
  - There is also a `TypeOnlyComparer` (to be reviewed). Ensure the responsibilities between comparers are clearly separated and documented to avoid subtle mismatches when used interchangeably.
