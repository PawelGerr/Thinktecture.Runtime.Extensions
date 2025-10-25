Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ModuleInfo.cs

- Default(record struct) permits invalid null state:
  - As a record struct, `default(ModuleInfo)` yields `Name == null` even though the property is non-nullable (`string`). This can cause NREs later if `Name` is used without checks.
  - Recommendation: Either change to a class to avoid invalid default, or provide `IsDefault/IsValid` semantics and guard usage. Alternatively, restrict construction via factory methods and keep the primary constructor internal/private.

- No validation/normalization of `Name`:
  - The primary constructor allows null/empty/whitespace at compile-time; consumers may assume a valid module name.
  - Recommendation: Add validation (e.g., `ArgumentException.ThrowIfNullOrWhiteSpace(name);`) and optionally normalize (trim, case policy) to stabilize identity.

- Textual identity brittleness:
  - Module names may differ by casing or include different file names (e.g., `.dll` presence) across environments. Relying on raw strings can lead to subtle mismatches.
  - Recommendation: Document expected format (e.g., exact `MetadataName` from Roslyn) and enforce normalization at construction.

- Consistency with project conventions:
  - Many state types implement `IHashCodeComputable` for deterministic hashing and consistency. This type does not and relies on record-generated hashing.
  - Recommendation: Implement `IHashCodeComputable` if uniformity is desired and base hashing on a normalized `Name`.
