Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedSerializerGeneratorState.cs

- Default(struct) produces invalid null state that breaks GetHashCode and property access:
  - As a readonly struct, `default(KeyedSerializerGeneratorState)` yields `Type == null`. `GetHashCode()` dereferences `Type` via `Type.GetHashCode()` causing NRE. Properties `Namespace`, `ContainingTypes`, and `Name` also dereference `Type` and will throw on default instances.
  - Recommendation: Prefer a class to avoid invalid default, or add `IsDefault/IsValid` semantics and guard `GetHashCode`/property access. At minimum, null-guard `Type` in `GetHashCode` and consider defensive null handling in forwarded properties.

- Missing constructor argument validation:
  - No checks for `type` or `attributeInfo`. Misuse/reflection can create invalid instances that fail later (e.g., NREs).
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);` and validate/normalize `attributeInfo` if it must be non-default. Optionally ensure `serializationFrameworks` is within valid flags.

- Inconsistent member equality vs project comparers:
  - Equality for `KeyMember` compares only `KeyMember?.TypeFullyQualified`. Other comparers in this project (e.g., `MemberInformationComparer`) include additional identity aspects (`Name`, `IsReferenceType`, `SpecialType`).
  - Impact: Two members with the same type but different names, ref/nullability, or special type can compare equal here but not elsewhere, leading to subtle bugs in sets/dictionaries.
  - Recommendation: Use `MemberInformationComparer.Instance.Equals(KeyMember, other.KeyMember)` and corresponding hash to align semantics.

- Potential duplication between `Type.Equals` and `ContainingTypes` comparison:
  - Equality compares `Type.Equals(other.Type)` and also `ContainingTypes.SequenceEqual(other.ContainingTypes)`. If `Type.Equals` already accounts for containing types, this double-counts and adds maintenance risk.
  - Recommendation: Ensure there is no duplication. Prefer comparing through a single canonical identity (either the `Type` or derived fully-qualified identity) to avoid drift.

- String hashing/equality intent implicit:
  - Uses `string.GetHashCode()` on `TypeFullyQualified` for `KeyMember`. This is ordinal and process-randomized; acceptable for in-memory collections but should not be used for persisted/cross-process hashing.
  - Recommendation: Document ordinal semantics; normalize strings (trim) if they originate from inputs where whitespace/casing can vary.

- Consistency with project conventions:
  - Many state types implement `IHashCodeComputable` for standardized hashing; this type does not.
  - Recommendation: Implement `IHashCodeComputable` for consistency and to enable reuse of stable hash logic.

- INamespaceAndName implementation robustness:
  - The forwarding properties (`Namespace`, `ContainingTypes`, `Name`) assume `Type` is non-null and valid. With default(struct) or misuse, these throw.
  - Recommendation: Either ensure instances cannot be default (class + private ctor + factories) or add explicit guard/contract documentation stating instances must be constructed via validated paths.
