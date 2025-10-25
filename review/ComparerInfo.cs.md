Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparerInfo.cs

- Default(record struct) permits invalid null state:
  - As a record struct, `default(ComparerInfo)` produces `Comparer == null` even though the property is non-nullable (`string`). This can leak into code paths expecting a valid comparer and cause NREs later.
  - Recommendation: Either convert to a class to avoid invalid default, or add an `IsDefault/IsValid` pattern and guard usage. Alternatively, provide factory methods that enforce non-null and keep the constructor internal.

- No validation of `Comparer` content:
  - The primary constructor allows `Comparer` to be null/empty/whitespace at compile-time. Consumers may treat the value as an accessor or fully-qualified name and fail at runtime.
  - Recommendation: Add validation in a custom constructor or static factories (e.g., `ArgumentException.ThrowIfNullOrWhiteSpace(comparer);`). Consider normalizing the string (trim, enforce `global::` prefix) to stabilize identity.

- Textual identity is brittle:
  - Storing the comparer as a raw string is sensitive to formatting/aliasing (`global::`, usings, whitespace, generic type arguments formatting). Two semantically identical comparers may differ textually.
  - Recommendation: Represent the comparer as a structured identity (e.g., a symbol or a canonical fully-qualified name) or centralize normalization logic to ensure canonical form before storing.

- Ambiguity of `IsAccessor` semantics:
  - It is unclear what guarantees are provided when `IsAccessor` is true vs false (e.g., is `Comparer` an accessor expression vs a type name?). Without documented invariants, usage across code-gen may diverge and introduce subtle bugs.
  - Recommendation: Document invariants or replace the boolean with a discriminated union-like type (e.g., `ComparerAccessor` vs `ComparerTypeName`) to eliminate invalid combinations and make intent explicit.

- Consistency with project conventions:
  - Many CodeAnalysis state types implement `IHashCodeComputable` for uniformity and deterministic behavior. This type does not and relies on record-generated hash.
  - Recommendation: Implement `IHashCodeComputable` if consistency is desired, and ensure hashing is based on a normalized comparer string to avoid accidental collisions due to formatting differences.
